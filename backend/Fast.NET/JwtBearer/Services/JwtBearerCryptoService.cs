// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Fast.IaaS;
using Fast.JwtBearer.Internal;
using Fast.JwtBearer.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Fast.JwtBearer.Services;

/// <summary>
/// <see cref="JwtBearerCryptoService"/> JwtBearer 加密解密服务
/// </summary>
#if NET8_0
public class JwtBearerCryptoService(IOptions<JWTSettingsOptions> jwtSettings) : IJwtBearerCryptoService
{
    private readonly JWTSettingsOptions _jwtSettings = jwtSettings.Value.LoadPostConfigure();
#else
public class JwtBearerCryptoService : IJwtBearerCryptoService
{
    private readonly JWTSettingsOptions _jwtSettings;

    public JwtBearerCryptoService(IOptions<JWTSettingsOptions> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value.LoadPostConfigure();
    }
#endif

    /// <summary>
    /// 生成 Token
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="expiredTime">过期时间（分钟）</param>
    /// <returns></returns>
    public string GenerateToken(IDictionary<string, object> payload, long? expiredTime = null)
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
            var minute = expiredTime ?? _jwtSettings?.TokenExpiredTime ?? 20;
            payload.Add(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(minute).ToUnixTimeSeconds());
        }

        if (!payload.ContainsKey(JwtRegisteredClaimNames.Iss))
        {
            payload.Add(JwtRegisteredClaimNames.Iss, _jwtSettings?.ValidIssuer);
        }

        if (!payload.ContainsKey(JwtRegisteredClaimNames.Aud))
        {
            payload.Add(JwtRegisteredClaimNames.Aud, _jwtSettings?.ValidAudience);
        }

        // 处理 JwtPayload 序列化不一致问题
        var stringPayload = payload is JwtPayload jwtPayload
            ? jwtPayload.SerializeToJson()
            : JsonSerializer.Serialize(payload,
                new JsonSerializerOptions {Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping});

        SigningCredentials credentials = null;

