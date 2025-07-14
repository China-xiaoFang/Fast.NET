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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Fast.UnifyResult;

/// <summary>
/// <see cref="RestfulResultProvider"/> 规范化RESTful风格返回值
/// </summary>
internal class RestfulResultProvider : IUnifyResultProvider
{
    /// <summary>
    /// 异常返回值
    /// </summary>
    /// <param name="context"><see cref="ExceptionContext"/></param>
    /// <param name="metadata"><see cref="ExceptionMetadata"/> 异常元数据</param>
    /// <param name="statusCode"><see cref="int"/> 更改的状态码</param>
    /// <param name="message"><see cref="string"/> 返回的错误消息</param>
    /// <returns><see cref="IActionResult"/></returns>
    public IActionResult OnException(ExceptionContext context, ExceptionMetadata metadata, int? statusCode = null,
        string message = null)
    {
        // 判断是否为生产环境，避免安全起见，生产环境不返回错误对象
        var hostEnvironment = context.HttpContext.RequestServices.GetService<IHostEnvironment>();

        // 如果获取到的为空，或者非开发环境，则不返回错误对象
        if (hostEnvironment == null || !hostEnvironment.IsDevelopment())
        {
            return new JsonResult(UnifyContext.GetRestfulResult(statusCode ?? metadata.StatusCode, false, null,
                message ?? context.Exception.Message, context.HttpContext)) {StatusCode = statusCode ?? metadata.StatusCode};
        }

        return new JsonResult(UnifyContext.GetRestfulResult(statusCode ?? metadata.StatusCode, false, context.Exception,
            message ?? context.Exception.Message, context.HttpContext)) {StatusCode = statusCode ?? metadata.StatusCode};
    }

    /// <summary>
    /// 成功返回值
    /// </summary>
    /// <param name="context"><see cref="ActionExecutedContext"/></param>
    /// <param name="data"></param>
    /// <returns><see cref="IActionResult"/></returns>
    public IActionResult OnSucceeded(ActionExecutedContext context, object data)
    {
        return new JsonResult(UnifyContext.GetRestfulResult(
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
        string message;
        // 一般为Model验证失败返回的结果
        if (metadata.ValidationResult is Dictionary<string, string[]> messageObj)
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
            message = metadata.ValidationResult.ToString();
        }

        return new JsonResult(UnifyContext.GetRestfulResult(StatusCodes.Status400BadRequest, false, null, message,
            context.HttpContext)) {StatusCode = StatusCodes.Status400BadRequest};
    }

    /// <summary>
    /// 拦截返回状态码
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="statusCode"><see cref="int"/> 状态码</param>
    /// <returns></returns>
    public async Task OnResponseStatusCodes(HttpContext httpContext, int statusCode)
    {
        var jsonSerializerOptions = httpContext.RequestServices.GetService<IOptions<JsonOptions>>()
            .Value.JsonSerializerOptions;

        // 设置响应状态码
        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(UnifyContext.HandleRestfulStatusCode(statusCode, null, null, httpContext),
            jsonSerializerOptions);
    }
}