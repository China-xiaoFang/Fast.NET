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

using Fast.JwtBearer.Utils;
using Fast.NET;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.JwtBearer.Handlers;

/// <summary>
/// 授权策略执行程序
/// </summary>
internal class AppAuthorizationHandler : IAuthorizationHandler
{
    /// <summary>Makes a decision if authorization is allowed.</summary>
    /// <param name="context">The authorization information.</param>
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var filterContext = context.Resource as AuthorizationFilterContext;
        var httpContext = filterContext?.HttpContext;

        // 自动刷新 Token 逻辑
        if (!JwtCryptoUtil.AutoRefreshToken(context, httpContext))
        {
            // 退出 Swagger 登录
            httpContext?.SignOutToSwagger();
            context.Fail();

            return;
        }

        // 获取所有未成功验证的需求
        var pendingRequirements = context.PendingRequirements;

        // 获取 JWT 处理类
        var jwtBearerHandle = httpContext?.RequestServices.GetService<IJwtBearerHandle>();

        if (jwtBearerHandle != null)
        {
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
            if (isAuthorizeSuccess)
            {
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
                    }
                    else
                    {
                        var result = await jwtBearerHandle.PermissionFailHandle(context, requirement, httpContext,
                            permissionException);

                        if (result != null)
                        {
                            // 存在自定义处理结果，则返回 403 状态码
                            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                            filterContext.Result = new JsonResult(result);
                            // 退出 Swagger 登录
                            httpContext?.SignOutToSwagger();
                        }
                        else
                        {
                            // 权限判断失败，返回 403 状态码
                            context.Fail();
                        }
                    }
                }
            }
            else
            {
                var result = await jwtBearerHandle.AuthorizeFailHandle(context, httpContext, authorizeException);

                if (result != null)
                {
                    // 存在自定义处理结果，则返回 401 状态码
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    filterContext.Result = new JsonResult(result);
                }
                else
                {
                    // 授权失败，返回 403 状态码
                    context.Fail();
                }
            }
        }
        else
        {
            foreach (var requirement in pendingRequirements)
            {
                context.Succeed(requirement);
            }
        }
    }
}