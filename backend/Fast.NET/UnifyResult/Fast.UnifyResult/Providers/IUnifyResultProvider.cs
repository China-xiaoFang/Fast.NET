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

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fast.UnifyResult.Providers;

/// <summary>
/// <see cref="IUnifyResultProvider"/> 规范化结果提供器
/// </summary>
public interface IUnifyResultProvider
{
    /// <summary>
    /// 异常返回值
    /// </summary>
    /// <param name="context"><see cref="ExceptionContext"/></param>
    /// <param name="statusCode"></param>
    /// <returns><see cref="IActionResult"/></returns>
    IActionResult OnException(ExceptionContext context, int statusCode);

    /// <summary>
    /// 成功返回值
    /// </summary>
    /// <param name="context"><see cref="ActionExecutedContext"/></param>
    /// <param name="data"></param>
    /// <returns><see cref="IActionResult"/></returns>
    IActionResult OnSucceeded(ActionExecutedContext context, object data);

    /// <summary>
    /// 验证失败返回值
    /// </summary>
    /// <param name="context"><see cref="ActionExecutingContext"/></param>
    /// <param name="message"></param>
    /// <returns><see cref="IActionResult"/></returns>
    IActionResult OnValidateFailed(ActionExecutingContext context, object message);

    /// <summary>
    /// 拦截返回状态码
    /// </summary>
    /// <param name="context"><see cref="HttpContext"/></param>
    /// <param name="statusCode"></param>
    /// <returns></returns>
    Task OnResponseStatusCodes(HttpContext context, int statusCode);
}