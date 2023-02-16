using System.Text;
using Fast.Core.Util.Json.Extension;
using Fast.Core.Util.Restful.Internal;
using Furion.Logging;
using Microsoft.AspNetCore.Http;

namespace Fast.Core.Internal.Middleware;

/// <summary>
/// AES解密中间件
/// </summary>
public class AESDecryptMiddleware
{
    private readonly RequestDelegate _next;

    public AESDecryptMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 允许读取请求的Body
        context.Request.EnableBuffering();

        switch (context.Request.Method)
        {
            case "GET":
                // 目前Get请求暂时没想到好的实现方式
                break;
            case "POST":
            case "PUT":
            case "DELETE":
                if (context.Request.Path.HasValue)
                {
                    // 请求参数
                    using var streamReader = new StreamReader(context.Request.Body, Encoding.UTF8);
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
                        // 写入
                        await streamWriter.WriteAsync(decryptData);
                        await streamWriter.FlushAsync();
                        context.Request.Body = memoryStream;
                        // 重置指针
                        context.Request.Body.Position = 0;

                        // 写日志文件
                        Log.Debug($"HTTP Request AES 解密详情：\r\n\t密文：{encryptParameter.Data}\r\n\t数据源：{decryptData}");
                    }
                }

                break;
        }

        // 抛给下一个过滤器
        await _next(context);
    }
}