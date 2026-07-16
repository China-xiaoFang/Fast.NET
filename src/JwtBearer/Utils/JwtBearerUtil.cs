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
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;


namespace Fast.JwtBearer;

/// <summary>
/// <see cref="JwtBearerUtil"/> JwtBearer 工具类
/// </summary>
public static class JwtBearerUtil
{
    /// <summary>
    /// JWT 载荷序列化配置。
    /// </summary>
    private static readonly JsonSerializerOptions _payloadSerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

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
        ArgumentNullException.ThrowIfNull(jwtSettings);
        if (string.IsNullOrWhiteSpace(jwtSettings.IssuerSigningKey))
            throw new InvalidOperationException("JWT 签名密钥不能为空。");

        var algorithm = jwtSettings.Algorithm?.ToString() ?? SecurityAlgorithms.HmacSha256;
        return new TokenValidationParameters
        {
            // 验证签发方密钥
            ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey ?? true,
            // 签发方密钥
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.IssuerSigningKey)),
            // 验证签发方
            ValidateIssuer = jwtSettings.ValidateIssuer ?? true,
            // 设置签发方
            ValidIssuer = jwtSettings.ValidIssuer,
            // 验证签收方
            ValidateAudience = jwtSettings.ValidateAudience ?? true,
            // 设置接收方
            ValidAudience = jwtSettings.ValidAudience,
            // 验证生存期
            ValidateLifetime = jwtSettings.ValidateLifetime ?? true,
            // 过期时间容错值
            ClockSkew = TimeSpan.FromSeconds(jwtSettings.ClockSkew ?? 5),
            // 只接受配置的算法，避免同一密钥被其他 JWT 算法复用。
            ValidAlgorithms = new[] {algorithm},
            RequireSignedTokens = true,
            RequireExpirationTime = true
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
        ArgumentNullException.ThrowIfNull(payload);
        var jwtSettings = Penetrates.JWTSettings ?? throw new InvalidOperationException("JWT 尚未配置，请先注册 JWTSettings。");
        if (string.IsNullOrWhiteSpace(jwtSettings.IssuerSigningKey))
            throw new InvalidOperationException("JWT 签名密钥不能为空，禁止生成未签名 Token。");

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
            var minute = expiredTime ?? jwtSettings.TokenExpiredTime ?? 20;
            payload.Add(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(minute)
                .ToUnixTimeSeconds());
        }

        if (!payload.ContainsKey(JwtRegisteredClaimNames.Iss))
        {
            payload.Add(JwtRegisteredClaimNames.Iss, jwtSettings.ValidIssuer);
        }

        if (!payload.ContainsKey(JwtRegisteredClaimNames.Aud))
        {
            payload.Add(JwtRegisteredClaimNames.Aud, jwtSettings.ValidAudience);
        }

        // 处理 JwtPayload 序列化不一致问题
        var stringPayload = payload is JwtPayload jwtPayload
            ? jwtPayload.SerializeToJson()
            : JsonSerializer.Serialize(payload, _payloadSerializerOptions);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.IssuerSigningKey));
        var credentials = new SigningCredentials(securityKey, jwtSettings.Algorithm?.ToString() ?? SecurityAlgorithms.HmacSha256);

        var tokenHandler = new JsonWebTokenHandler();
        return tokenHandler.CreateToken(stringPayload, credentials);
    }

    /// <summary>
    /// 生成刷新 Token
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public static string GenerateRefreshToken(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new ArgumentException("Access Token 不能为空。", nameof(accessToken));

        // 分割Token
        var tokenParagraphs = accessToken.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (tokenParagraphs.Length != 3)
            throw new ArgumentException("Access Token 不是有效的 JWT 格式。", nameof(accessToken));

        var payloadLength = tokenParagraphs[1].Length;
        if (payloadLength == 0)
            throw new ArgumentException("Access Token 的载荷不能为空。", nameof(accessToken));

        var maxLength = Math.Min(12, payloadLength);
        var minLength = Math.Min(3, maxLength);
        var l = minLength == maxLength ? minLength : RandomNumberGenerator.GetInt32(minLength, maxLength + 1);
        var maxStart = payloadLength - l;
        var s = maxStart == 0 ? 0 : RandomNumberGenerator.GetInt32(maxStart + 1);

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
        ArgumentNullException.ThrowIfNull(httpContext);
        if (string.IsNullOrWhiteSpace(headerKey))
            throw new ArgumentException("Token 请求头名称不能为空。", nameof(headerKey));
        if (string.IsNullOrEmpty(tokenPrefix))
            throw new ArgumentException("Token 前缀不能为空。", nameof(tokenPrefix));

        // 判断请求报文头中是否有 "Authorization" 报文头
        var bearerToken = httpContext.Request.Headers[headerKey]
            .ToString();
        if (string.IsNullOrWhiteSpace(bearerToken))
            return null;

        var prefixLength = tokenPrefix.Length;
        return bearerToken.StartsWith(tokenPrefix, StringComparison.OrdinalIgnoreCase) && bearerToken.Length > prefixLength
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
        return ValidateAsync(accessToken)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// 异步验证 Token。
    /// </summary>
    /// <param name="accessToken">待验证的 Token。</param>
    /// <returns>验证结果。</returns>
    public static async Task<(bool IsValid, JsonWebToken Token, TokenValidationResult validationResult)> ValidateAsync(
        string accessToken)
    {
        if (Penetrates.JWTSettings == null || string.IsNullOrWhiteSpace(accessToken))
            return (false, null, null);

        try
        {
            var validationParameters = CreateTokenValidationParameters(Penetrates.JWTSettings);
            return await ValidateAsync(accessToken, validationParameters)
                .ConfigureAwait(false);
        }
        catch (Exception)
        {
            // 保持工具方法原有契约：格式、配置或签名错误均通过 IsValid=false 返回。
            return (false, null, null);
        }
    }

    /// <summary>
    /// 使用指定参数验证 Token。
    /// </summary>
    private static async Task<(bool IsValid, JsonWebToken Token, TokenValidationResult validationResult)> ValidateAsync(
        string accessToken, TokenValidationParameters validationParameters)
    {
        var tokenHandler = new JsonWebTokenHandler();
        var tokenValidationResult = await ValidateTokenAsync(tokenHandler, accessToken, validationParameters)
            .ConfigureAwait(false);
        if (!tokenValidationResult.IsValid)
            return (false, null, tokenValidationResult);

        var jsonWebToken = tokenValidationResult.SecurityToken as JsonWebToken;
        return (jsonWebToken != null, jsonWebToken, tokenValidationResult);
    }

    /// <summary>
    /// 兼容不同 IdentityModel 版本的 Token 验证 API。
    /// </summary>
    private static Task<TokenValidationResult> ValidateTokenAsync(JsonWebTokenHandler tokenHandler, string accessToken,
        TokenValidationParameters validationParameters)
    {
#if NET8_0_OR_GREATER
        return tokenHandler.ValidateTokenAsync(accessToken, validationParameters);
#else
        return Task.FromResult(tokenHandler.ValidateToken(accessToken, validationParameters));
#endif
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
    /// <remarks>仅解析令牌，不会验证签名、签发方或有效期；安全决策请使用 <see cref="Validate"/>。</remarks>
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
        return ExchangeAsync(httpContext, expiredToken, refreshToken, expiredTime, clockSkew)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// 异步使用过期 Token 和刷新 Token 换取新的 Token。
    /// </summary>
    public static async Task<string> ExchangeAsync(HttpContext httpContext, string expiredToken, string refreshToken,
        long? expiredTime = null, long? clockSkew = null)
    {
        if (Penetrates.JWTSettings == null
            || httpContext == null
            || string.IsNullOrWhiteSpace(expiredToken)
            || string.IsNullOrWhiteSpace(refreshToken))
            return null;

        // 原访问令牌必须签名、签发方和接收方均有效，仅在此处临时忽略生命周期。
        // 不能直接把普通 Validate=false 当作“已过期”，否则被篡改或伪造的令牌也会进入刷新流程。
        var expiredValidationParameters = CreateTokenValidationParameters(Penetrates.JWTSettings);
        expiredValidationParameters.ValidateLifetime = false;
        var (isExpiredTokenAuthentic, expiredTokenObj, _) = await ValidateAsync(expiredToken, expiredValidationParameters)
            .ConfigureAwait(false);
        if (!isExpiredTokenAuthentic
            || !expiredTokenObj.TryGetPayloadValue<long>(JwtRegisteredClaimNames.Exp, out var expiredAt)
            || DateTimeOffset.FromUnixTimeSeconds(expiredAt) > DateTimeOffset.UtcNow)
            return null;

        var (isRefreshTokenValid, refreshTokenObj, _) = await ValidateAsync(refreshToken)
            .ConfigureAwait(false);
        if (!isRefreshTokenValid)
            return null;

        if (!refreshTokenObj.TryGetPayloadValue<int>("s", out var start)
            || !refreshTokenObj.TryGetPayloadValue<int>("l", out var length)
            || !refreshTokenObj.TryGetPayloadValue<string>("f", out var refreshHeader)
            || !refreshTokenObj.TryGetPayloadValue<string>("e", out var refreshSignature)
            || !refreshTokenObj.TryGetPayloadValue<string>("k", out var refreshPayloadFragment))
            return null;

        var blacklistRefreshKey = "BLACKLIST_REFRESH_TOKEN:" + refreshToken;
        var distributedCache = httpContext?.RequestServices.GetService<IDistributedCache>();

        var nowTime = DateTimeOffset.UtcNow;
        var cachedValue = distributedCache == null
            ? null
            : await distributedCache.GetStringAsync(blacklistRefreshKey)
                .ConfigureAwait(false);
        var isRefresh = !string.IsNullOrWhiteSpace(cachedValue);
        if (isRefresh)
        {
            if (!long.TryParse(cachedValue, out var refreshTicks))
                return null;

            var refreshTime = new DateTimeOffset(refreshTicks, TimeSpan.Zero);
            if ((nowTime - refreshTime).TotalSeconds > (clockSkew ?? Penetrates.JWTSettings.ClockSkew ?? 5))
                return null;
        }

        var tokenParagraphs = expiredToken.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (tokenParagraphs.Length != 3)
            return null;

        if (start < 0 || length < 0 || start > tokenParagraphs[1].Length - length)
            return null;

        if (!string.Equals(refreshHeader, tokenParagraphs[0], StringComparison.Ordinal)
            || !string.Equals(refreshSignature, tokenParagraphs[2], StringComparison.Ordinal)
            || !string.Equals(tokenParagraphs[1]
                .Substring(start, length), refreshPayloadFragment, StringComparison.Ordinal))
            return null;

        // 上面已完成签名验证，此时才可以读取原令牌载荷并签发新令牌。
        var payload = SecurityReadJwtToken(expiredToken)
            .Payload;
        foreach (var innerKey in DateTypeClaimTypes)
        {
            payload.Remove(innerKey);
        }

        if (!isRefresh && distributedCache != null)
        {
            await distributedCache.SetStringAsync(blacklistRefreshKey, nowTime.Ticks.ToString(),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTimeOffset.FromUnixTimeSeconds(
                            refreshTokenObj.GetPayloadValue<long>(JwtRegisteredClaimNames.Exp))
                    })
                .ConfigureAwait(false);
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
        SetExpiredTokenAsync(httpContext, expiredToken)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// 异步标记失效 Token。
    /// </summary>
    public static async Task SetExpiredTokenAsync(HttpContext httpContext, string expiredToken)
    {
        if (string.IsNullOrEmpty(expiredToken))
            return;

        // 标记过期 必须原Token 是有效的
        var (_isValid, accessTokenObj, _) = await ValidateAsync(expiredToken)
            .ConfigureAwait(false);
        if (!_isValid)
            return;

        var nowTime = DateTimeOffset.UtcNow;
        var blacklistAccessKey = "BLACKLIST_ACCESS_TOKEN:" + expiredToken;
        var distributedCache = httpContext?.RequestServices.GetService<IDistributedCache>();

        // 标记失效
        if (distributedCache != null)
        {
            await distributedCache.SetStringAsync(blacklistAccessKey, nowTime.Ticks.ToString(),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTimeOffset.FromUnixTimeSeconds(
                            accessTokenObj.GetPayloadValue<long>(JwtRegisteredClaimNames.Exp))
                    })
                .ConfigureAwait(false);
        }
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
        return AutoRefreshTokenAsync(context, httpContext, expiredTime, tokenPrefix, clockSkew)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// 异步自动刷新 Token 信息。
    /// </summary>
    public static async Task<bool> AutoRefreshTokenAsync(AuthorizationHandlerContext context, HttpContext httpContext,
        long? expiredTime = null, string tokenPrefix = "Bearer ", long? clockSkew = null)
    {
        if (context == null || httpContext == null)
            return false;

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

                var cachedValue = distributedCache == null
                    ? null
                    : await distributedCache.GetStringAsync(blacklistAccessKey)
                        .ConfigureAwait(false);
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
        var newAccessToken = await ExchangeAsync(httpContext, expiredToken, refreshToken, expiredTime, clockSkew)
            .ConfigureAwait(false);
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
        await httpContext.SignInAsync(claimsPrincipal)
            .ConfigureAwait(false);

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