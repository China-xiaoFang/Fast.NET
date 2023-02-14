using Fast.Core.Util.Json.Extension;
using Fast.Core.Util.Restful.Internal;
using Furion.DataEncryption;
using Furion.DataValidation;
using Furion.DependencyInjection;
using Furion.FriendlyException;
using Furion.Localization;
using Furion.Logging;
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
    /// 获取规范化RESTful风格AES加密的返回值
    /// </summary>
    /// <param name="code"></param>
    /// <param name="success"></param>
    /// <param name="data"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    private XnRestfulResult<object> GetXnRestfulResult(int code, bool success, object data, object message)
    {
        // 时间
        var time = DateTimeOffset.UtcNow;

        // 时间戳
        var timestamp = time.ToUnixTimeMilliseconds();

        var resultData = data;

        // 判断是否成功
        if (success && data != null)
        {
            if (GlobalContext.ServiceCollectionOptions.RequestAESDecrypt)
            {
                var dataJsonStr = data.ToJsonString();
                resultData = AESUtil.AESEncrypt(dataJsonStr, $"Fast.NET.XnRestful.{timestamp}", $"FIV{timestamp}");

                // 写日志文件
                Log.Debug($"HTTP Request AES 加密详情：\r\n\t源数据：{dataJsonStr}\r\n\tAES加密：{resultData}");
            }
        }

        return new XnRestfulResult<object>
        {
            Code = code,
            Success = success,
            Data = resultData,
            Message = message,
            Extras = UnifyContext.Take(),
            Time = time.LocalDateTime,
            Timestamp = timestamp
        };
    }

    /// <summary>
    /// 异常返回值
    /// </summary>
    /// <param name="context"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public IActionResult OnException(ExceptionContext context, ExceptionMetadata metadata)
    {
        return new JsonResult(GetXnRestfulResult(metadata.StatusCode, false, metadata.Errors, L.Text["系统内部错误，请联系管理员处理！"].Value));
    }

    /// <summary>
    /// 成功返回值
    /// </summary>
    /// <param name="context"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public IActionResult OnSucceeded(ActionExecutedContext context, object data)
    {
        return new JsonResult(GetXnRestfulResult(
            // 处理没有返回值情况 204
            context.Result is EmptyResult ? StatusCodes.Status204NoContent : StatusCodes.Status200OK, true, data,
            L.Text["请求成功"].Value));
    }

    /// <summary>
    /// 验证失败返回值
    /// </summary>
    /// <param name="context"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public IActionResult OnValidateFailed(ActionExecutingContext context, ValidationMetadata metadata)
    {
        return new JsonResult(GetXnRestfulResult(StatusCodes.Status400BadRequest, false, null, metadata.ValidationResult));
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
                    GetXnRestfulResult(StatusCodes.Status429TooManyRequests, false, null, L.Text["429 频繁请求"].Value),
                    App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                break;
            // 处理 401 状态码
            case StatusCodes.Status401Unauthorized:
                await context.Response.WriteAsJsonAsync(
                    GetXnRestfulResult(StatusCodes.Status401Unauthorized, false, null, L.Text["401 未经授权"].Value),
                    App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                break;
            // 处理 403 状态码
            case StatusCodes.Status403Forbidden:
                await context.Response.WriteAsJsonAsync(
                    GetXnRestfulResult(StatusCodes.Status403Forbidden, false, null, L.Text["403 禁止访问"].Value),
                    App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                break;
        }
    }
}