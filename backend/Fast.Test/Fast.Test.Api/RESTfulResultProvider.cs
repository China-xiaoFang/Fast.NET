using Fast.UnifyResult.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fast.Test.Api;

/// <summary>
/// RESTful 风格返回值
/// </summary>
public class RESTfulResultProvider : IUnifyResultProvider
{
    /// <summary>
    /// 异常返回值
    /// </summary>
    /// <param name="context"><see cref="ExceptionContext"/></param>
    /// <param name="statusCode"></param>
    /// <returns><see cref="IActionResult"/></returns>
    public IActionResult OnException(ExceptionContext context, int statusCode)
    {
        return null;
    }

    /// <summary>
    /// 成功返回值
    /// </summary>
    /// <param name="context"><see cref="ActionExecutedContext"/></param>
    /// <param name="data"></param>
    /// <returns><see cref="IActionResult"/></returns>
    public IActionResult OnSucceeded(ActionExecutedContext context, object data)
    {
        return null;
    }

    /// <summary>
    /// 验证失败返回值
    /// </summary>
    /// <param name="context"><see cref="ActionExecutingContext"/></param>
    /// <param name="message"></param>
    /// <returns><see cref="IActionResult"/></returns>
    public IActionResult OnValidateFailed(ActionExecutingContext context, object message)
    {
        return null;
    }

    /// <summary>
    /// 拦截返回状态码
    /// </summary>
    /// <param name="context"><see cref="HttpContext"/></param>
    /// <param name="statusCode"></param>
    /// <returns></returns>
    public async Task OnResponseStatusCodes(HttpContext context, int statusCode)
    {
    }
}