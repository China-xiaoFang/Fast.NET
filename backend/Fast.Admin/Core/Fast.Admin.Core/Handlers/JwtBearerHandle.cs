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

using Fast.JwtBearer.Handlers;
using Microsoft.AspNetCore.Authorization;

namespace Fast.Admin.Core.Handlers;

/// <summary>
/// <see cref="JwtBearerHandle"/> Jwt验证提供器
/// </summary>
public class JwtBearerHandle : IJwtBearerHandle
{
    /// <summary>
    /// 授权处理
    /// <remarks>这里已经判断了 Token 是否有效，并且已经处理了自动刷新 Token。只需要处理其余的逻辑即可。如果返回 false或抛出异常，搭配 AuthorizeFailHandle 则抛出 HttpStatusCode = 401 状态码，否则走默认处理 AuthorizationHandlerContext.Fail() 会返回 HttpStatusCode = 403 状态码</remarks>
    /// </summary>
    /// <param name="context"><see cref="T:Microsoft.AspNetCore.Authorization.AuthorizationHandlerContext" /></param>
    /// <param name="httpContext"><see cref="T:Microsoft.AspNetCore.Http.HttpContext" /></param>
    /// <returns><see cref="T:System.Boolean" /></returns>
    public async Task<bool> AuthorizeHandle(AuthorizationHandlerContext context, HttpContext httpContext)
    {
        return false;
    }

    /// <summary>
    /// 授权失败处理
    /// <remarks>如果返回 null，则走默认处理 AuthorizationHandlerContext.Fail()</remarks>
    /// </summary>
    /// <param name="context"><see cref="T:Microsoft.AspNetCore.Authorization.AuthorizationHandlerContext" /></param>
    /// <param name="httpContext"><see cref="T:Microsoft.AspNetCore.Http.HttpContext" /></param>
    /// <param name="exception"><see cref="T:System.Exception" /></param>
    /// <returns></returns>
    public async Task<object> AuthorizeFailHandle(AuthorizationHandlerContext context, HttpContext httpContext,
        Exception exception)
    {
        return null;
    }

    /// <summary>
    /// 权限判断处理
    /// <remarks>这里只需要判断你的权限逻辑即可，如果返回 false或抛出异常 则抛出 HttpStatusCode = 403 状态码</remarks>
    /// </summary>
    /// <param name="context"><see cref="T:Microsoft.AspNetCore.Authorization.AuthorizationHandlerContext" /></param>
    /// <param name="requirement"><see cref="T:Microsoft.AspNetCore.Authorization.IAuthorizationRequirement" /></param>
    /// <param name="httpContext"><see cref="T:Microsoft.AspNetCore.Http.HttpContext" /></param>
    /// <returns></returns>
    public async Task<bool> PermissionHandle(AuthorizationHandlerContext context, IAuthorizationRequirement requirement,
        HttpContext httpContext)
    {
        return false;
    }

    /// <summary>
    /// 权限判断失败处理
    /// <remarks>如果返回 null，则走默认处理 AuthorizationHandlerContext.Fail()</remarks>
    /// </summary>
    /// <param name="context"><see cref="T:Microsoft.AspNetCore.Authorization.AuthorizationHandlerContext" /></param>
    /// <param name="requirement"><see cref="T:Microsoft.AspNetCore.Authorization.IAuthorizationRequirement" /></param>
    /// <param name="httpContext"><see cref="T:Microsoft.AspNetCore.Http.HttpContext" /></param>
    /// <param name="exception"><see cref="T:System.Exception" /></param>
    /// <returns></returns>
    public async Task<object> PermissionFailHandle(AuthorizationHandlerContext context, IAuthorizationRequirement requirement,
        HttpContext httpContext, Exception exception)
    {
        return null;
    }
}