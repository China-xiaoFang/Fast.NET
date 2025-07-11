// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

// ReSharper disable once CheckNamespace
namespace Fast.JwtBearer;

/// <summary>
/// <see cref="JwtBearerUtil"/> JwtBearer 工具类
/// </summary>
public static class JwtBearerUtil
{
    /// <summary>
    /// 日期类型的 Claim 类型
    /// </summary>
    public static readonly string[] DateTypeClaimTypes =
    [
        JwtRegisteredClaimNames.Iat, JwtRegisteredClaimNames.Nbf, JwtRegisteredClaimNames.Exp
    ];

    /// <summary>
    /// 刷新 Token 身份标识
    /// </summary>
    public static readonly string[] RefreshTokenClaims = ["f", "e", "s", "l", "k"];

    /// <summary>
    /// 生成Token验证参数
    /// </summary>
    /// <param name="jwtSettings"></param>
    /// <returns></returns>
    public static TokenValidationParameters CreateTokenValidationParameters(JWTSettingsOptions jwtSettings)
    {
        return new TokenValidationParameters
        {
            // 验证签发方密钥
            ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey ?? false,
            // 签发方密钥
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.IssuerSigningKey)),
            // 验证签发方
            ValidateIssuer = jwtSettings.ValidateIssuer ?? false,
            // 设置签发方
            ValidIssuer = jwtSettings.ValidIssuer,
            // 验证签收方
            ValidateAudience = jwtSettings.ValidateAudience ?? false,
            // 设置接收方
            ValidAudience = jwtSettings.ValidAudience,
            // 验证生存期
            ValidateLifetime = jwtSettings.ValidateLifetime ?? false,
            // 过期时间容错值
            ClockSkew = TimeSpan.FromSeconds(jwtSettings.ClockSkew ?? 5)
        };
    }

    /// <summary>
    /// 生成 Token
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="expiredTime">过期时间（分钟）</param>
    /// <returns></returns>
    public static string GenerateToken(IDictionary<string, object> payload, long? expiredTime = null)
    {
        var datetimeOffset = DateTimeOffset.UtcNow;

        if (!payload.ContainsKey(JwtRegisteredClaimNames.Iat))
        {
            payload.Add(JwtRegisteredClaimNames.Iat, datetimeOffset.ToUnixTimeSeconds());
        }

        if (!payload.ContainsKey(JwtRegisteredClaimNames.Nbf))
        {
            payload.Add(JwtRegisteredClaimNames.Nbf, datetimeOffset.ToUnixTimeSeconds());
        }

        if (!payload.ContainsKey(JwtRegisteredClaimNames.Exp))
        {
            var minute = expiredTime ?? Penetrates.JWTSettings?.TokenExpiredTime ?? 20;
            payload.Add(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(minute)
                .ToUnixTimeSeconds());
        }

        if (!payload.ContainsKey(JwtRegisteredClaimNames.Iss))
        {
            payload.Add(JwtRegisteredClaimNames.Iss, Penetrates.JWTSettings?.ValidIssuer);
        }

        if (!payload.ContainsKey(JwtRegisteredClaimNames.Aud))
        {
            payload.Add(JwtRegisteredClaimNames.Aud, Penetrates.JWTSettings?.ValidAudience);
        }

        // 处理 JwtPayload 序列化不一致问题
        var stringPayload = payload is JwtPayload jwtPayload
            ? jwtPayload.SerializeToJson()
            : JsonSerializer.Serialize(payload,
                new JsonSerializerOptions {Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping});

        SigningCredentials credentials = null;

        if (!string.IsNullOrWhiteSpace(Penetrates.JWTSettings?.IssuerSigningKey))
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Penetrates.JWTSettings?.IssuerSigningKey));
            credentials = new SigningCredentials(securityKey,
                Penetrates.JWTSettings?.Algorithm?.ToString() ?? SecurityAlgorithms.HmacSha256);
        }

        var tokenHandler = new JsonWebTokenHandler();
        return credentials == null
            ? tokenHandler.CreateToken(stringPayload)
            : tokenHandler.CreateToken(stringPayload, credentials);
    }

    /// <summary>
    /// 生成刷新 Token
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public static string GenerateRefreshToken(string accessToken)
    {
        // 分割Token
        var tokenParagraphs = accessToken.Split('.', StringSplitOptions.RemoveEmptyEntries);

        var s = RandomNumberGenerator.GetInt32(10, tokenParagraphs[1].Length / 2 + 2);
        var l = RandomNumberGenerator.GetInt32(3, 13);

        var payload = new Dictionary<string, object>
        {
            {"f", tokenParagraphs[0]},
            {"e", tokenParagraphs[2]},
            {"s", s},
            {"l", l},
            {
                "k", tokenParagraphs[1]
                    .Substring(s, l)
            }
        };

        return GenerateToken(payload, Penetrates.JWTSettings?.RefreshTokenExpireTime ?? 43200);
    }

    /// <summary>
    /// 获取 JWT Bearer Token
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="headerKey"></param>
    /// <param name="tokenPrefix"></param>
    /// <returns></returns>
    public static string GetJwtBearerToken(HttpContext httpContext, string headerKey = "Authorization",
        string tokenPrefix = "Bearer ")
    {
        // 判断请求报文头中是否有 "Authorization" 报文头
        var bearerToken = httpContext.Request.Headers[headerKey]
            .ToString();
        if (string.IsNullOrWhiteSpace(bearerToken))
            return null;

        var prefixLength = tokenPrefix.Length;
        return bearerToken.StartsWith(tokenPrefix, true, null) && bearerToken.Length > prefixLength
            ? bearerToken[prefixLength..]
            : null;
    }

    /// <summary>
    /// 验证 Token
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public static (bool IsValid, JsonWebToken Token, TokenValidationResult validationResult) Validate(string accessToken)
    {
        if (Penetrates.JWTSettings == null)
            return (false, null, null);

        // 加密Key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Penetrates.JWTSettings.IssuerSigningKey));
        var cress = new SigningCredentials(key, Penetrates.JWTSettings?.Algorithm?.ToString() ?? SecurityAlgorithms.HmacSha256);

        // 创建Token验证参数
        var tokenValidationParameters = CreateTokenValidationParameters(Penetrates.JWTSettings);
        tokenValidationParameters.IssuerSigningKey ??= cress.Key;

        // 验证 Token
        var tokenHandler = new JsonWebTokenHandler();
        try
        {
#if NET8_0
            // 处理 .NET8 中 ValidateToken 方法已过时的警告
            var tokenValidationResult = tokenHandler.ValidateTokenAsync(accessToken, tokenValidationParameters)
                .Result;
#else
            var tokenValidationResult = tokenHandler.ValidateToken(accessToken, tokenValidationParameters);
#endif
            if (!tokenValidationResult.IsValid)
                return (false, null, tokenValidationResult);

            var jsonWebToken = tokenValidationResult.SecurityToken as JsonWebToken;
            return (true, jsonWebToken, tokenValidationResult);
        }
        catch
        {
            return (false, null, null);
        }
    }

    /// <summary>
    /// 验证 Token
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="token"></param>
    /// <param name="headerKey"></param>
    /// <param name="tokenPrefix"></param>
    /// <returns></returns>
    public static bool ValidateJwtBearerToken(DefaultHttpContext httpContext, out JsonWebToken token,
        string headerKey = "Authorization", string tokenPrefix = "Bearer ")
    {
        // 获取 token
        var accessToken = GetJwtBearerToken(httpContext, headerKey, tokenPrefix);
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            token = null;
            return false;
        }

        // 验证token
        var (IsValid, Token, _) = Validate(accessToken);
        token = IsValid ? Token : null;

        return IsValid;
    }

    /// <summary>
    /// 读取 Token，不含验证
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public static JsonWebToken ReadJwtToken(string accessToken)
    {
        var tokenHandler = new JsonWebTokenHandler();
        if (tokenHandler.CanReadToken(accessToken))
        {
            return tokenHandler.ReadJsonWebToken(accessToken);
        }

        return null;
    }

    /// <summary>
    /// 读取 Token
    /// </summary>
    /// <remarks>会验证签名等</remarks>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public static JwtSecurityToken SecurityReadJwtToken(string accessToken)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(accessToken);
        return jwtSecurityToken;
    }

    /// <summary>
    /// 通过过期Token 和 刷新Token 换取新的 Token
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="expiredToken"></param>
    /// <param name="refreshToken"></param>
    /// <param name="expiredTime">过期时间（分钟）</param>
    /// <param name="clockSkew">刷新token容差值，秒做单位</param>
    /// <returns></returns>
    public static string Exchange(HttpContext httpContext, string expiredToken, string refreshToken, long? expiredTime = null,
        long? clockSkew = null)
    {
        // 交换刷新Token 必须原Token 已过期
        var (_isValid, _, _) = Validate(expiredToken);
        if (_isValid)
            return null;

        // 判断刷新Token 是否过期
        var (isValid, refreshTokenObj, _) = Validate(refreshToken);
        if (!isValid)
            return null;

        // 判断这个刷新Token 是否已刷新过
        var blacklistRefreshKey = "BLACKLIST_REFRESH_TOKEN:" + refreshToken;
        var distributedCache = httpContext?.RequestServices.GetService<IDistributedCache>();

        // 处理token并发容错问题
        var nowTime = DateTimeOffset.UtcNow;
        var cachedValue = distributedCache?.GetString(blacklistRefreshKey);
        var isRefresh = !string.IsNullOrWhiteSpace(cachedValue); // 判断是否刷新过
        if (isRefresh)
        {
            var refreshTime = new DateTimeOffset(long.Parse(cachedValue), TimeSpan.Zero);
            // 处理并发时容差值
            if ((nowTime - refreshTime).TotalSeconds > (clockSkew ?? Penetrates.JWTSettings?.ClockSkew ?? 5))
                return null;
        }

        // 分割过期Token
        var tokenParagraphs = expiredToken.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (tokenParagraphs.Length < 3)
            return null;

        // 判断各个部分是否匹配
        if (!refreshTokenObj.GetPayloadValue<string>("f")
                .Equals(tokenParagraphs[0]))
            return null;
        if (!refreshTokenObj.GetPayloadValue<string>("e")
                .Equals(tokenParagraphs[2]))
            return null;
        if (!tokenParagraphs[1]
                .Substring(refreshTokenObj.GetPayloadValue<int>("s"), refreshTokenObj.GetPayloadValue<int>("l"))
                .Equals(refreshTokenObj.GetPayloadValue<string>("k")))
            return null;

        // 获取过期 Token 的存储信息
        var jwtSecurityToken = SecurityReadJwtToken(expiredToken);
        var payload = jwtSecurityToken.Payload;

        // 移除 Iat，Nbf，Exp
        foreach (var innerKey in DateTypeClaimTypes)
        {
            if (!payload.ContainsKey(innerKey))
                continue;

            payload.Remove(innerKey);
        }

        // 交换成功后登记刷新Token，标记失效
        if (!isRefresh)
        {
            distributedCache?.SetString(blacklistRefreshKey, nowTime.Ticks.ToString(),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration =
                        DateTimeOffset.FromUnixTimeSeconds(refreshTokenObj.GetPayloadValue<long>(JwtRegisteredClaimNames.Exp))
                });
        }

        return GenerateToken(payload, expiredTime);
    }

    /// <summary>
    /// 标记过期 Token
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="expiredToken"></param>
    public static void SetExpiredToken(HttpContext httpContext, string expiredToken)
    {
        if (string.IsNullOrEmpty(expiredToken))
            return;

        // 标记过期 必须原Token 是有效的
        var (_isValid, accessTokenObj, _) = Validate(expiredToken);
        if (!_isValid)
            return;

        var nowTime = DateTimeOffset.UtcNow;
        var blacklistAccessKey = "BLACKLIST_ACCESS_TOKEN:" + expiredToken;
        var distributedCache = httpContext?.RequestServices.GetService<IDistributedCache>();

        // 标记失效
        distributedCache?.SetString(blacklistAccessKey, nowTime.Ticks.ToString(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpiration =
                    DateTimeOffset.FromUnixTimeSeconds(accessTokenObj.GetPayloadValue<long>(JwtRegisteredClaimNames.Exp))
            });
    }

    /// <summary>
    /// 自动刷新 Token 信息
    /// </summary>
    /// <param name="context"></param>
    /// <param name="httpContext"></param>
    /// <param name="expiredTime">新 Token 过期时间（分钟）</param>
    /// <param name="tokenPrefix"></param>
    /// <param name="clockSkew"></param>
    /// <returns></returns>
    public static bool AutoRefreshToken(AuthorizationHandlerContext context, HttpContext httpContext, long? expiredTime = null,
        string tokenPrefix = "Bearer ", long? clockSkew = null)
    {
        // 如果验证有效，则跳过刷新
        if (context.User.Identity?.IsAuthenticated == true)
        {
            // 禁止使用刷新 Token 进行单独校验
            if (RefreshTokenClaims.All(k => context.User.Claims.Any(c => c.Type == k)))
            {
                return false;
            }

            // 判断是否含有匿名特性
            if (httpContext.GetEndpoint()
                    ?.Metadata.GetMetadata<AllowAnonymousAttribute>()
                != null)
                return true;

            // 判断是否开启验证 AccessToken
            if (Penetrates.JWTSettings?.ValidateAccessToken == true)
            {
                // 读取Token
                var accessToken = GetJwtBearerToken(httpContext, tokenPrefix: tokenPrefix);
                if (string.IsNullOrWhiteSpace(accessToken))
                    return false;

                // 判断这个Token 是否已标记过期
                var blacklistAccessKey = "BLACKLIST_ACCESS_TOKEN:" + accessToken;
                var distributedCache = httpContext.RequestServices.GetService<IDistributedCache>();

                var cachedValue = distributedCache?.GetString(blacklistAccessKey);
                if (!string.IsNullOrWhiteSpace(cachedValue))
                    return false;
            }

            return true;
        }

        // 判断是否含有匿名特性
        if (httpContext.GetEndpoint()
                ?.Metadata.GetMetadata<AllowAnonymousAttribute>()
            != null)
            return true;

        // 获取过期Token 和 刷新Token
        var expiredToken = GetJwtBearerToken(httpContext, tokenPrefix: tokenPrefix);
        var refreshToken = GetJwtBearerToken(httpContext, "X-Authorization", tokenPrefix);
        if (string.IsNullOrWhiteSpace(expiredToken) || string.IsNullOrWhiteSpace(refreshToken))
            return false;

        // 交换新的 Token
        var newAccessToken = Exchange((context.Resource as AuthorizationFilterContext)?.HttpContext, expiredToken, refreshToken,
            expiredTime, clockSkew);
        if (string.IsNullOrWhiteSpace(newAccessToken))
            return false;

        // 读取新的 Token Clamis
        var claims = ReadJwtToken(newAccessToken)
            ?.Claims;
        if (claims == null)
            return false;

        // 创建身份信息
        var claimIdentity = new ClaimsIdentity("AuthenticationTypes.Federation");
        claimIdentity.AddClaims(claims);
        var claimsPrincipal = new ClaimsPrincipal(claimIdentity);

        // 设置 HttpContext.User 并登录
        httpContext.User = claimsPrincipal;
        httpContext.SignInAsync(claimsPrincipal);

        string accessTokenKey = "access-token",
            xAccessTokenKey = "x-access-token",
            accessControlExposeKey = "Access-Control-Expose-Headers";

        // 返回新的 Token
        httpContext.Response.Headers[accessTokenKey] = newAccessToken;
        // 返回新的 刷新Token
        httpContext.Response.Headers[xAccessTokenKey] = GenerateRefreshToken(newAccessToken);

        // 处理 axios 问题
        httpContext.Response.Headers.TryGetValue(accessControlExposeKey, out var aches);
        httpContext.Response.Headers[accessControlExposeKey] = string.Join(',',
            StringValues.Concat(aches, new StringValues([accessTokenKey, xAccessTokenKey]))
                .Distinct());

        return true;
    }
}