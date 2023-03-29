using System.Text;
using System.Web;
using Fast.Core.Restful.Internal;
using Fast.Core.Util.Json.Extension;
using Furion.Logging;
using Microsoft.AspNetCore.Http;

namespace Fast.Core.Middleware;

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
            case "DELETE":
                if (context.Request.QueryString.HasValue)
                {
                    var queryParam = context.Request.QueryString.Value;

                    // ReSharper disable once PossibleNullReferenceException
                    var queryParamDic = queryParam.TrimStart('?').Split('&').Select(sl => sl.Split('='))
                        .ToDictionary(keyArr => keyArr[0].ToLower(), keyArr => HttpUtility.UrlDecode(keyArr[1]));

                    if (queryParamDic.TryGetValue(nameof(XnRestfulResult<string>.Data).ToLower(), out var queryDataStr) &&
                        queryParamDic.TryGetValue(nameof(XnRestfulResult<string>.Timestamp).ToLower(), out var queryTimestamp))
                    {
                        // AES解密
                        var decryptData = AESUtil.AESDecrypt(queryDataStr, $"Fast.NET.XnRestful.{queryTimestamp}",
                            $"FIV{queryTimestamp}");

                        // 替换请求参数
                        context.Request.QueryString =
                            new QueryString(
                                $"?{string.Join("&", decryptData.ToObject<Dictionary<string, object>>().Select(sl => $"{sl.Key}={sl.Value}"))}");

                        // 写日志文件
                        Log.Debug($"HTTP Request AES 解密详情：\r\n\t密文：{queryDataStr}\r\n\t数据源：{decryptData}");
                    }
                }

                break;
            case "POST":
            case "PUT":
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