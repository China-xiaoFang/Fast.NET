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

namespace Fast.JwtBearer;

/// <summary>
/// 定义 JWT 身份验证和权限检查的自定义处理契约。
/// </summary>
[SuppressSniffer]
public interface IJwtBearerHandle
{
    /// <summary>
    /// 执行身份验证后的附加授权检查。
    /// </summary>
    /// <remarks>
    /// 调用此方法前，框架已完成令牌验证和自动刷新。返回 <see langword="false"/> 或抛出异常时，
    /// 框架将调用 <see cref="AuthorizeFailHandle"/>；该方法未提供响应时调用
    /// <see cref="AuthorizationHandlerContext.Fail()"/>。
    /// </remarks>
    /// <param name="context">当前 <see cref="AuthorizationHandlerContext"/>。</param>
    /// <param name="httpContext">当前 <see cref="HttpContext"/> 请求上下文。</param>
    /// <returns>授权通过时返回 <see langword="true"/>；返回 <see langword="false"/> 或抛出异常时进入失败处理。</returns>
    Task<bool> AuthorizeHandle(AuthorizationHandlerContext context, HttpContext httpContext);

    /// <summary>
    /// 创建身份验证失败时的自定义响应数据。
    /// </summary>
    /// <remarks>
    /// 返回非 <see langword="null"/> 数据时，框架以 HTTP 401 状态码写入该数据；返回
    /// <see langword="null"/> 时调用 <see cref="AuthorizationHandlerContext.Fail()"/>。
    /// </remarks>
    /// <param name="context">当前 <see cref="AuthorizationHandlerContext"/>。</param>
    /// <param name="httpContext">当前 <see cref="HttpContext"/> 请求上下文。</param>
    /// <param name="exception">身份验证检查抛出的异常；没有捕获到异常时为 <see langword="null"/>。</param>
    /// <returns>自定义响应数据；使用默认失败处理时返回 <see langword="null"/>。</returns>
    Task<object> AuthorizeFailHandle(AuthorizationHandlerContext context, HttpContext httpContext, Exception exception);

    /// <summary>
    /// 判断当前请求是否满足指定授权要求。
    /// </summary>
    /// <remarks>
    /// 返回 <see langword="false"/> 或抛出异常时，框架将调用 <see cref="PermissionFailHandle"/>。
    /// </remarks>
    /// <param name="context">当前 <see cref="AuthorizationHandlerContext"/>。</param>
    /// <param name="requirement">当前待验证的 <see cref="IAuthorizationRequirement"/>。</param>
    /// <param name="httpContext">当前 <see cref="HttpContext"/> 请求上下文。</param>
    /// <returns>权限检查通过时返回 <see langword="true"/>；返回 <see langword="false"/> 或抛出异常时进入失败处理。</returns>
    Task<bool> PermissionHandle(AuthorizationHandlerContext context, IAuthorizationRequirement requirement,
        HttpContext httpContext);

    /// <summary>
    /// 创建权限检查失败时的自定义响应数据。
    /// </summary>
    /// <remarks>
    /// 返回非 <see langword="null"/> 数据时，框架以 HTTP 403 状态码写入该数据；返回
    /// <see langword="null"/> 时调用 <see cref="AuthorizationHandlerContext.Fail()"/>。
    /// </remarks>
    /// <param name="context">当前 <see cref="AuthorizationHandlerContext"/>。</param>
    /// <param name="requirement">验证失败的 <see cref="IAuthorizationRequirement"/>。</param>
    /// <param name="httpContext">当前 <see cref="HttpContext"/> 请求上下文。</param>
    /// <param name="exception">权限检查抛出的异常；没有捕获到异常时为 <see langword="null"/>。</param>
    /// <returns>自定义响应数据；使用默认失败处理时返回 <see langword="null"/>。</returns>
    Task<object> PermissionFailHandle(AuthorizationHandlerContext context, IAuthorizationRequirement requirement,
        HttpContext httpContext, Exception exception);
}
