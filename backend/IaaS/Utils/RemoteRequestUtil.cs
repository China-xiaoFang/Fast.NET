// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
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

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="HttpClient"/> 远程请求工具类
/// </summary>
[SuppressSniffer]
public static class RemoteRequestUtil
{
    /// <summary>
    /// Http Get 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="param"><see cref="object"/> Url拼接的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒</param>
    /// <returns></returns>
    public static (T result, HttpResponseHeaders headers) Get<T>(string url, object param = null,
        IDictionary<string, string> headers = null, int timeout = 60) where T : class
    {
        return SendAsync<T>(HttpMethod.Get, url, param, null, headers, timeout).Result;
    }

    /// <summary>
    /// Http Get 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="param"><see cref="object"/> Url拼接的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒</param>
    /// <returns></returns>
    public static Task<(T result, HttpResponseHeaders headers)> GetAsync<T>(string url, object param = null,
        IDictionary<string, string> headers = null, int timeout = 60) where T : class
    {
        return SendAsync<T>(HttpMethod.Get, url, param, null, headers, timeout);
    }

    /// <summary>
    /// Http Get 请求
    /// </summary>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="param"><see cref="object"/> Url拼接的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒</param>
    /// <returns></returns>
    public static (string result, HttpResponseHeaders headers) Get(string url, object param = null,
        IDictionary<string, string> headers = null, int timeout = 60)
    {
        return SendAsync(HttpMethod.Get, url, param, null, headers, timeout).Result;
    }

    /// <summary>
    /// Http Get 请求
    /// </summary>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="param"><see cref="object"/> Url拼接的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒</param>
    /// <returns></returns>
    public static Task<(string result, HttpResponseHeaders headers)> GetAsync(string url, object param = null,
        IDictionary<string, string> headers = null, int timeout = 60)
    {
        return SendAsync(HttpMethod.Get, url, param, null, headers, timeout);
    }

    /// <summary>
    /// Http Post 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="data"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒</param>
    /// <returns></returns>
    public static (T result, HttpResponseHeaders headers) Post<T>(string url, object data,
        IDictionary<string, string> headers = null, int timeout = 60) where T : class
    {
        return SendAsync<T>(HttpMethod.Post, url, null, data, headers, timeout).Result;
    }

    /// <summary>
    /// Http Post 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="data"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒</param>
    /// <returns></returns>
    public static Task<(T result, HttpResponseHeaders headers)> PostAsync<T>(string url, object data,
        IDictionary<string, string> headers = null, int timeout = 60) where T : class
    {
        return SendAsync<T>(HttpMethod.Post, url, null, data, headers, timeout);
    }

    /// <summary>
    /// Http Post 请求
    /// </summary>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="data"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒</param>
    /// <returns></returns>
    public static (string result, HttpResponseHeaders headers) Post(string url, object data,
        IDictionary<string, string> headers = null, int timeout = 60)
    {
        return SendAsync(HttpMethod.Post, url, null, data, headers, timeout).Result;
    }

    /// <summary>
    /// Http Post 请求
    /// </summary>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="data"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒</param>
    /// <returns></returns>
    public static Task<(string result, HttpResponseHeaders headers)> PostAsync(string url, object data,
        IDictionary<string, string> headers = null, int timeout = 60)
    {
        return SendAsync(HttpMethod.Post, url, null, data, headers, timeout);
    }

    /// <summary>
    /// Http Put 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="data"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒</param>
    /// <returns></returns>
    public static (T result, HttpResponseHeaders headers) Put<T>(string url, object data,
        IDictionary<string, string> headers = null, int timeout = 60) where T : class
    {
        return SendAsync<T>(HttpMethod.Put, url, null, data, headers, timeout).Result;
    }

    /// <summary>
    /// Http Put 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="data"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒</param>
    /// <returns></returns>
    public static Task<(T result, HttpResponseHeaders headers)> PutAsync<T>(string url, object data,
        IDictionary<string, string> headers = null, int timeout = 60) where T : class
    {
        return SendAsync<T>(HttpMethod.Put, url, null, data, headers, timeout);
    }

    /// <summary>
    /// Http Put 请求
    /// </summary>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="data"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒</param>
    /// <returns></returns>
    public static (string result, HttpResponseHeaders headers) Put(string url, object data,
        IDictionary<string, string> headers = null, int timeout = 60)
    {
        return SendAsync(HttpMethod.Put, url, null, data, headers, timeout).Result;
    }

    /// <summary>
    /// Http Put 请求
    /// </summary>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="data"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒</param>
    /// <returns></returns>
    public static Task<(string result, HttpResponseHeaders headers)> PutAsync(string url, object data,
        IDictionary<string, string> headers = null, int timeout = 60)
    {
        return SendAsync(HttpMethod.Put, url, null, data, headers, timeout);
    }