        if (!string.IsNullOrWhiteSpace(_jwtSettings?.IssuerSigningKey))
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings?.IssuerSigningKey));
            credentials =
                new SigningCredentials(securityKey, _jwtSettings?.Algorithm?.ToString() ?? SecurityAlgorithms.HmacSha256);
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
    public string GenerateRefreshToken(string accessToken)
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
            {"k", tokenParagraphs[1].Substring(s, l)}
        };

        return GenerateToken(payload, _jwtSettings?.RefreshTokenExpireTime ?? 43200);
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
    public string Exchange(HttpContext httpContext, string expiredToken, string refreshToken, long? expiredTime = null,
        long? clockSkew = null)
    {
        // 交换刷新Token 必须原Token 已过期
        var (_isValid, _, _) = Validate(expiredToken);
        if (_isValid)
            return default;

        // 判断刷新Token 是否过期
        var (isValid, refreshTokenObj, _) = Validate(refreshToken);
        if (!isValid)
            return default;

        // 判断这个刷新Token 是否已刷新过
        var blacklistRefreshKey = "BLACKLIST_REFRESH_TOKEN:" + refreshToken;
        var distributedCache = httpContext?.RequestServices?.GetService<IDistributedCache>();

        // 处理token并发容错问题
        var nowTime = DateTimeOffset.UtcNow;
        var cachedValue = distributedCache?.GetString(blacklistRefreshKey);
        var isRefresh = !string.IsNullOrWhiteSpace(cachedValue); // 判断是否刷新过
        if (isRefresh)
        {
            var refreshTime = new DateTimeOffset(long.Parse(cachedValue), TimeSpan.Zero);
            // 处理并发时容差值
            if ((nowTime - refreshTime).TotalSeconds > (clockSkew ?? _jwtSettings?.ClockSkew ?? 5))
                return default;
        }

        // 分割过期Token
        var tokenParagraphs = expiredToken.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (tokenParagraphs.Length < 3)
            return default;

        // 判断各个部分是否匹配
        if (!refreshTokenObj.GetPayloadValue<string>("f").Equals(tokenParagraphs[0]))
            return default;
        if (!refreshTokenObj.GetPayloadValue<string>("e").Equals(tokenParagraphs[2]))
            return default;
        if (!tokenParagraphs[1].Substring(refreshTokenObj.GetPayloadValue<int>("s"), refreshTokenObj.GetPayloadValue<int>("l"))
                .Equals(refreshTokenObj.GetPayloadValue<string>("k")))
            return default;

        // 获取过期 Token 的存储信息
        var jwtSecurityToken = SecurityReadJwtToken(expiredToken);
        var payload = jwtSecurityToken.Payload;

        // 移除 Iat，Nbf，Exp
        foreach (var innerKey in Penetrates.DateTypeClaimTypes)
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
    /// 自动刷新 Token 信息
    /// </summary>
    /// <param name="context"></param>
    /// <param name="httpContext"></param>
    /// <param name="expiredTime">新 Token 过期时间（分钟）</param>
    /// <param name="tokenPrefix"></param>
    /// <param name="clockSkew"></param>
    /// <returns></returns>
    public bool AutoRefreshToken(AuthorizationHandlerContext context, HttpContext httpContext, long? expiredTime = null,
        string tokenPrefix = "Bearer ", long? clockSkew = null)
    {
        // 如果验证有效，则跳过刷新
        if (context.User.Identity?.IsAuthenticated == true)
        {
            // 禁止使用刷新 Token 进行单独校验
            if (Penetrates.RefreshTokenClaims.All(k => context.User.Claims.Any(c => c.Type == k)))
            {
                return false;
            }

            return true;
        }

        // 判断是否含有匿名特性
        if (httpContext.GetEndpoint()?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null)
            return true;

        // 获取过期Token 和 刷新Token
        var expiredToken = GetJwtBearerToken(httpContext, tokenPrefix: tokenPrefix);
        var refreshToken = GetJwtBearerToken(httpContext, "X-Authorization", tokenPrefix: tokenPrefix);
        if (string.IsNullOrWhiteSpace(expiredToken) || string.IsNullOrWhiteSpace(refreshToken))
            return false;

        // 交换新的 Token
        var accessToken = Exchange((context.Resource as AuthorizationFilterContext)?.HttpContext, expiredToken, refreshToken,
            expiredTime, clockSkew);
        if (string.IsNullOrWhiteSpace(accessToken))
            return false;

        // 读取新的 Token Clamis
        var claims = ReadJwtToken(accessToken)?.Claims;
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
        httpContext.Response.Headers[accessTokenKey] = accessToken;
        // 返回新的 刷新Token
        httpContext.Response.Headers[xAccessTokenKey] = GenerateRefreshToken(accessToken);

        // 处理 axios 问题
        httpContext.Response.Headers.TryGetValue(accessControlExposeKey, out var aches);
        httpContext.Response.Headers[accessControlExposeKey] = string.Join(',',
            StringValues.Concat(aches, new StringValues(new[] {accessTokenKey, xAccessTokenKey})).Distinct());

        return true;
    }

    /// <summary>
    /// 验证 Token
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public (bool IsValid, JsonWebToken Token, TokenValidationResult validationResult) Validate(string accessToken)
    {
        if (_jwtSettings == null)
            return (false, default, default);

        // 加密Key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.IssuerSigningKey));
        var cress = new SigningCredentials(key, _jwtSettings?.Algorithm?.ToString() ?? SecurityAlgorithms.HmacSha256);

        // 创建Token验证参数
        var tokenValidationParameters = CreateTokenValidationParameters(_jwtSettings);
        tokenValidationParameters.IssuerSigningKey ??= cress.Key;

        // 验证 Token
        var tokenHandler = new JsonWebTokenHandler();
        try
        {
#if NET8_0
            // 处理 .NET8 中 ValidateToken 方法已过时的警告
            var tokenValidationResult = tokenHandler.ValidateTokenAsync(accessToken, tokenValidationParameters).Result;
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
            return (false, default, default);
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
    public bool ValidateJwtBearerToken(DefaultHttpContext httpContext, out JsonWebToken token, string headerKey = "Authorization",
        string tokenPrefix = "Bearer ")
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
    public JsonWebToken ReadJwtToken(string accessToken)
    {
        var tokenHandler = new JsonWebTokenHandler();
        if (tokenHandler.CanReadToken(accessToken))
        {
            return tokenHandler.ReadJsonWebToken(accessToken);
        }

        return default;
    }

    /// <summary>
    /// 读取 Token，不含验证
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public JwtSecurityToken SecurityReadJwtToken(string accessToken)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(accessToken);
        return jwtSecurityToken;
    }

    /// <summary>
    /// 获取 JWT Bearer Token
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="headerKey"></param>
    /// <param name="tokenPrefix"></param>
    /// <returns></returns>
    public string GetJwtBearerToken(HttpContext httpContext, string headerKey = "Authorization", string tokenPrefix = "Bearer ")
    {
        // 判断请求报文头中是否有 "Authorization" 报文头
        var bearerToken = httpContext.Request.Headers[headerKey].ToString();
        if (string.IsNullOrWhiteSpace(bearerToken))
            return default;

        var prefixLength = tokenPrefix.Length;
        return bearerToken.StartsWith(tokenPrefix, true, null) && bearerToken.Length > prefixLength
            ? bearerToken[prefixLength..]
            : default;
    }

    /// <summary>
    /// 生成Token验证参数
    /// </summary>
    /// <param name="jwtSettings"></param>
    /// <returns></returns>
    public TokenValidationParameters CreateTokenValidationParameters(JWTSettingsOptions jwtSettings)
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
            ClockSkew = TimeSpan.FromSeconds(jwtSettings.ClockSkew ?? 5),
        };
    }
}