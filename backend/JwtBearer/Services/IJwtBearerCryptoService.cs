// Apache开源许可证
// 
// 版权所有 © 2018-2024 1.8K仔
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
using Fast.JwtBearer.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Fast.JwtBearer.Services;

/// <summary>
/// <see cref="JwtBearerCryptoService"/> JwtBearer 加密解密服务接口
/// </summary>
public interface IJwtBearerCryptoService
{
    /// <summary>
    /// 生成 Token
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="expiredTime">过期时间（分钟）</param>
    /// <returns></returns>
    string GenerateToken(IDictionary<string, object> payload, long? expiredTime = null);

    /// <summary>
    /// 生成刷新 Token
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    string GenerateRefreshToken(string accessToken);

    /// <summary>
    /// 通过过期Token 和 刷新Token 换取新的 Token
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="expiredToken"></param>
    /// <param name="refreshToken"></param>
    /// <param name="expiredTime">过期时间（分钟）</param>
    /// <param name="clockSkew">刷新token容差值，秒做单位</param>
    /// <returns></returns>
    string Exchange(HttpContext httpContext, string expiredToken, string refreshToken, long? expiredTime = null,
        long? clockSkew = null);

    /// <summary>
    /// 自动刷新 Token 信息
    /// </summary>
    /// <param name="context"></param>
    /// <param name="httpContext"></param>
    /// <param name="expiredTime">新 Token 过期时间（分钟）</param>
    /// <param name="tokenPrefix"></param>
    /// <param name="clockSkew"></param>
    /// <returns></returns>
    bool AutoRefreshToken(AuthorizationHandlerContext context, HttpContext httpContext, long? expiredTime = null,
        string tokenPrefix = "Bearer ", long? clockSkew = null);

    /// <summary>
    /// 验证 Token
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    (bool IsValid, JsonWebToken Token, TokenValidationResult validationResult) Validate(string accessToken);

    /// <summary>
    /// 验证 Token
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="token"></param>
    /// <param name="headerKey"></param>
    /// <param name="tokenPrefix"></param>
    /// <returns></returns>
    bool ValidateJwtBearerToken(DefaultHttpContext httpContext, out JsonWebToken token, string headerKey = "Authorization",
        string tokenPrefix = "Bearer ");

    /// <summary>
    /// 读取 Token，不含验证
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    JsonWebToken ReadJwtToken(string accessToken);

    /// <summary>
    /// 读取 Token，不含验证
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    JwtSecurityToken SecurityReadJwtToken(string accessToken);

    /// <summary>
    /// 获取 JWT Bearer Token
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="headerKey"></param>
    /// <param name="tokenPrefix"></param>
    /// <returns></returns>
    string GetJwtBearerToken(HttpContext httpContext, string headerKey = "Authorization", string tokenPrefix = "Bearer ");

    /// <summary>
    /// 生成Token验证参数
    /// </summary>
    /// <param name="jwtSettings"></param>
    /// <returns></returns>
    TokenValidationParameters CreateTokenValidationParameters(JWTSettingsOptions jwtSettings);
}