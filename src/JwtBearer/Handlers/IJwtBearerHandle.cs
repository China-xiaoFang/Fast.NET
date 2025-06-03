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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

// ReSharper disable once CheckNamespace
namespace Fast.JwtBearer;

/// <summary>
/// <see cref="IJwtBearerHandle"/> Jwt验证提供器
/// </summary>
[SuppressSniffer]
public interface IJwtBearerHandle
{
    /// <summary>
    /// 授权处理
    /// </summary>
    /// <remarks>这里已经判断了 Token 是否有效，并且已经处理了自动刷新 Token。只需要处理其余的逻辑即可。如果返回 false或抛出异常，搭配 AuthorizeFailHandle 则抛出 HttpStatusCode = 401 状态码，否则走默认处理 AuthorizationHandlerContext.Fail() 会返回 HttpStatusCode = 403 状态码</remarks>
    /// <param name="context"><see cref="AuthorizationHandlerContext"/></param>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="bool"/></returns>
    Task<bool> AuthorizeHandle(AuthorizationHandlerContext context, HttpContext httpContext);

    /// <summary>
    /// 授权失败处理
    /// </summary>
    /// <remarks>如果返回 null，则走默认处理 AuthorizationHandlerContext.Fail()</remarks>
    /// <param name="context"><see cref="AuthorizationHandlerContext"/></param>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="exception"><see cref="Exception"/></param>
    /// <returns></returns>
    Task<object> AuthorizeFailHandle(AuthorizationHandlerContext context, HttpContext httpContext, Exception exception);

    /// <summary>
    /// 权限判断处理
    /// </summary>
    /// <remarks>这里只需要判断你的权限逻辑即可，如果返回 false或抛出异常 则抛出 HttpStatusCode = 403 状态码</remarks>
    /// <param name="context"><see cref="AuthorizationHandlerContext"/></param>
    /// <param name="requirement"><see cref="IAuthorizationRequirement"/></param>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns></returns>
    Task<bool> PermissionHandle(AuthorizationHandlerContext context, IAuthorizationRequirement requirement,
        HttpContext httpContext);

    /// <summary>
    /// 权限判断失败处理
    /// </summary>
    /// <remarks>如果返回 null，则走默认处理 AuthorizationHandlerContext.Fail()</remarks>
    /// <param name="context"><see cref="AuthorizationHandlerContext"/></param>
    /// <param name="requirement"><see cref="IAuthorizationRequirement"/></param>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="exception"><see cref="Exception"/></param>
    /// <returns></returns>
    Task<object> PermissionFailHandle(AuthorizationHandlerContext context, IAuthorizationRequirement requirement,
        HttpContext httpContext, Exception exception);
}