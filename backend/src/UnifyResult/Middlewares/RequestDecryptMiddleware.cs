// Apache开源许可证
//
// 版权所有 © 2018-Now 小方
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

using System.Text;
using System.Text.Json;
using Fast.IaaS;
using Fast.UnifyResult.Inputs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fast.UnifyResult.Middlewares;

/// <summary>
/// <see cref="RequestDecryptMiddleware"/> 请求解密中间件
/// </summary>
internal class RequestDecryptMiddleware
{
    private readonly RequestDelegate _next;

    public RequestDecryptMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 判断是否为WebStock请求
        if (!context.IsWebSocketRequest())
        {
            // 获取回调服务
            var requestCipherHandler = context.RequestServices.GetService<IRequestCipherHandler>();

            // 获取 System.Text.Json 全局配置
            var jsonSerializerOptions = context.RequestServices.GetService<IOptions<JsonOptions>>().Value?.JsonSerializerOptions;

            try
            {
                // 允许读取请求的Body
                context.Request.EnableBuffering();

                // 判断请求方式
                var requestMethod = context.GetRequestMethod();

                // 密文数据（请求源数据）
                RequestDecryptInput ciphertextData = null;
                // 明文数据（解密后的数据）
                IDictionary<string, object> plaintextData = null;

                // 根据不同的请求方式解密
                switch (requestMethod)
                {
                    // Url的传参方式
                    case HttpRequestMethodEnum.Get:
                    case HttpRequestMethodEnum.Delete:
                    case HttpRequestMethodEnum.Head:
                    {
                        // 判断请求参数是否存在值
                        if (context.Request.Query.Count > 0)
                        {
                            // 判断必须同时存在 "data" 和 "timestamp" 参数才能解密
                            if (context.Request.Query.TryGetValue(nameof(RequestDecryptInput.Data).ToLower(),
                                    out var queryData) &&
                                context.Request.Query.TryGetValue(nameof(RequestDecryptInput.Timestamp).ToLower(),
                                    out var queryTimestamp))
                            {
                                ciphertextData = new RequestDecryptInput
                                {
                                    Data = queryData.ToString(), Timestamp = long.Parse(queryTimestamp.ToString())
                                };

                                // 使用 AES 解密
                                var decryptData = CryptoUtil.AESDecrypt(ciphertextData.Data, ciphertextData.Timestamp.ToString(),
                                    $"FIV{ciphertextData.Timestamp}");

                                // 转换密文数据
                                plaintextData =
                                    JsonSerializer.Deserialize<Dictionary<string, object>>(decryptData, jsonSerializerOptions);

                                // 替换请求参数
                                context.Request.QueryString =
                                    new QueryString($"?{string.Join("&", plaintextData.Select(sl => $"{sl.Key}={sl.Value}"))}");
                            }
                        }
                    }
                        break;
                    // Form Body的传参方式
                    case HttpRequestMethodEnum.Post:
                    case HttpRequestMethodEnum.Put:
                    case HttpRequestMethodEnum.Patch:
                    {
                        // 获取请求参数
                        using var streamReader = new StreamReader(context.Request.Body, Encoding.UTF8);
                        var requestParam = await streamReader.ReadToEndAsync();

                        // 判断请求参数是否为空
                        if (!requestParam.IsEmpty())
                        {
                            // JSON序列化请求参数
                            ciphertextData = JsonSerializer.Deserialize<RequestDecryptInput>(requestParam, jsonSerializerOptions);

                            // 使用 AES 解密
                            var decryptData = CryptoUtil.AESDecrypt(ciphertextData.Data, ciphertextData.Timestamp.ToString(),
                                $"FIV{ciphertextData.Timestamp}");

                            // 转换密文数据
                            plaintextData =
                                JsonSerializer.Deserialize<Dictionary<string, object>>(decryptData, jsonSerializerOptions);

                            // 写入解密后的参数
                            var memoryStream = new MemoryStream();
                            var streamWriter = new StreamWriter(memoryStream);
                            await streamWriter.WriteAsync(decryptData);
                            await streamWriter.FlushAsync();
                            context.Request.Body = memoryStream;
                            // 重置指针
                            context.Request.Body.Position = 0;
                        }
                    }
                        break;
                    // 不支持的请求方式
                    case HttpRequestMethodEnum.Options:
                    case HttpRequestMethodEnum.Connect:
                    case HttpRequestMethodEnum.Trace:
                    default:
                        throw new HttpRequestException("This request mode is not supported.");
                }

                // 判断密文数据和回调服务是否为空
                if (ciphertextData != null && requestCipherHandler != null)
                {
                    // 回调请求解密
                    await requestCipherHandler.DecipherAsync(context, requestMethod, plaintextData, ciphertextData);
                }
            }
            catch (Exception ex)
            {
                if (requestCipherHandler != null)
                {
                    // 反馈解密异常
                    await requestCipherHandler.DecipherExceptionAsync(context, ex);
                }

                // 抛出异常
                throw;
            }
        }

        // 继续执行下一个中间件
        await _next(context);
    }
}