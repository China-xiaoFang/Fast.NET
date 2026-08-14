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

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fast.UnifyResult;

/// <summary>
/// 规范化响应数据提供器
/// </summary>
public interface IUnifyResponseProvider
{
    /// <summary>
    /// 响应异常处理
    /// </summary>
    /// <param name="context">当前异常处理上下文</param>
    /// <param name="metadata">异常响应使用的元数据</param>
    /// <param name="httpContext">当前请求上下文</param>
    /// <returns>表示异步响应异常处理的任务，任务结果为响应异常处理</returns>
    Task<(int statusCode, string message)> ResponseExceptionAsync(ExceptionContext context, ExceptionMetadata metadata,
        HttpContext httpContext);

    /// <summary>
    /// 响应数据验证异常处理
    /// </summary>
    /// <param name="context">当前操作上下文</param>
    /// <param name="metadata">验证失败响应使用的元数据</param>
    /// <param name="httpContext">当前请求上下文</param>
    /// <returns>表示异步“响应数据验证异常处理”操作的任务</returns>
    Task ResponseValidationExceptionAsync(ActionExecutingContext context, ValidationMetadata metadata, HttpContext httpContext);

    /// <summary>
    /// 响应数据处理
    /// </summary>
    /// <remarks>只有响应成功且为正常返回才会调用</remarks>
    /// <param name="timestamp">时间戳</param>
    /// <param name="data">要处理或传输的数据</param>
    /// <param name="httpContext">当前请求上下文</param>
    /// <returns>表示异步响应数据处理的任务，任务结果为响应数据处理</returns>
    Task<object> ResponseDataAsync(long timestamp, object data, HttpContext httpContext);
}
