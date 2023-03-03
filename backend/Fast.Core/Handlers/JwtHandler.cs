using System.Security.Claims;
using Fast.Core.Const;
using Fast.Core.Operation.Config;
using Furion.Authorization;
using Furion.DataEncryption;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Fast.Core.Handlers;

public class JwtHandler : AppAuthorizeHandler
{
    /// <summary>
    /// 自动刷新 Token 信息
    /// </summary>
    /// <param name="context"></param>
    /// <param name="httpContext"></param>
    /// <param name="tokenPrefix">Token 前缀</param>
    /// <param name="clockSkew"></param>
    /// <returns></returns>
    public static bool AutoRefreshToken(AuthorizationHandlerContext context, DefaultHttpContext httpContext,
        string tokenPrefix = "Bearer ", long clockSkew = 5L)
    {
        if (context.User.Identity!.IsAuthenticated)
        {
            return true;
        }

        if (httpContext.GetEndpoint()?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null)
        {
            return true;
        }

        var jwtBearerToken = JWTEncryption.GetJwtBearerToken(httpContext, ClaimConst.AuthAccessToken, tokenPrefix);
        var jwtBearerRefreshToken = JWTEncryption.GetJwtBearerToken(httpContext, ClaimConst.AuthRefreshToken, tokenPrefix);
        if (string.IsNullOrWhiteSpace(jwtBearerToken) || string.IsNullOrWhiteSpace(jwtBearerRefreshToken))
        {
            return false;
        }

        // 新 Token 过期时间（分钟）
        var expiredTime = (ConfigOperation.Tenant.GetConfig(ConfigConst.Tenant.TokenExpiredTime)).Value.ParseToInt();

        var refreshToken = JWTEncryption.Exchange(jwtBearerToken, jwtBearerRefreshToken, expiredTime, clockSkew);
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return false;
        }

        var enumerable = JWTEncryption.ReadJwtToken(refreshToken)?.Claims;
        if (enumerable == null)
        {
            return false;
        }

        var claimsIdentity = new ClaimsIdentity("AuthenticationTypes.Federation");
        claimsIdentity.AddClaims(enumerable);
        var principal = (httpContext.User = new ClaimsPrincipal(claimsIdentity));
        httpContext.SignInAsync(principal);
        const string key = "Access-Control-Expose-Headers";
        // 新刷新 Token 有效期（分钟）
        var refreshTokenExpiredTime = ConfigOperation.Tenant.GetConfig(ConfigConst.Tenant.RefreshTokenExpiredTime).Value
            .ParseToInt();
        httpContext.Response.Headers[ClaimConst.AccessToken] = refreshToken;
        httpContext.Response.Headers[ClaimConst.RefreshToken] =
            JWTEncryption.GenerateRefreshToken(refreshToken, refreshTokenExpiredTime);
        httpContext.Response.Headers.TryGetValue(key, out var value);
        httpContext.Response.Headers[key] = string.Join(',',
            StringValues.Concat(value, new StringValues(new[] {ClaimConst.AccessToken, ClaimConst.RefreshToken})).Distinct());
        return true;
    }

    /// <summary>
    /// 重写 Handler 添加自动刷新
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task HandleAsync(AuthorizationHandlerContext context)
    {
        // 自动刷新Token
        if (AutoRefreshToken(context, context.GetCurrentHttpContext()))
            await AuthorizeHandleAsync(context);
        else
            context.Fail(); // 授权失败
    }

    /// <summary>
    /// 授权判断逻辑，授权通过返回 true，否则返回 false
    /// </summary>
    /// <param name="context"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public override async Task<bool> PipelineAsync(AuthorizationHandlerContext context, DefaultHttpContext httpContext)
    {
        // 此处已经自动验证 Jwt Token的有效性了，无需手动验证
        return await CheckAuthorizationAsync(httpContext);
    }

    /// <summary>
    /// 检查权限
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    private static async Task<bool> CheckAuthorizationAsync(DefaultHttpContext httpContext)
    {
        // 超级管理员跳过判断
        if (GlobalContext.IsSuperAdmin)
            return true;

        return true;
    }
}