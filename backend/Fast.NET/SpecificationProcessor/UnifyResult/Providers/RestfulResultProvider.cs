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

using Fast.IaaS;
using Fast.SpecificationProcessor.DataValidation;
using Fast.SpecificationProcessor.FriendlyException.Contexts;
using Fast.SpecificationProcessor.FriendlyException.Metadatas;
using Fast.SpecificationProcessor.UnifyResult.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fast.SpecificationProcessor.UnifyResult.Providers;

/// <summary>
/// 规范化RESTful风格返回值
/// </summary>
internal class RestfulResultProvider : IUnifyResultProvider
{
    /// <summary>
    /// 异常返回值
    /// </summary>
    /// <param name="context"><see cref="ExceptorContext"/></param>
    /// <param name="metadata"><see cref="ExceptionMetadata"/> 异常元数据</param>
    /// <returns><see cref="IActionResult"/></returns>
    public IActionResult OnException(ExceptionContext context, ExceptionMetadata metadata)
    {
#if DEBUG
        return new JsonResult(GetRestfulResult(metadata.StatusCode, false, context.Exception, context.Exception.Message,
            context.HttpContext));
#else
        // 如果是发布模式，避免安全期间，则不返回错误对象
        return new JsonResult(GetRestfulResult(metadata.StatusCode, false, null, context.Exception.Message,
            context.HttpContext));
#endif
    }

    /// <summary>
    /// 成功返回值
    /// </summary>
    /// <param name="context"><see cref="ActionExecutedContext"/></param>
    /// <param name="data"></param>
    /// <returns><see cref="IActionResult"/></returns>
    public IActionResult OnSucceeded(ActionExecutedContext context, object data)
    {
        return new JsonResult(GetRestfulResult(
            // 处理没有返回值情况 204
            context.Result is EmptyResult ? StatusCodes.Status204NoContent : StatusCodes.Status200OK, true, data, "请求成功",
            context.HttpContext));
    }

    /// <summary>
    /// 验证失败返回值
    /// </summary>
    /// <param name="context"><see cref="ActionExecutingContext"/></param>
    /// <param name="metadata"><see cref="ValidationMetadata"/> 验证信息元数据</param>
    /// <returns><see cref="IActionResult"/></returns>
    public IActionResult OnValidateFailed(ActionExecutingContext context, ValidationMetadata metadata)
    {
        return new JsonResult(GetRestfulResult(StatusCodes.Status400BadRequest, false, null, metadata.ValidationResult,
            context.HttpContext));
    }

    /// <summary>
    /// 拦截返回状态码
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="statusCode"><see cref="int"/> 状态码</param>
    /// <returns></returns>
    public async Task OnResponseStatusCodes(HttpContext httpContext, int statusCode)
    {
        var jsonSerializerOptions = httpContext.RequestServices.GetService<IOptions<JsonOptions>>().Value?.JsonSerializerOptions;

        // 设置响应状态码
        httpContext.Response.StatusCode = statusCode;

        switch (statusCode)
        {
            // 处理 400 状态码
            case StatusCodes.Status400BadRequest:
                await httpContext.Response.WriteAsJsonAsync(
                    GetRestfulResult(StatusCodes.Status400BadRequest, false, null, "400 请求无效", httpContext),
                    jsonSerializerOptions);
                break;
            // 处理 401 状态码
            case StatusCodes.Status401Unauthorized:
                await httpContext.Response.WriteAsJsonAsync(
                    GetRestfulResult(StatusCodes.Status401Unauthorized, false, null, "401 未经授权", httpContext),
                    jsonSerializerOptions);
                break;
            // 处理 403 状态码
            case StatusCodes.Status403Forbidden:
                await httpContext.Response.WriteAsJsonAsync(
                    GetRestfulResult(StatusCodes.Status403Forbidden, false, null, "403 无操作权限", httpContext),
                    jsonSerializerOptions);
                break;
            // 处理 404 状态码
            case StatusCodes.Status404NotFound:
                await httpContext.Response.WriteAsJsonAsync(
                    GetRestfulResult(StatusCodes.Status404NotFound, false, null, "404 无效的地址", httpContext),
                    jsonSerializerOptions);
                break;
            // 处理 405 状态码
            case StatusCodes.Status405MethodNotAllowed:
                await httpContext.Response.WriteAsJsonAsync(
                    GetRestfulResult(StatusCodes.Status404NotFound, false, null, "405 方法不被允许", httpContext),
                    jsonSerializerOptions);
                break;
            // 处理 429 状态码
            case StatusCodes.Status429TooManyRequests:
                await httpContext.Response.WriteAsJsonAsync(
                    GetRestfulResult(StatusCodes.Status429TooManyRequests, false, null, "429 频繁请求", httpContext),
                    jsonSerializerOptions);
                break;
            // 处理 500 状态码
            case StatusCodes.Status500InternalServerError:
                await httpContext.Response.WriteAsJsonAsync(
                    GetRestfulResult(StatusCodes.Status500InternalServerError, false, null, "500 服务器内部错误", httpContext),
                    jsonSerializerOptions);
                break;
            // 处理 502 状态码
            case StatusCodes.Status502BadGateway:
                await httpContext.Response.WriteAsJsonAsync(
                    GetRestfulResult(StatusCodes.Status500InternalServerError, false, null, "502 网关错误", httpContext),
                    jsonSerializerOptions);
                break;
            // 处理 503 状态码
            case StatusCodes.Status503ServiceUnavailable:
                await httpContext.Response.WriteAsJsonAsync(
                    GetRestfulResult(StatusCodes.Status500InternalServerError, false, null, "503 服务不可用", httpContext),
                    jsonSerializerOptions);
                break;
        }
    }

    /// <summary>
    /// 获取规范化RESTful风格返回值
    /// </summary>
    /// <param name="code"><see cref="int"/> 状态码</param>
    /// <param name="success"><see cref="bool"/> 执行成功</param>
    /// <param name="data"><see cref="object"/> 数据</param>
    /// <param name="message"><see cref="string"/> 错误信息</param>
    /// <param name="httpContext"><see cref="HttpContext"/> 请求上下文</param>
    /// <returns></returns>
    private static RestfulResult<object> GetRestfulResult(int code, bool success, object data, object message,
        HttpContext httpContext)
    {
        // 从请求响应头部中获取时间戳
        var timestamp = httpContext.UnifyResponseTimestamp();

        // 一般为Model验证失败返回的结果
        if (message is Dictionary<string, string[]> messageObj)
        {
            var newMessage = "";
            foreach (var dicVal in messageObj.SelectMany(dicItem => dicItem.Value))
            {
                newMessage += $"{dicVal}\r\n";
            }

            message = newMessage.Remove(newMessage.LastIndexOf("\r\n", StringComparison.Ordinal));
        }
        else
        {
            message = message.ToString();
        }

        return new RestfulResult<object>
        {
            Code = code,
            Success = success,
            Data = data,
            Message = message,
            Timestamp = timestamp
        };
    }
}