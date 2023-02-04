using Fast.Core.Util.Restful.Internal;
using Furion.DataValidation;
using Furion.DependencyInjection;
using Furion.FriendlyException;
using Furion.Localization;
using Furion.UnifyResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fast.Core.Util.Restful;

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
        return new JsonResult(new XnRestfulResult<object>
        {
            Code = metadata.StatusCode,
            Success = false,
            Data = metadata.Errors,
            Message = L.Text["系统内部错误，请联系管理员处理！"].Value,
            Extras = UnifyContext.Take(),
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 成功返回值
    /// </summary>
    /// <param name="context"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public IActionResult OnSucceeded(ActionExecutedContext context, object data)
    {
        return new JsonResult(new XnRestfulResult<object>
        {
            Code = context.Result is EmptyResult ? StatusCodes.Status204NoContent : StatusCodes.Status200OK, // 处理没有返回值情况 204
            Success = true,
            Data = data,
            Message = L.Text["请求成功"].Value,
            Extras = UnifyContext.Take(),
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 验证失败返回值
    /// </summary>
    /// <param name="context"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public IActionResult OnValidateFailed(ActionExecutingContext context, ValidationMetadata metadata)
    {
        return new JsonResult(new XnRestfulResult<object>
        {
            Code = StatusCodes.Status400BadRequest,
            Success = false,
            Data = null,
            Message = metadata.ValidationResult,
            Extras = UnifyContext.Take(),
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
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
            // 处理 429 状态码
            case StatusCodes.Status429TooManyRequests:
                await context.Response.WriteAsJsonAsync(
                    new XnRestfulResult<object>
                    {
                        Code = StatusCodes.Status429TooManyRequests,
                        Success = false,
                        Data = null,
                        Message = L.Text["429 频繁请求"].Value,
                        Extras = UnifyContext.Take(),
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    }, App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                break;
            // 处理 401 状态码
            case StatusCodes.Status401Unauthorized:
                await context.Response.WriteAsJsonAsync(
                    new XnRestfulResult<object>
                    {
                        Code = StatusCodes.Status401Unauthorized,
                        Success = false,
                        Data = null,
                        Message = L.Text["401 未经授权"].Value,
                        Extras = UnifyContext.Take(),
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    }, App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                break;
            // 处理 403 状态码
            case StatusCodes.Status403Forbidden:
                await context.Response.WriteAsJsonAsync(
                    new XnRestfulResult<object>
                    {
                        Code = StatusCodes.Status403Forbidden,
                        Success = false,
                        Data = null,
                        Message = L.Text["403 禁止访问"].Value,
                        Extras = UnifyContext.Take(),
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    }, App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                break;
        }
    }
}