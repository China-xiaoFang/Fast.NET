using Fast.Core.Restful.Internal;
using Furion.DataValidation;
using Furion.DependencyInjection;
using Furion.FriendlyException;
using Furion.UnifyResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GetXnRestfulResult = Fast.Core.Restful.Extension.Extension;

namespace Fast.Core.Restful;

/// <summary>
/// 规范化RESTful风格返回值
/// </summary>
[SuppressSniffer, UnifyModel(typeof(XnRestfulResult<>))]
public class XnRestfulResultProvider : IUnifyResultProvider
{
    /// <summary>
    /// 异常返回值
    /// </summary>
    /// <param name="context"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public IActionResult OnException(ExceptionContext context, ExceptionMetadata metadata)
    {
        // 异常拦截，主要是为了处理特殊情况下使用 throw new Exception 抛出的业务异常
        var exceptionType = context.Exception.GetType();

        // SqlSugar 更新提交覆盖异常
        if (exceptionType == typeof(VersionExceptions))
        {
            return new JsonResult(GetXnRestfulResult.GetXnRestfulResult(StatusCodes.Status400BadRequest, false, metadata.Errors,
                "当前编辑的数据不是最新版本！"));
        }

        return new JsonResult(GetXnRestfulResult.GetXnRestfulResult(metadata.StatusCode, false, metadata.Errors,
            "系统内部错误，请联系管理员处理！"));
    }

    /// <summary>
    /// 成功返回值
    /// </summary>
    /// <param name="context"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public IActionResult OnSucceeded(ActionExecutedContext context, object data)
    {
        return new JsonResult(GetXnRestfulResult.GetXnRestfulResult(
            // 处理没有返回值情况 204
            context.Result is EmptyResult ? StatusCodes.Status204NoContent : StatusCodes.Status200OK, true, data, "请求成功"));
    }

    /// <summary>
    /// 验证失败返回值
    /// </summary>
    /// <param name="context"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public IActionResult OnValidateFailed(ActionExecutingContext context, ValidationMetadata metadata)
    {
        return new JsonResult(GetXnRestfulResult.GetXnRestfulResult(StatusCodes.Status400BadRequest, false, null,
            metadata.ValidationResult));
    }

    /// <summary>
    /// 处理输出状态码
    /// </summary>
    /// <param name="context"></param>
    /// <param name="statusCode"></param>
    /// <param name="unifyResultSettings"></param>
    /// <returns></returns>
    public async Task OnResponseStatusCodes(HttpContext context, int statusCode, UnifyResultSettingsOptions unifyResultSettings)
    {
        // 设置响应状态码
        UnifyContext.SetResponseStatusCodes(context, statusCode, unifyResultSettings);

        switch (statusCode)
        {
            // 处理 401 状态码
            case StatusCodes.Status401Unauthorized:
                await context.Response.WriteAsJsonAsync(
                    GetXnRestfulResult.GetXnRestfulResult(StatusCodes.Status401Unauthorized, false, null, "401 未经授权"),
                    App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                break;
            // 处理 403 状态码
            case StatusCodes.Status403Forbidden:
                await context.Response.WriteAsJsonAsync(
                    GetXnRestfulResult.GetXnRestfulResult(StatusCodes.Status403Forbidden, false, null, "403 演示环境，禁止操作！"),
                    App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                break;
            // 处理 429 状态码
            case StatusCodes.Status429TooManyRequests:
                await context.Response.WriteAsJsonAsync(
                    GetXnRestfulResult.GetXnRestfulResult(StatusCodes.Status429TooManyRequests, false, null, "429 频繁请求"),
                    App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                break;
        }
    }
}