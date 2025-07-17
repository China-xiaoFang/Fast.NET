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

using Fast.Runtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Fast.JwtBearer;

/// <summary>
/// 授权策略执行程序
/// </summary>
internal class AppAuthorizationHandler : IAuthorizationHandler
{
    /// <summary>Makes a decision if authorization is allowed.</summary>
    /// <param name="context">The authorization information.</param>
    /// <exception cref="ArgumentException"></exception>
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var filterContext = context.Resource as AuthorizationFilterContext;
        var httpContext = filterContext?.HttpContext;
        // 获取 JWT 处理类
        var jwtBearerHandle = httpContext?.RequestServices.GetService<IJwtBearerHandle>();

        // 自动刷新 Token 逻辑
        if (!JwtBearerUtil.AutoRefreshToken(context, httpContext))
        {
            // 退出 Swagger 登录
            httpContext?.SignOutToSwagger();
            await AuthorizeFailHandle(null);
            return;
        }

        // 获取所有未成功验证的需求
        var pendingRequirements = context.PendingRequirements;

        if (jwtBearerHandle == null)
        {
            foreach (var requirement in pendingRequirements)
            {
                context.Succeed(requirement);
            }

            return;
        }

        Exception authorizeException = null;

        bool isAuthorizeSuccess;
        try
        {
            isAuthorizeSuccess = await jwtBearerHandle.AuthorizeHandle(context, httpContext);
        }
        catch (Exception ex)
        {
            isAuthorizeSuccess = false;
            authorizeException = ex;
        }

        // 授权检测
        if (!isAuthorizeSuccess)
        {
            await AuthorizeFailHandle(authorizeException);
            return;
        }

        // 判断是否跳过权限检查
        if (httpContext.GetEndpoint()
                ?.Metadata.GetMetadata<AllowForbiddenAttribute>()
            != null)
        {
            foreach (var requirement in pendingRequirements)
            {
                context.Succeed(requirement);
            }

            return;
        }

        foreach (var requirement in pendingRequirements)
        {
            bool isPermissionSuccess;
            Exception permissionException = null;

            try
            {
                isPermissionSuccess = await jwtBearerHandle.PermissionHandle(context, requirement, httpContext);
            }
            catch (Exception ex)
            {
                isPermissionSuccess = false;
                permissionException = ex;
            }

            // 权限检测
            if (isPermissionSuccess)
            {
                context.Succeed(requirement);
                continue;
            }

            var result = await jwtBearerHandle.PermissionFailHandle(context, requirement, httpContext, permissionException);

            if (result != null)
            {
                // 存在自定义处理结果，则返回 403 状态码
                filterContext.Result = new JsonResult(result) {StatusCode = StatusCodes.Status403Forbidden};
            }
            else
            {
                context.Fail();
            }
        }

        return;

        async Task AuthorizeFailHandle(Exception exception)
        {
            if (jwtBearerHandle == null)
            {
                context.Fail();
                return;
            }

            var result = await jwtBearerHandle.AuthorizeFailHandle(context, httpContext, exception);

            if (filterContext != null && result != null)
            {
                // 存在自定义处理结果，则返回 401 状态码
                filterContext.Result = new JsonResult(result) {StatusCode = StatusCodes.Status401Unauthorized};
            }
            else
            {
                context.Fail();
            }
        }
    }
}