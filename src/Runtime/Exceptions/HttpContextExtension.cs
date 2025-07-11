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

using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Fast.Runtime;

/// <summary>
/// <see cref="HttpContext"/> 拓展类
/// </summary>
[SuppressSniffer]
public static class HttpContextExtension
{
    /// <summary>
    /// 设置规范化响应时间戳
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="timestamp"><see cref="long"/></param>
    /// <returns><see cref="string"/></returns>
    public static void UnifyResponseTimestamp(this HttpContext httpContext, long timestamp)
    {
        httpContext?.Response.Headers.TryAdd(nameof(Fast) + "-NET-Timestamp", $"{timestamp}");
    }

    /// <summary>
    /// 获取规范化响应时间戳
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="string"/></returns>
    public static long UnifyResponseTimestamp(this HttpContext httpContext)
    {
        var timestampStr = httpContext?.Response.Headers[nameof(Fast) + "-NET-Timestamp"];

        if (string.IsNullOrEmpty(timestampStr))
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // 设置请求响应头部时间戳
            httpContext.UnifyResponseTimestamp(timestamp);

            return timestamp;
        }

        return long.Parse(timestampStr);
    }

    /// <summary>
    /// 判断是否是 WebSocket 请求
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsWebSocketRequest(this HttpContext httpContext)
    {
        return httpContext.WebSockets.IsWebSocketRequest || httpContext.Request.Path == "/ws";
    }

    /// <summary>
    /// 获取 Action 特性
    /// </summary>
    /// <typeparam name="TAttribute"></typeparam>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns></returns>
    public static TAttribute GetMetadata<TAttribute>(this HttpContext httpContext) where TAttribute : class
    {
        return httpContext.GetEndpoint()
            ?.Metadata.GetMetadata<TAttribute>();
    }

    /// <summary>
    /// 获取 Action 特性
    /// </summary>
    /// <param name="metadata"><see cref="EndpointMetadataCollection"/></param>
    /// <param name="attributeType"><see cref="Type"/></param>
    /// <returns><see cref="object"/></returns>
    public static object GetMetadata(this EndpointMetadataCollection metadata, Type attributeType)
    {
        return metadata?.GetType()
            .GetMethod(nameof(EndpointMetadataCollection.GetMetadata))
            ?.MakeGenericMethod(attributeType)
            .Invoke(metadata, null);
    }

    /// <summary>
    /// 获取 Action 特性
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="attributeType"><see cref="Type"/></param>
    /// <returns><see cref="object"/></returns>
    public static object GetMetadata(this HttpContext httpContext, Type attributeType)
    {
        return httpContext.GetEndpoint()
            ?.Metadata.GetMetadata(attributeType);
    }

    /// <summary>
    /// 设置规范化文档自动登录
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="accessToken"></param>
    public static void SignInToSwagger(this HttpContext httpContext, string accessToken)
    {
        if (httpContext != null)
        {
            // 设置 Swagger 刷新自动授权
            httpContext.Response.Headers["access-token"] = accessToken;
        }
    }

    /// <summary>
    /// 设置规范化文档退出登录
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    public static void SignOutToSwagger(this HttpContext httpContext)
    {
        if (httpContext != null)
        {
            httpContext.Response.Headers["access-token"] = "invalid_token";
        }
    }

    /// <summary>
    /// 局域网 IPv4 地址
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="string"/></returns>
    public static string LanIpv4(this HttpContext httpContext)
    {
        var remoteIpAddress = httpContext.Connection.RemoteIpAddress;
        if (remoteIpAddress is {AddressFamily: AddressFamily.InterNetwork})
        {
            return remoteIpAddress.ToString();
        }

        // 处理可能获取到的是 IPV6 的地址，且是 localhost，则获取到的为 ::1
        if (remoteIpAddress is {AddressFamily: AddressFamily.InterNetworkV6} && remoteIpAddress.ToString() == "::1")
        {
            return "127.0.0.1";
        }

        return string.Empty;
    }

    /// <summary>
    /// 局域网 IPv6 地址
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="string"/></returns>
    public static string LanIpv6(this HttpContext httpContext)
    {
        var remoteIpAddress = httpContext.Connection.RemoteIpAddress;
        if (remoteIpAddress is {AddressFamily: AddressFamily.InterNetworkV6})
        {
            return remoteIpAddress.ToString();
        }

        return string.Empty;
    }

    /// <summary>
    /// 本机 IPv4 地址
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="string"/></returns>
    public static string LocalIpv4(this HttpContext httpContext)
    {
        var localIpAddress = httpContext.Connection.LocalIpAddress;
        // 处理可能获取到的是 IPV6 的地址，且是 localhost，则获取到的为 ::1
        if (localIpAddress is {AddressFamily: AddressFamily.InterNetworkV6} && localIpAddress.ToString() == "::1")
        {
            return "127.0.0.1";
        }

        return httpContext.Connection.LocalIpAddress?.MapToIPv4()
            .ToString();
    }

    /// <summary>
    /// 本机 IPv6 地址
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="string"/></returns>
    public static string LocalIpv6(this HttpContext httpContext)
    {
        return httpContext.Connection.LocalIpAddress?.MapToIPv6()
            .ToString();
    }

    /// <summary>
    /// 远程 Ipv4 地址
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="string"/></returns>
    public static string RemoteIpv4(this HttpContext httpContext)
    {
        if (httpContext == null)
            return string.Empty;

        var remoteIpv4 = string.Empty;

        // 判断是否为 Nginx 反向代理
        if (httpContext.Request.Headers.TryGetValue("X-Real-IP", out var header1))
        {
            if (IPAddress.TryParse(header1, out var ipv4) && ipv4.AddressFamily == AddressFamily.InterNetwork)
            {
                remoteIpv4 = ipv4.ToString();
            }
        }

        // 判断是否启用了代理并获取代理服务器的IP地址
        if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var header2))
        {
            if (IPAddress.TryParse(header2, out var ipv4) && ipv4.AddressFamily == AddressFamily.InterNetwork)
            {
                remoteIpv4 = ipv4.ToString();
            }
        }

        if (string.IsNullOrEmpty(remoteIpv4))
        {
            var remoteIpAddress = httpContext.Connection.RemoteIpAddress;
            // 处理可能获取到的是 IPV6 的地址，且是 localhost，则获取到的为 ::1
            if (remoteIpAddress is {AddressFamily: AddressFamily.InterNetworkV6} && remoteIpAddress.ToString() == "::1")
            {
                remoteIpv4 = "127.0.0.1";
            }
            else
            {
                remoteIpv4 = remoteIpAddress?.MapToIPv4()
                    .ToString();
            }
        }

        return remoteIpv4;
    }

    /// <summary>
    /// 远程 Ipv6 地址
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="string"/></returns>
    public static string RemoteIpv6(this HttpContext httpContext)
    {
        if (httpContext == null)
            return string.Empty;

        var remoteIpv4 = httpContext.Connection.RemoteIpAddress?.MapToIPv6()
            .ToString();

        // 判断是否为 Nginx 反向代理
        if (httpContext.Request.Headers.TryGetValue("X-Real-IP", out var header1))
        {
            if (IPAddress.TryParse(header1, out var ipv6) && ipv6.AddressFamily == AddressFamily.InterNetworkV6)
            {
                remoteIpv4 = ipv6.ToString();
            }
        }

        // 判断是否启用了代理并获取代理服务器的IP地址
        if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var header2))
        {
            if (IPAddress.TryParse(header2, out var ipv6) && ipv6.AddressFamily == AddressFamily.InterNetworkV6)
            {
                remoteIpv4 = ipv6.ToString();
            }
        }

        return remoteIpv4 ?? string.Empty;
    }

    /// <summary>
    /// 请求用户代理字符串（User-Agent）
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="userAgentHeaderKey">默认从 “User-Agent” 获取</param>
    /// <returns><see cref="string"/></returns>
    public static string RequestUserAgent(this HttpContext httpContext, string userAgentHeaderKey = "User-Agent")
    {
        return httpContext?.Request.Headers[userAgentHeaderKey];
    }

    /// <summary>
    /// 请求用户代理信息（User-Agent）
    /// </summary>
    /// <remarks>注：如果需要正常解析，需要引用 "UAParser" 程序集，否则会返回 null</remarks>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="UserAgentInfo"/></returns>
    public static UserAgentInfo RequestUserAgentInfo(this HttpContext httpContext)
    {
        // 从 HttpContext.Items 中尝试获取缓存数据
        var userAgentObj = httpContext.Items[nameof(Fast) + nameof(UserAgentInfo)];

        // 判断是否为空
        if (userAgentObj != null)
        {
            // 直接返回缓存中的信息
            return userAgentObj as UserAgentInfo;
        }

        // 获取用户代理字符串
        var userAgent = httpContext.RequestUserAgent();

        try
        {
            // 判断是否安装了 UAParser 程序集
            var uaParserAssembly = MAppContext.Assemblies.SingleOrDefault(s => s.GetName()
                                                                                   .Name?.Equals("UAParser")
                                                                               == true);

            if (uaParserAssembly == null)
            {
                return null;
            }

            // 加载 UAParser 的 Parser 类型
            var uaParserParserType = uaParserAssembly.GetType("UAParser.Parser");

            if (uaParserParserType == null)
            {
                return null;
            }

            // 加载 Parser 类型 的 GetDefault() 方法
            var uaParserParserGetDefaultMethod =
                uaParserParserType.GetMethod("GetDefault", BindingFlags.Public | BindingFlags.Static);

            if (uaParserParserGetDefaultMethod == null)
            {
                return null;
            }

            // 调用 Parser 类型 的 GetDefault() 方法
            var parser = uaParserParserGetDefaultMethod.Invoke(null, new object[] {null});

            // 加载 Parser 类型 的 Parse() 方法，这里是 Public | HideBySig，但是我没有找到 HideBySig 所以直接获取吧
            var uaParserParserParseMethod = uaParserParserType.GetMethod("Parse");

            if (uaParserParserParseMethod == null)
            {
                return null;
            }

            // 调用 Parser 类型 的 Parse() 方法，解析用户代理字符串
            dynamic clientInfo = uaParserParserParseMethod.Invoke(parser, new object[] {userAgent});

            if (clientInfo == null)
            {
                return null;
            }

            var result = new UserAgentInfo
            {
                Device = clientInfo.Device.ToString(), OS = clientInfo.OS.ToString(), Browser = clientInfo.UA.ToString()
            };

            // 放入 HttpContext.Items 中
            httpContext.Items[nameof(Fast) + nameof(UserAgentInfo)] = result;

            return result;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 远程 Ipv4 地址信息
    /// </summary>
    /// <remarks>自带内存缓存，缓存过期时间为24小时（注：需要注入内存缓存，如不注入，则默认不走缓存）</remarks>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="ip"><see cref="string"/> 要的IP地址信息，默认为 null，如果为 null，默认获取当前远程的 Ipv4 地址</param>
    /// <returns><see cref="WanNetIPInfo"/></returns>
    public static WanNetIPInfo RemoteIpv4Info(this HttpContext httpContext, string ip = null)
    {
        return httpContext.RemoteIpv4InfoAsync(ip)
            .Result;
    }

    /// <summary>
    /// Ip地址获取并发锁
    /// </summary>
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> ipLockSemaphoreSlims = new();

    /// <summary>
    /// 远程 Ipv4 地址信息
    /// </summary>
    /// <remarks>自带内存缓存，缓存过期时间为24小时（注：需要注入内存缓存，如不注入，则默认不走缓存）</remarks>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="ip"><see cref="string"/> 要的IP地址信息，默认为 null，如果为 null，默认获取当前远程的 Ipv4 地址</param>
    /// <returns><see cref="WanNetIPInfo"/></returns>
    public static async Task<WanNetIPInfo> RemoteIpv4InfoAsync(this HttpContext httpContext, string ip = null)
    {
        // 从 HttpContext.Items 中尝试获取缓存数据
        var wanNetIPInfoObj = httpContext.Items[nameof(Fast) + nameof(WanNetIPInfo)];

        // 判断是否为空
        if (wanNetIPInfoObj != null)
        {
            // 直接返回缓存中的信息
            return wanNetIPInfoObj as WanNetIPInfo;
        }

        // 判断是否传入IP地址
        ip ??= httpContext.RemoteIpv4();

        WanNetIPInfo result;

        // 获取内存缓存服务
        var _memoryCache = httpContext.RequestServices.GetService<IMemoryCache>();

        if (_memoryCache == null)
        {
            result = await GetWanNetInfoAsync(ip);
        }
        else
        {
            var cacheKey = $"{nameof(Fast)}.NET:Http:RemoteIpv4Info:{ip}";

            // 避免并发请求
            var semaphoreSlim = ipLockSemaphoreSlims.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));

            await semaphoreSlim.WaitAsync();
            try
            {
                // 从缓存中读取
                if (!_memoryCache.TryGetValue(cacheKey, out result))
                {
                    result = await GetWanNetInfoAsync(ip);
                    // 放入内存缓存，设置过期时间为24个小时
                    _memoryCache.Set(cacheKey, result, TimeSpan.FromHours(24));
                }
            }
            finally
            {
                semaphoreSlim.Release();
                ipLockSemaphoreSlims.TryRemove(cacheKey, out _);
            }
        }

        // 放入 HttpContext.Items 中
        httpContext.Items[nameof(Fast) + nameof(WanNetIPInfo)] = result;

        return result;
    }

    /// <summary>
    /// 获取远程 Ipv4 地址信息
    /// </summary>
    /// <param name="ip"><see cref="string"/> 要的IP地址信息</param>
    /// <remarks>无内存缓存，请谨慎调用</remarks>
    /// <returns></returns>
    private static async Task<WanNetIPInfo> GetWanNetInfoAsync(string ip)
    {
        // .NET5+ 后，默认没有注册 GBK 编码，所以这里进行注册
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var result = new WanNetIPInfo {Ip = ip};

        // 发送 Http 请求
        using var httpClient = new HttpClient();

        // 设置请求超时时间
        httpClient.Timeout = TimeSpan.FromSeconds(10);

        using var request = new HttpRequestMessage();

        // 设置请求 Url
        request.RequestUri = new Uri($"https://whois.pconline.com.cn/ipJson.jsp?ip={ip}");
        // 设置请求方式
        request.Method = HttpMethod.Get;
        // 设置请求头部
        request.Headers.Add("Accept", "application/json, text/plain, */*");

        // 添加默认 User-Agent
        request.Headers.TryAddWithoutValidation("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.5112.81 Safari/537.36 Edg/104.0.1293.47");

        try
        {
            // 发送请求
            using var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            // 这里默认使用 GBK 编码解析
            var responseContent = Encoding.GetEncoding("GBK")
                .GetString(response.Content.ReadAsByteArrayAsync()
                    .Result);

            var ipInfo = responseContent[
                    (responseContent.IndexOf("IPCallBack(", StringComparison.Ordinal) + "IPCallBack(".Length)..]
                .TrimEnd();
            ipInfo = ipInfo[..^3];

            var ipInfoDictionary = JsonSerializer.Deserialize<IDictionary<string, string>>(ipInfo);

            if (ipInfoDictionary.TryGetValue("ip", out var resIp))
            {
                result.Ip = resIp;
            }

            if (ipInfoDictionary.TryGetValue("pro", out var resPro))
            {
                result.Province = resPro;
            }

            if (ipInfoDictionary.TryGetValue("pro", out var resProCode))
            {
                result.ProvinceZipCode = resProCode;
            }

            if (ipInfoDictionary.TryGetValue("city", out var resCity))
            {
                result.City = resCity;
            }

            if (ipInfoDictionary.TryGetValue("cityCode", out var resCityCode))
            {
                result.CityZipCode = resCityCode;
            }

            if (ipInfoDictionary.TryGetValue("addr", out var resAddress))
            {
                result.Address = resAddress;
            }
        }
        catch (HttpRequestException ex)
        {
            var logSb = new StringBuilder();
            logSb.Append("\u001b[41m\u001b[30m");
            logSb.Append("fail");
            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            logSb.Append(": ");
            logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
            logSb.Append(Environment.NewLine);
            logSb.Append("\u001b[41m\u001b[30m");
            logSb.Append("      ");
            logSb.Append("远程请求错误");
            logSb.Append(Environment.NewLine);
            logSb.Append("      ");
            logSb.Append($"HttpContextExtension.GetWanNetInfoAsync: {ex}");
            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            Console.WriteLine(logSb.ToString());
        }
        catch (TaskCanceledException ex)
        {
            var logSb = new StringBuilder();
            logSb.Append("\u001b[41m\u001b[30m");
            logSb.Append("fail");
            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            logSb.Append(": ");
            logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
            logSb.Append(Environment.NewLine);
            logSb.Append("\u001b[41m\u001b[30m");
            logSb.Append("      ");
            logSb.Append("远程请求超时");
            logSb.Append(Environment.NewLine);
            logSb.Append("      ");
            logSb.Append($"HttpContextExtension.GetWanNetInfoAsync: {ex}");
            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            Console.WriteLine(logSb.ToString());
        }

        return result;
    }

    /// <summary>
    /// 获取 控制器/Action 描述器
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="ControllerActionDescriptor"/></returns>
    public static ControllerActionDescriptor GetControllerActionDescriptor(this HttpContext httpContext)
    {
        return httpContext.GetEndpoint()
            ?.Metadata.FirstOrDefault(u => u is ControllerActionDescriptor) as ControllerActionDescriptor;
    }

    /// <summary>
    /// 读取 Body 内容
    /// </summary>
    /// <remarks>需先在 Startup 的 Configure 中注册 app.EnableBuffering()</remarks>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="string"/></returns>
    public static async Task<string> ReadBodyContentAsync(this HttpContext httpContext)
    {
        if (httpContext == null)
            return null;
        return await httpContext.Request.ReadBodyContentAsync();
    }

    /// <summary>
    /// 读取 Body 内容
    /// </summary>
    /// <remarks>需先在 Startup 的 Configure 中注册 app.EnableBuffering()</remarks>
    /// <param name="httpRequest"><see cref="HttpRequest"/></param>
    /// <returns><see cref="string"/></returns>
    public static async Task<string> ReadBodyContentAsync(this HttpRequest httpRequest)
    {
        httpRequest.Body.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(httpRequest.Body, Encoding.UTF8, true, 1024, true);
        var body = await reader.ReadToEndAsync();

        httpRequest.Body.Seek(0, SeekOrigin.Begin);
        return body;
    }

    /// <summary>
    /// 完整请求地址
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="string"/></returns>
    public static string RequestUrlAddress(this HttpContext httpContext)
    {
        var request = httpContext?.Request;
        if (request != null)
        {
            return new StringBuilder().Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .Append(request.Path)
                .Append(request.QueryString)
                .ToString();
        }

        return string.Empty;
    }

    /// <summary>
    /// 完整请求地址
    /// </summary>
    /// <param name="httpRequest"><see cref="HttpRequest"/></param>
    /// <returns><see cref="string"/></returns>
    public static string RequestUrlAddress(this HttpRequest httpRequest)
    {
        if (httpRequest != null)
        {
            return new StringBuilder().Append(httpRequest.Scheme)
                .Append("://")
                .Append(httpRequest.Host)
                .Append(httpRequest.PathBase)
                .Append(httpRequest.Path)
                .Append(httpRequest.QueryString)
                .ToString();
        }

        return string.Empty;
    }

    /// <summary>
    /// 来源地址
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="refererHeaderKey">默认从 “Referer” 获取</param>
    /// <returns><see cref="string"/></returns>
    public static string RefererUrlAddress(this HttpContext httpContext, string refererHeaderKey = "Referer")
    {
        var request = httpContext?.Request;
        if (request != null)
        {
            return request.Headers[refererHeaderKey]
                .ToString();
        }

        return string.Empty;
    }

    /// <summary>
    /// 设置响应状态码
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="statusCode"><see cref="int"/></param>
    /// <param name="return200StatusCodes"><see cref="Array"/> 设置返回 200 状态码列表。只支持 400+(404除外) 状态码</param>
    /// <param name="adaptStatusCodes"><see cref="Array"/> 适配（篡改）状态码。只支持 400+(404除外) 状态码</param>
    /// <remarks>
    /// 示例：
    ///     return200StatusCodes = [401, 403]
    ///     adaptStatusCodes = [[401, 200], [403, 200]]
    /// </remarks>
    public static void SetResponseStatusCodes(this HttpContext httpContext, int statusCode, int[] return200StatusCodes = null,
        int[][] adaptStatusCodes = null)
    {
        // 篡改响应状态码
        if (adaptStatusCodes is {Length: > 0})
        {
            var adaptStatusCode = adaptStatusCodes.FirstOrDefault(f => f[0] == statusCode);
            if (adaptStatusCode is {Length: > 0} && adaptStatusCode[0] > 0)
            {
                httpContext.Response.StatusCode = adaptStatusCode[1];
                return;
            }
        }

        // 200 状态码返回
        if (return200StatusCodes is {Length: > 0})
        {
            // 判断当前状态码是否存在与200状态码列表中
            if (return200StatusCodes.Contains(statusCode))
            {
                httpContext.Response.StatusCode = StatusCodes.Status200OK;
            }
        }
    }
}