using Fast.NET.Core.Attributes;
using Fast.SpecificationProcessor.DataValidation;
using Fast.SpecificationProcessor.FriendlyException.Metadatas;
using Fast.SpecificationProcessor.UnifyResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fast.Test.Api;

/// <summary>
/// RESTful 风格返回值
/// </summary>
[SuppressSniffer, UnifyModel(typeof(RestfulResult<>))]
public class RESTfulResultProvider : IUnifyResultProvider
{
    /// <summary>
    /// 异常返回值
    /// </summary>
    /// <param name="context"><see cref="ExceptionContext"/></param>
    /// <param name="metadata"><see cref="ExceptionMetadata"/> 异常元数据</param>
    /// <returns><see cref="IActionResult"/></returns>
    public IActionResult OnException(ExceptionContext context, ExceptionMetadata metadata)
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
    /// 拦截返回状态码
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="statusCode"><see cref="int"/> 状态码</param>
    /// <returns></returns>
    public async Task OnResponseStatusCodes(HttpContext httpContext, int statusCode)
    {
    }

    /// <summary>
    /// 验证失败返回值
    /// </summary>
    /// <param name="context"><see cref="ActionExecutingContext"/></param>
    /// <param name="metadata"><see cref="ValidationMetadata"/> 验证信息元数据</param>
    /// <returns><see cref="IActionResult"/></returns>
    public IActionResult OnValidateFailed(ActionExecutingContext context, ValidationMetadata metadata)
    {
        return null;
    }
}