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