    /// <summary>
    /// Http Delete 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒</param>
    /// <returns></returns>
    public static (T result, HttpResponseHeaders headers) Delete<T>(string url, IDictionary<string, string> headers = null,
        int timeout = 60) where T : class
    {
        return SendAsync<T>(HttpMethod.Delete, url, null, null, headers, timeout).Result;
    }

    /// <summary>
    /// Http Delete 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒</param>
    /// <returns></returns>
    public static Task<(T result, HttpResponseHeaders headers)> DeleteAsync<T>(string url,
        IDictionary<string, string> headers = null, int timeout = 60) where T : class
    {
        return SendAsync<T>(HttpMethod.Delete, url, null, null, headers, timeout);
    }

    /// <summary>
    /// Http Delete 请求
    /// </summary>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒</param>
    /// <returns></returns>
    public static (string result, HttpResponseHeaders headers) Delete(string url, IDictionary<string, string> headers = null,
        int timeout = 60)
    {
        return SendAsync(HttpMethod.Delete, url, null, null, headers, timeout).Result;
    }

    /// <summary>
    /// Http Delete 请求
    /// </summary>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒</param>
    /// <returns></returns>
    public static Task<(string result, HttpResponseHeaders headers)> DeleteAsync(string url,
        IDictionary<string, string> headers = null, int timeout = 60)
    {
        return SendAsync(HttpMethod.Delete, url, null, null, headers, timeout);
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
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒</param>
    /// <returns></returns>
    private static async Task<(T result, HttpResponseHeaders headers)> SendAsync<T>(HttpMethod httpMethod, string url,
        object urlParam = null, object bodyData = null, IDictionary<string, string> headers = null, int timeout = 60)
    {
        var (responseContent, responseHeaders) = await SendAsync(httpMethod, url, urlParam, bodyData, headers, timeout);

        return (JsonSerializer.Deserialize<T>(responseContent), responseHeaders);
    }

    /// <summary>
    /// 发送请求
    /// </summary>
    /// <param name="httpMethod"><see cref="HttpMethod"/> 请求方式</param>
    /// <param name="url"><see cref="string"/> 请求的Url</param>
    /// <param name="urlParam"><see cref="object"/> Url拼接的参数</param>
    /// <param name="bodyData"><see cref="object"/> 写入请求Body中的参数</param>
    /// <param name="headers"><see cref="IDictionary{TKey,TValue}"/> 请求头部信息</param>
    /// <param name="timeout"><see cref="int"/> 请求超时时间，默认60秒</param>
    /// <returns></returns>
    private static async Task<(string result, HttpResponseHeaders headers)> SendAsync(HttpMethod httpMethod, string url,
        object urlParam = null, object bodyData = null, IDictionary<string, string> headers = null, int timeout = 60)
    {
        headers ??= new Dictionary<string, string>();

        // 发送 Http 请求
        using var httpClient = new HttpClient(new HttpClientHandler
        {
            // 自动处理各种响应解压缩
            AutomaticDecompression = DecompressionMethods.All
        });

        // 设置请求超时时间
        httpClient.Timeout = TimeSpan.FromSeconds(timeout);

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
                    // 转义
                    query[Uri.EscapeDataString(param.Key)] = Uri.EscapeDataString(param.Value.ToString());
                    //query[param.Key] = param.Value.ToString();
                }
            }
            else if (urlParam is IDictionary<string, string> paramStrDic)
            {
                foreach (var param in paramStrDic)
                {
                    // 转义
                    query[Uri.EscapeDataString(param.Key)] = Uri.EscapeDataString(param.Value);
                    //query[param.Key] = param.Value
                }
            }
            else
            {
                foreach (var param in urlParam.ToDictionary())
                {
                    // 转义
                    query[Uri.EscapeDataString(param.Key)] = Uri.EscapeDataString(param.Value.ToString());
                    //query[param.Key] = param.Value.ToString();
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
                var reqBodyDataJson = JsonSerializer.Serialize(bodyData);

                var httpContent = new StringContent(reqBodyDataJson);

                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // 写入请求内容
                request.Content = httpContent;
            }
        }

        try
        {
            // 发送请求
            using var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseContent = response.Content.ReadAsByteArrayAsync().Result;
            // 获取 charset 编码
            var encoding = GetCharsetEncoding(response);
            var result = encoding.GetString(responseContent); 
            // 通过指定编码解码
            return (result, response.Headers);
        }
        catch (HttpRequestException ex)
        {
            throw new Exception("远程请求错误：" + ex.Message, ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new Exception("远程请求超时：" + ex.Message, ex);
        }
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
            charset = contentTypes.First().Split(';', StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(u => u.Contains("charset", StringComparison.OrdinalIgnoreCase)) ?? "charset=UTF-8";
        }
        else
        {
            charset = "charset=UTF-8";
        }

        var encoding = charset.Split('=', StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? "UTF-8";
        return Encoding.GetEncoding(encoding);
    }
}