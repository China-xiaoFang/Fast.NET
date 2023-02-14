using System.Text;
using Fast.Core.Util.Json.Extension;
using Fast.Core.Util.Restful.Internal;
using Furion.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fast.Core.Internal.Filter;

/// <summary>
/// AES解密过滤器
/// </summary>
public class AESDecryptActionFilter : IAsyncActionFilter
{
    /// <summary>
    /// AES解密
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpRequest = context.HttpContext.Request;

        // 允许读取请求的Body
        httpRequest.EnableBuffering();

        // 重置指针
        httpRequest.Body.Position = 0;

        switch (httpRequest.Method)
        {
            case "GET":
                break;
            case "POST":
            case "PUT":
            case "DELETE":
                if (httpRequest.Path.HasValue)
                {
                    // 请求参数
                    using var streamReader = new StreamReader(httpRequest.Body, Encoding.UTF8);
                    var requestParam = await streamReader.ReadToEndAsync();

                    // JSON序列化
                    var encryptParameter = requestParam.ToObject<XnRestfulResult<string>>();

                    if (!encryptParameter.Timestamp.IsNullOrZero() && !encryptParameter.Data.IsEmpty())
                    {
                        // AES解密
                        var decryptData = AESUtil.AESDecrypt(encryptParameter.Data,
                            $"Fast.NET.XnRestful.{encryptParameter.Timestamp}", $"FIV{encryptParameter.Timestamp}");

                        // 写入解密参数
                        var memoryStream = new MemoryStream();
                        var streamWriter = new StreamWriter(memoryStream);
                        await streamWriter.WriteAsync(decryptData);
                        await streamWriter.FlushAsync();

                        // 写入
                        httpRequest.Body = memoryStream;

                        // 重置指针
                        httpRequest.Body.Position = 0;

                        // 写日志文件
                        Log.Debug($"HTTP Request AES 解密详情：\r\n\t密文：{encryptParameter.Data}\r\n\t数据源：{decryptData}");
                    }
                }

                break;
        }

        // 抛给下一个过滤器
        await next();
    }
}