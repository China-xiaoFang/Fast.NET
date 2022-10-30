using Fast.SqlSugar;
using Furion.Authorization;
using Furion.DataEncryption;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Fast.Core.Handlers;

public class JwtHandler : AppAuthorizeHandler
{
    /// <summary>
    /// 重写 Handler 添加自动刷新
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task HandleAsync(AuthorizationHandlerContext context)
    {
        // 自动刷新Token
        if (JWTEncryption.AutoRefreshToken(context, context.GetCurrentHttpContext()))
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
        if (SugarGlobalContext.IsSuperAdmin)
            return true;

        return true;
    }
}