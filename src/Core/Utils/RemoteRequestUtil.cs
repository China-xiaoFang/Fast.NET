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

using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

// ReSharper disable once CheckNamespace
namespace Fast.NET.Core;

/// <summary>
/// <see cref="HttpClient"/> 远程请求工具类
/// </summary>
[SuppressSniffer]
public static class RemoteRequestUtil
{
    /// <summary>
    /// 默认 System.Text.Json 序列化配置
    /// </summary>
    private static readonly JsonSerializerOptions _defaultJsonSerializerOptions = new()
    {
        // 忽略只有在 .NET 6 才会存在的循环引用问题
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        // 解决 JSON 乱码问题
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        // 允许尾随逗号
        AllowTrailingCommas = true,
        // 忽略注释
        ReadCommentHandling = JsonCommentHandling.Skip,
        // 允许数字带引号
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        // 默认不区分大小写匹配
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// 得到每日一句
    /// </summary>
    /// <returns></returns>
    public static async Task<DaySentenceInfo> GetDaySentence()
    {
        using var client = new HttpClient();
        try
        {
            var response = await client.GetAsync("https://open.iciba.com/dsapi/");
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DaySentenceInfo>(responseBody);
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Day sentence request error，{ex.Message}");
        }
    }

    /// <summary>
    /// Http Get 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="param"><see cref="object"/> Url拼接的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="paramEncode"><see cref="bool"/> Url参数是否进行URL编码，默认 false，null则不超时</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒，null则不超时</param>
    /// <returns></returns>
    public static (T result, HttpResponseHeaders headers) Get<T>(string url, object param = null,
        IDictionary<string, string> headers = null, bool paramEncode = false, int? timeout = 60) where T : class
    {
        return SendAsync<T>(HttpMethod.Get, url, param, null, headers, paramEncode, timeout)
            .Result;
    }

    /// <summary>
    /// Http Get 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="param"><see cref="object"/> Url拼接的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="paramEncode"><see cref="bool"/> Url参数是否进行URL编码，默认 false，null则不超时</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒，null则不超时</param>
    /// <returns></returns>
    public static Task<(T result, HttpResponseHeaders headers)> GetAsync<T>(string url, object param = null,
        IDictionary<string, string> headers = null, bool paramEncode = false, int? timeout = 60) where T : class
    {
        return SendAsync<T>(HttpMethod.Get, url, param, null, headers, paramEncode, timeout);
    }

    /// <summary>
    /// Http Get 请求
    /// </summary>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="param"><see cref="object"/> Url拼接的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="paramEncode"><see cref="bool"/> Url参数是否进行URL编码，默认 false，null则不超时</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒，null则不超时</param>
    /// <returns></returns>
    public static (string result, HttpResponseHeaders headers) Get(string url, object param = null,
        IDictionary<string, string> headers = null, bool paramEncode = false, int? timeout = 60)
    {
        return SendAsync(HttpMethod.Get, url, param, null, headers, paramEncode, timeout)
            .Result;
    }

    /// <summary>
    /// Http Get 请求
    /// </summary>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="param"><see cref="object"/> Url拼接的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="paramEncode"><see cref="bool"/> Url参数是否进行URL编码，默认 false，null则不超时</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒，null则不超时</param>
    /// <returns></returns>
    public static Task<(string result, HttpResponseHeaders headers)> GetAsync(string url, object param = null,
        IDictionary<string, string> headers = null, bool paramEncode = false, int? timeout = 60)
    {
        return SendAsync(HttpMethod.Get, url, param, null, headers, paramEncode, timeout);
    }

    /// <summary>
    /// Http Post 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="data"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="paramEncode"><see cref="bool"/> Url参数是否进行URL编码，默认 false，null则不超时</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒，null则不超时</param>
    /// <returns></returns>
    public static (T result, HttpResponseHeaders headers) Post<T>(string url, object data,
        IDictionary<string, string> headers = null, bool paramEncode = false, int? timeout = 60) where T : class
    {
        return SendAsync<T>(HttpMethod.Post, url, null, data, headers, paramEncode, timeout)
            .Result;
    }

    /// <summary>
    /// Http Post 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="data"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="paramEncode"><see cref="bool"/> Url参数是否进行URL编码，默认 false，null则不超时</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒，null则不超时</param>
    /// <returns></returns>
    public static Task<(T result, HttpResponseHeaders headers)> PostAsync<T>(string url, object data,
        IDictionary<string, string> headers = null, bool paramEncode = false, int? timeout = 60) where T : class
    {
        return SendAsync<T>(HttpMethod.Post, url, null, data, headers, paramEncode, timeout);
    }

    /// <summary>
    /// Http Post 请求
    /// </summary>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="data"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="paramEncode"><see cref="bool"/> Url参数是否进行URL编码，默认 false，null则不超时</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒，null则不超时</param>
    /// <returns></returns>
    public static (string result, HttpResponseHeaders headers) Post(string url, object data,
        IDictionary<string, string> headers = null, bool paramEncode = false, int? timeout = 60)
    {
        return SendAsync(HttpMethod.Post, url, null, data, headers, paramEncode, timeout)
            .Result;
    }

    /// <summary>
    /// Http Post 请求
    /// </summary>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="data"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="paramEncode"><see cref="bool"/> Url参数是否进行URL编码，默认 false，null则不超时</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒，null则不超时</param>
    /// <returns></returns>
    public static Task<(string result, HttpResponseHeaders headers)> PostAsync(string url, object data,
        IDictionary<string, string> headers = null, bool paramEncode = false, int? timeout = 60)
    {
        return SendAsync(HttpMethod.Post, url, null, data, headers, paramEncode, timeout);
    }

    /// <summary>
    /// Http Put 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="data"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="paramEncode"><see cref="bool"/> Url参数是否进行URL编码，默认 false，null则不超时</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒，null则不超时</param>
    /// <returns></returns>
    public static (T result, HttpResponseHeaders headers) Put<T>(string url, object data,
        IDictionary<string, string> headers = null, bool paramEncode = false, int? timeout = 60) where T : class
    {
        return SendAsync<T>(HttpMethod.Put, url, null, data, headers, paramEncode, timeout)
            .Result;
    }

    /// <summary>
    /// Http Put 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="data"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="paramEncode"><see cref="bool"/> Url参数是否进行URL编码，默认 false，null则不超时</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒，null则不超时</param>
    /// <returns></returns>
    public static Task<(T result, HttpResponseHeaders headers)> PutAsync<T>(string url, object data,
        IDictionary<string, string> headers = null, bool paramEncode = false, int? timeout = 60) where T : class
    {
        return SendAsync<T>(HttpMethod.Put, url, null, data, headers, paramEncode, timeout);
    }

    /// <summary>
    /// Http Put 请求
    /// </summary>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="data"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="paramEncode"><see cref="bool"/> Url参数是否进行URL编码，默认 false，null则不超时</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒，null则不超时</param>
    /// <returns></returns>
    public static (string result, HttpResponseHeaders headers) Put(string url, object data,
        IDictionary<string, string> headers = null, bool paramEncode = false, int? timeout = 60)
    {
        return SendAsync(HttpMethod.Put, url, null, data, headers, paramEncode, timeout)
            .Result;
    }

    /// <summary>
    /// Http Put 请求
    /// </summary>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="data"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="paramEncode"><see cref="bool"/> Url参数是否进行URL编码，默认 false，null则不超时</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒，null则不超时</param>
    /// <returns></returns>
    public static Task<(string result, HttpResponseHeaders headers)> PutAsync(string url, object data,
        IDictionary<string, string> headers = null, bool paramEncode = false, int? timeout = 60)
    {
        return SendAsync(HttpMethod.Put, url, null, data, headers, paramEncode, timeout);
    }

    /// <summary>
    /// Http Delete 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="paramEncode"><see cref="bool"/> Url参数是否进行URL编码，默认 false，null则不超时</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒，null则不超时</param>
    /// <returns></returns>
    public static (T result, HttpResponseHeaders headers) Delete<T>(string url, IDictionary<string, string> headers = null,
        bool paramEncode = false, int? timeout = 60) where T : class
    {
        return SendAsync<T>(HttpMethod.Delete, url, null, null, headers, paramEncode, timeout)
            .Result;
    }

    /// <summary>
    /// Http Delete 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="paramEncode"><see cref="bool"/> Url参数是否进行URL编码，默认 false，null则不超时</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒，null则不超时</param>
    /// <returns></returns>
    public static Task<(T result, HttpResponseHeaders headers)> DeleteAsync<T>(string url,
        IDictionary<string, string> headers = null, bool paramEncode = false, int? timeout = 60) where T : class
    {
        return SendAsync<T>(HttpMethod.Delete, url, null, null, headers, paramEncode, timeout);
    }

    /// <summary>
    /// Http Delete 请求
    /// </summary>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="paramEncode"><see cref="bool"/> Url参数是否进行URL编码，默认 false，null则不超时</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒，null则不超时</param>
    /// <returns></returns>
    public static (string result, HttpResponseHeaders headers) Delete(string url, IDictionary<string, string> headers = null,
        bool paramEncode = false, int? timeout = 60)
    {
        return SendAsync(HttpMethod.Delete, url, null, null, headers, paramEncode, timeout)
            .Result;
    }

    /// <summary>
    /// Http Delete 请求
    /// </summary>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="paramEncode"><see cref="bool"/> Url参数是否进行URL编码，默认 false，null则不超时</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒，null则不超时</param>
    /// <returns></returns>
    public static Task<(string result, HttpResponseHeaders headers)> DeleteAsync(string url,
        IDictionary<string, string> headers = null, bool paramEncode = false, int? timeout = 60)
    {
        return SendAsync(HttpMethod.Delete, url, null, null, headers, paramEncode, timeout);
    }

    /// <summary>
    /// 发送请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="httpMethod"><see cref="HttpMethod"/> 请求方式</param>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="urlParam"><see cref="object"/> Url拼接的参数</param>
    /// <param name="bodyData"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="paramEncode"><see cref="bool"/> Url参数是否进行URL编码，默认 false，null则不超时</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒，null则不超时</param>
    /// <returns></returns>
    public static async Task<(T result, HttpResponseHeaders headers)> SendAsync<T>(HttpMethod httpMethod, string url,
        object urlParam = null, object bodyData = null, IDictionary<string, string> headers = null, bool paramEncode = false,
        int? timeout = 60)
    {
        var (responseContent, responseHeaders) =
            await SendAsync(httpMethod, url, urlParam, bodyData, headers, paramEncode, timeout);

        return (JsonSerializer.Deserialize<T>(responseContent, _defaultJsonSerializerOptions), responseHeaders);
    }

    /// <summary>
    /// 发送请求
    /// </summary>
    /// <param name="httpMethod"><see cref="HttpMethod"/> 请求方式</param>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="urlParam"><see cref="object"/> Url拼接的参数</param>
    /// <param name="bodyData"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="paramEncode"><see cref="bool"/> Url参数是否进行URL编码，默认 false，null则不超时</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒，null则不超时</param>
    /// <returns></returns>
    public static async Task<(string result, HttpResponseHeaders headers)> SendAsync(HttpMethod httpMethod, string url,
        object urlParam = null, object bodyData = null, IDictionary<string, string> headers = null, bool paramEncode = false,
        int? timeout = 60)
    {
        headers ??= new Dictionary<string, string>();

        // 发送 Http 请求
        using var httpClient = new HttpClient(new HttpClientHandler
        {
            // 自动处理各种响应解压缩
            AutomaticDecompression = DecompressionMethods.All
        });

        // 设置请求超时时间
        httpClient.Timeout = timeout == null ? Timeout.InfiniteTimeSpan : TimeSpan.FromSeconds(timeout.Value);

        // 处理请求 URL
        var reqUriBuilder = new UriBuilder(url);

        // 处理 Url 本身自带的参数
        var query = HttpUtility.ParseQueryString(reqUriBuilder.Query);

        // 请求参数处理
        if (urlParam != null)
        {
            // 判断是否原本就为字典
            if (urlParam is IDictionary<string, object> paramObjDic)
            {
                foreach (var param in paramObjDic)
                {
                    if (param.Value != null)
                    {
                        // 转义
                        //query[param.Key.UrlEncode()] = param.Value.ToString()
                        //    .UrlEncode();
                        if (paramEncode)
                            query[param.Key] = param.Value.ToString()
                                .UrlEncode();
                        else
                            query[param.Key] = param.Value.ToString();
                    }
                }
            }
            else if (urlParam is IDictionary<string, string> paramStrDic)
            {
                foreach (var param in paramStrDic)
                {
                    if (!string.IsNullOrEmpty(param.Value))
                    {
                        // 转义
                        //query[param.Key.UrlEncode()] = param.Value.UrlEncode();
                        if (paramEncode)
                            query[param.Key] = param.Value.UrlEncode();
                        else
                            query[param.Key] = param.Value;
                    }
                }
            }
            else
            {
                foreach (var param in urlParam.ToDictionary())
                {
                    if (param.Value != null)
                    {
                        // 转义
                        //query[param.Key.UrlEncode()] = param.Value?.ToString()
                        //    .UrlEncode();
                        if (paramEncode)
                            query[param.Key] = param.Value.ToString()
                                .UrlEncode();
                        else
                            query[param.Key] = param.Value.ToString();
                    }
                }
            }
        }

        // 设置Url参数
        reqUriBuilder.Query = query.ToString();

        using var request = new HttpRequestMessage();

        // 设置请求 Url
        request.RequestUri = reqUriBuilder.Uri;
        // 设置请求方式
        request.Method = httpMethod;
        // 设置请求头部
        request.Headers.Add("Accept", "application/json, text/plain, */*");
        request.Headers.Add("Accept-Encoding", "gzip, compress, deflate, br");
        request.Headers.Referrer = reqUriBuilder.Uri;

        // 添加默认 User-Agent
        request.Headers.TryAddWithoutValidation("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.5112.81 Safari/537.36 Edg/104.0.1293.47");

        // 循环添加头部
        foreach (var header in headers)
        {
            // 转义
            //request.Headers.TryAddWithoutValidation(header.Key.UrlEncode(), header.Value.UrlEncode());
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        // Body 参数处理
        if (bodyData != null)
        {
            // 判断是否原本就为字符串
            if (bodyData is string dataStr)
            {
                var httpContent = new StringContent(dataStr);

                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // 写入请求内容
                request.Content = httpContent;
            }
            else
            {
                // 请求数据转为 JSON 字符串
                var reqBodyDataJson = JsonSerializer.Serialize(bodyData, _defaultJsonSerializerOptions);

                var httpContent = new StringContent(reqBodyDataJson);

                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // 写入请求内容
                request.Content = httpContent;
            }
        }

        string responseContent = null;

        try
        {
            // 发送请求
            using var response = await httpClient.SendAsync(request);

            byte[] responseContentBytes;

            // 判断是否启用了响应压缩
            if (response.Content.Headers.ContentEncoding.Contains("br"))
            {
                // Brotli 解压缩
                var responseBytes = await response.Content.ReadAsByteArrayAsync();
                using var compressedStream = new MemoryStream(responseBytes);
                using var decompressedStream = new MemoryStream();
                await using var brotliStream = new BrotliStream(compressedStream, CompressionMode.Decompress);
                await brotliStream.CopyToAsync(decompressedStream);
                responseContentBytes = decompressedStream.ToArray();
            }
            else if (response.Content.Headers.ContentEncoding.Contains("gzip"))
            {
                // Gzip 解压缩
                var responseBytes = await response.Content.ReadAsByteArrayAsync();
                using var compressedStream = new MemoryStream(responseBytes);
                using var decompressedStream = new MemoryStream();
                await using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
                await gzipStream.CopyToAsync(decompressedStream);
                responseContentBytes = decompressedStream.ToArray();
            }
            else if (response.Content.Headers.ContentEncoding.Contains("deflate"))
            {
                // Deflate  解压缩
                var responseBytes = await response.Content.ReadAsByteArrayAsync();
                using var compressedStream = new MemoryStream(responseBytes);
                using var decompressedStream = new MemoryStream();
                await using var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress);
                await deflateStream.CopyToAsync(decompressedStream);
                responseContentBytes = decompressedStream.ToArray();
            }
            else
            {
                responseContentBytes = await response.Content.ReadAsByteArrayAsync();
            }

            // 获取 charset 编码
            var encoding = GetCharsetEncoding(response);
            // 通过指定编码解码
            responseContent = encoding.GetString(responseContentBytes);

            // 优先尝试获取返回的响应对象，再检查请求状态码，因为这里可能存在 RestfulResult 风格的返回。或者直接返回了错误信息
            response.EnsureSuccessStatusCode();

            return (responseContent, response.Headers);
        }
        catch (HttpRequestException ex)
        {
            if (string.IsNullOrEmpty(responseContent))
            {
                throw;
            }

            // 根据 Exception 序列化判断，一般只会序列化 Message，Source，StackTrace，InnerException 这四个参数
            object responseData = null;
            try
            {
                responseData = JsonSerializer.Deserialize<IDictionary<string, object>>(responseContent,
                    _defaultJsonSerializerOptions);
            }
            catch
            {
                // ignored
            }

            if (responseData is IDictionary<string, object> responseDataDictionary)
            {
                if (responseDataDictionary.TryGetValue("Message", out var responseMessage)
                    && responseDataDictionary.TryGetValue("StackTrace", out _))
                {
                    throw new HttpRequestException(responseMessage.ToString(), ex);
                }
            }

            throw new HttpRequestException(responseContent, ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new Exception("远程请求超时：" + ex.Message, ex);
        }
    }

    /// <summary>
    /// 将一个字符串 URL 编码
    /// <para>如果已经 URL 编码则不会继续编码</para>
    /// </summary>
    /// <param name="str"><see cref="string"/></param>
    /// <returns><see cref="string"/></returns>
    private static string UrlEncode(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return "";
        }

        var result = HttpUtility.UrlEncode(str, Encoding.UTF8);

        try
        {
            // 尝试解密，避免再次编码已经是 URL 编码的字符串
            var tryDecode = HttpUtility.UrlDecode(str, Encoding.UTF8);
            // 如果解码后不相同，则直接返回原来的
            return str.Equals(tryDecode, StringComparison.OrdinalIgnoreCase) ? result : str;
        }
        catch
        {
            // 报错了，不管直接编码返回
            return result;
        }
    }

    /// <summary>
    /// 将一个Object对象转为 字典
    /// </summary>
    /// <param name="obj"><see cref="object"/></param>
    /// <param name="includeNull"><see cref="bool"/> 包括 null 值的属性</param>
    /// <returns><see cref="IDictionary{TKey,TValue}"/></returns>
    private static IDictionary<string, object> ToDictionary(this object obj, bool includeNull = false)
    {
        var dictionary = new Dictionary<string, object>();

        var t = obj.GetType(); // 获取对象对应的类， 对应的类型

        var pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance); // 获取当前type公共属性

        foreach (var p in pi)
        {
            var m = p.GetGetMethod();

            if (m == null || !m.IsPublic)
                continue;

            // 进行判NULL处理
            var value = m.Invoke(obj, Array.Empty<object>());
            if (value != null || includeNull)
            {
                dictionary.Add(p.Name, value); // 向字典添加元素
            }
        }

        return dictionary;
    }

    /// <summary>
    /// 获取响应报文 charset 编码
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    private static Encoding GetCharsetEncoding(HttpResponseMessage response)
    {
        if (response == null)
        {
            return Encoding.UTF8;
        }

        // 获取 charset
        string charset;

        var withContentType = response.Content.Headers.TryGetValues("Content-Type", out var contentTypes);
        if (withContentType)
        {
            charset = contentTypes.First()
                          .Split(';', StringSplitOptions.RemoveEmptyEntries)
                          .FirstOrDefault(u => u.Contains("charset", StringComparison.OrdinalIgnoreCase))
                      ?? "charset=UTF-8";
        }
        else
        {
            charset = "charset=UTF-8";
        }

        var encoding = charset.Split('=', StringSplitOptions.RemoveEmptyEntries)
                           .LastOrDefault()
                       ?? "UTF-8";
        return Encoding.GetEncoding(encoding);
    }
}