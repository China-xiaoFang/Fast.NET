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

using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Runtime;

/// <summary>
/// 为 <see cref="HttpContext"/> 提供扩展方法
/// </summary>
[SuppressSniffer]
public static class HttpContextExtension
{
    /// <summary>
    /// IP 信息查询使用的锁分片数量
    /// </summary>
    private const int IP_LOOKUP_LOCK_COUNT = 64;

    /// <summary>
    /// IP 信息查询客户端，复用底层连接池
    /// </summary>
    private static readonly HttpClient _ipLookupHttpClient = new() {Timeout = Timeout.InfiniteTimeSpan};

    /// <summary>
    /// IP 信息查询锁分片，限制锁对象数量并保证同一缓存键串行回源
    /// </summary>
    private static readonly SemaphoreSlim[] _ipLookupLocks = Enumerable.Range(0, IP_LOOKUP_LOCK_COUNT)
        .Select(_ => new SemaphoreSlim(1, 1))
        .ToArray();

    static HttpContextExtension()
    {
        // 现代 .NET 默认不注册代码页编码，初始化一次即可复用 GBK 解码器
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    /// <summary>
    /// 设置规范化响应时间戳
    /// </summary>
    /// <param name="httpContext">当前请求上下文</param>
    /// <param name="timestamp">时间戳</param>
    public static void UnifyResponseTimestamp(this HttpContext httpContext, long timestamp)
    {
        httpContext?.Response.Headers.TryAdd(nameof(Fast) + "-NET-Timestamp", $"{timestamp}");
    }

    /// <summary>
    /// 获取规范化响应时间戳
    /// </summary>
    /// <param name="httpContext">当前请求上下文</param>
    /// <returns>获取到的规范化响应时间戳</returns>
    public static long UnifyResponseTimestamp(this HttpContext httpContext)
    {
        var timestampStr = httpContext?.Response.Headers[nameof(Fast) + "-NET-Timestamp"];

        if (string.IsNullOrEmpty(timestampStr))
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // 将请求开始时间写入响应头，供耗时统计复用
            httpContext.UnifyResponseTimestamp(timestamp);

            return timestamp;
        }

        return long.Parse(timestampStr, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// 获取终结点元数据中的指定特性
    /// </summary>
    /// <param name="httpContext">当前请求上下文</param>
    /// <typeparam name="TAttribute">要查找的特性类型</typeparam>
    /// <returns>匹配的特性实例；未找到时返回 <see langword="null"/></returns>
    public static TAttribute GetMetadata<TAttribute>(this HttpContext httpContext) where TAttribute : class
    {
        return httpContext.GetEndpoint()
            ?.Metadata.GetMetadata<TAttribute>();
    }

    /// <summary>
    /// 获取终结点元数据中的指定特性
    /// </summary>
    /// <param name="metadata">当前元数据</param>
    /// <param name="attributeType">要读取的特性类型</param>
    /// <returns>匹配的特性实例；未找到时返回 <see langword="null"/></returns>
    public static object GetMetadata(this EndpointMetadataCollection metadata, Type attributeType)
    {
        return metadata?.GetType()
            .GetMethod(nameof(EndpointMetadataCollection.GetMetadata))
            ?.MakeGenericMethod(attributeType)
            .Invoke(metadata, null);
    }

    /// <summary>
    /// 获取终结点元数据中的指定特性
    /// </summary>
    /// <param name="httpContext">当前请求上下文</param>
    /// <param name="attributeType">要读取的特性类型</param>
    /// <returns>匹配的特性实例；未找到时返回 <see langword="null"/></returns>
    public static object GetMetadata(this HttpContext httpContext, Type attributeType)
    {
        return httpContext.GetEndpoint()
            ?.Metadata.GetMetadata(attributeType);
    }

    /// <summary>
    /// 设置规范化文档自动登录
    /// </summary>
    /// <param name="httpContext">当前请求上下文</param>
    /// <param name="accessToken">访问令牌</param>
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
    /// <param name="httpContext">当前请求上下文</param>
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
    /// <param name="httpContext">当前请求上下文</param>
    /// <returns>局域网 IPv4 地址</returns>
    public static string LanIpv4(this HttpContext httpContext)
    {
        var remoteIpAddress = httpContext.Connection.RemoteIpAddress;
        if (remoteIpAddress is {AddressFamily: AddressFamily.InterNetwork})
        {
            return remoteIpAddress.ToString();
        }

        // localhost 的 IPv6 回环地址为 ::1，按当前方法的地址族进行转换
        if (remoteIpAddress is {AddressFamily: AddressFamily.InterNetworkV6} && remoteIpAddress.ToString() == "::1")
        {
            return "127.0.0.1";
        }

        return string.Empty;
    }

    /// <summary>
    /// 局域网 IPv6 地址
    /// </summary>
    /// <param name="httpContext">当前请求上下文</param>
    /// <returns>局域网 IPv6 地址</returns>
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
    /// <param name="httpContext">当前请求上下文</param>
    /// <returns>本机 IPv4 地址</returns>
    public static string LocalIpv4(this HttpContext httpContext)
    {
        var localIpAddress = httpContext.Connection.LocalIpAddress;
        // localhost 的 IPv6 回环地址为 ::1，按当前方法的地址族进行转换
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
    /// <param name="httpContext">当前请求上下文</param>
    /// <returns>本机 IPv6 地址</returns>
    public static string LocalIpv6(this HttpContext httpContext)
    {
        return httpContext.Connection.LocalIpAddress?.MapToIPv6()
            .ToString();
    }

    /// <summary>
    /// 远程 IPv4 地址
    /// </summary>
    /// <param name="httpContext">当前请求上下文</param>
    /// <returns>远程 IPv4 地址</returns>
    public static string RemoteIpv4(this HttpContext httpContext)
    {
        if (httpContext == null)
            return string.Empty;

        var remoteIpv4 = string.Empty;

        // Nginx 反向代理场景优先读取转发地址头
        if (httpContext.Request.Headers.TryGetValue("X-Real-IP", out var header1))
        {
            if (IPAddress.TryParse(header1, out var ipv4) && ipv4.AddressFamily == AddressFamily.InterNetwork)
            {
                remoteIpv4 = ipv4.ToString();
            }
        }

        // 启用受信任代理后，优先读取代理传递的客户端 IP 地址
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
            // localhost 的 IPv6 回环地址为 ::1，按当前方法的地址族进行转换
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
    /// 远程 IPv6 地址
    /// </summary>
    /// <param name="httpContext">当前请求上下文</param>
    /// <returns>远程 IPv6 地址</returns>
    public static string RemoteIpv6(this HttpContext httpContext)
    {
        if (httpContext == null)
            return string.Empty;

        var remoteIpv4 = httpContext.Connection.RemoteIpAddress?.MapToIPv6()
            .ToString();

        // Nginx 反向代理场景优先读取转发地址头
        if (httpContext.Request.Headers.TryGetValue("X-Real-IP", out var header1))
        {
            if (IPAddress.TryParse(header1, out var ipv6) && ipv6.AddressFamily == AddressFamily.InterNetworkV6)
            {
                remoteIpv4 = ipv6.ToString();
            }
        }

        // 启用受信任代理后，优先读取代理传递的客户端 IP 地址
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
    /// <param name="httpContext">当前请求上下文</param>
    /// <param name="userAgentHeaderKey">包含用户代理信息的请求头名称</param>
    /// <returns>请求用户代理字符串（User-Agent）</returns>
    public static string RequestUserAgent(this HttpContext httpContext, string userAgentHeaderKey = "User-Agent")
    {
        return httpContext?.Request.Headers[userAgentHeaderKey];
    }

    /// <summary>
    /// 请求用户代理信息（User-Agent）
    /// </summary>
    /// <remarks>注：如果需要正常解析，需要引用 "UAParser" 程序集，否则会返回 <see langword="null"/></remarks>
    /// <param name="httpContext">当前请求上下文</param>
    /// <returns>请求用户代理信息（User-Agent）</returns>
    public static UserAgentInfo RequestUserAgentInfo(this HttpContext httpContext)
    {
        // 同一请求内优先复用 HttpContext.Items 中的解析结果
        var userAgentObj = httpContext.Items[nameof(Fast) + nameof(UserAgentInfo)];

        if (userAgentObj != null)
        {
            return userAgentObj as UserAgentInfo;
        }

        // 获取用户代理字符串
        var userAgent = httpContext.RequestUserAgent();

        try
        {
            // 判断是否安装了 UAParser 程序集
            var uaParserAssembly = MAppContext.Assemblies.SingleOrDefault(s => s.GetName()
                                                                                   .Name?.Equals("UAParser",
                                                                                       StringComparison.Ordinal)
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

            // Parse 是唯一匹配的公共实例方法，按名称获取即可兼容 UAParser 的可选依赖加载方式
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

            // 缓存到 HttpContext.Items，避免同一请求重复解析
            httpContext.Items[nameof(Fast) + nameof(UserAgentInfo)] = result;

            return result;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 远程 IPv4 地址信息
    /// </summary>
    /// <remarks>自带内存缓存，缓存过期时间为 24 小时（注：需要注入内存缓存，如不注入，则默认不走缓存）</remarks>
    /// <param name="httpContext">当前请求上下文</param>
    /// <param name="ip">要的 IP 地址信息，默认为 <see langword="null"/>，如果为 <see langword="null"/>，默认获取当前远程的 IPv4 地址</param>
    /// <returns>远程 IPv4 地址信息</returns>
    public static WanNetIPInfo RemoteIpv4Info(this HttpContext httpContext, string ip = null)
    {
        return httpContext.RemoteIpv4InfoAsync(ip)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// 远程 IPv4 地址信息
    /// </summary>
    /// <remarks>自带内存缓存，缓存过期时间为 24 小时（注：需要注入内存缓存，如不注入，则默认不走缓存）</remarks>
    /// <param name="httpContext">当前请求上下文</param>
    /// <param name="ip">要的 IP 地址信息，默认为 <see langword="null"/>，如果为 <see langword="null"/>，默认获取当前远程的 IPv4 地址</param>
    /// <returns>表示异步远程 IPv4 地址信息的任务，任务结果为远程 IPv4 地址信息</returns>
    public static async Task<WanNetIPInfo> RemoteIpv4InfoAsync(this HttpContext httpContext, string ip = null)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        // 同一请求内优先复用 HttpContext.Items 中的解析结果
        var wanNetIPInfoObj = httpContext.Items[nameof(Fast) + nameof(WanNetIPInfo)];

        if (wanNetIPInfoObj != null)
        {
            return wanNetIPInfoObj as WanNetIPInfo;
        }

        // 判断是否传入 IP 地址
        ip ??= httpContext.RemoteIpv4();

        WanNetIPInfo result;

        // 获取内存缓存服务
        var _memoryCache = httpContext.RequestServices.GetService<IMemoryCache>();

        if (_memoryCache == null)
        {
            result = await GetWanNetInfoAsync(ip, httpContext.RequestAborted)
                .ConfigureAwait(false);
        }
        else
        {
            var cacheKey = $"{nameof(Fast)}.NET:Http:RemoteIpv4Info:{ip}";

            // 同一缓存键始终映射到同一锁分片，避免重复回源和按 IP 永久累积锁对象
            var semaphoreSlim = GetIpLookupLock(cacheKey);

            await semaphoreSlim.WaitAsync(httpContext.RequestAborted)
                .ConfigureAwait(false);
            try
            {
                // 从缓存中读取
                if (!_memoryCache.TryGetValue(cacheKey, out result))
                {
                    result = await GetWanNetInfoAsync(ip, httpContext.RequestAborted)
                        .ConfigureAwait(false);
                    // 放入内存缓存，设置过期时间为 24 个小时
                    _memoryCache.Set(cacheKey, result, TimeSpan.FromHours(24));
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        // 缓存到 HttpContext.Items，避免同一请求重复解析
        httpContext.Items[nameof(Fast) + nameof(WanNetIPInfo)] = result;

        return result;
    }

    /// <summary>
    /// 获取指定缓存键对应的 IP 查询锁分片
    /// </summary>
    /// <param name="cacheKey">IP 信息缓存键</param>
    /// <returns>用于串行化同一缓存键回源操作的信号量</returns>
    private static SemaphoreSlim GetIpLookupLock(string cacheKey)
    {
        var hashCode = StringComparer.Ordinal.GetHashCode(cacheKey);
        return _ipLookupLocks[(int) ((uint) hashCode % (uint) _ipLookupLocks.Length)];
    }

    /// <summary>
    /// 获取远程 IPv4 地址信息
    /// </summary>
    /// <remarks>无内存缓存，请谨慎调用</remarks>
    /// <param name="ip">IP 地址信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>表示异步获取远程 IPv4 地址信息的任务，任务结果为获取到的远程 IPv4 地址信息</returns>
    private static async Task<WanNetIPInfo> GetWanNetInfoAsync(string ip, CancellationToken cancellationToken)
    {
        var result = new WanNetIPInfo {Ip = ip};
        using var timeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutTokenSource.CancelAfter(TimeSpan.FromSeconds(10));

        using var request = new HttpRequestMessage();

        request.RequestUri = new Uri($"https://whois.pconline.com.cn/ipJson.jsp?ip={Uri.EscapeDataString(ip)}&json=true");
        request.Method = HttpMethod.Get;
        // 设置请求头部
        request.Headers.Add("Accept", "application/json, text/plain, */*");

        // 添加默认 User-Agent
        request.Headers.TryAddWithoutValidation("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.5112.81 Safari/537.36 Edg/104.0.1293.47");

        try
        {
            using var response = await _ipLookupHttpClient.SendAsync(request, timeoutTokenSource.Token)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            // 目标 IP 查询服务固定返回 GBK 字节流，不能依赖响应头推断编码
            var responseBytes = await response.Content.ReadAsByteArrayAsync(timeoutTokenSource.Token)
                .ConfigureAwait(false);
            var responseContent = Encoding.GetEncoding("GBK")
                .GetString(responseBytes);

            var ipInfoDictionary = JsonSerializer.Deserialize<IDictionary<string, string>>(responseContent);
            if (ipInfoDictionary == null)
                return result;

            if (ipInfoDictionary.TryGetValue("ip", out var resIp))
            {
                result.Ip = resIp;
            }

            if (ipInfoDictionary.TryGetValue("pro", out var resPro))
            {
                result.Province = resPro;
            }

            if (ipInfoDictionary.TryGetValue("proCode", out var resProCode))
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
            MAppContext.ConsoleWrite(console =>
            {
                console.BackgroundColor = ConsoleColor.DarkRed;
                console.ForegroundColor = ConsoleColor.Black;
                console.Write("fail");
                console.ResetColor();
                console.WriteLine($": {DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                console.BackgroundColor = ConsoleColor.DarkRed;
                console.ForegroundColor = ConsoleColor.Black;
                console.WriteLine("      远程请求错误");
                console.WriteLine($"      HttpContextExtension.GetWanNetInfoAsync: {ex}");
            });
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // 请求终止属于正常取消，交由调用方感知
            throw;
        }
        catch (OperationCanceledException ex) when (timeoutTokenSource.IsCancellationRequested)
        {
            MAppContext.ConsoleWrite(console =>
            {
                console.BackgroundColor = ConsoleColor.DarkRed;
                console.ForegroundColor = ConsoleColor.Black;
                console.Write("fail");
                console.ResetColor();
                console.WriteLine($": {DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                console.BackgroundColor = ConsoleColor.DarkRed;
                console.ForegroundColor = ConsoleColor.Black;
                console.WriteLine("      远程请求超时");
                console.WriteLine($"      HttpContextExtension.GetWanNetInfoAsync: {ex}");
            });
        }

        return result;
    }

    /// <summary>
    /// 获取 控制器/Action 描述器
    /// </summary>
    /// <param name="httpContext">当前请求上下文</param>
    /// <returns>获取到的 控制器/Action 描述器</returns>
    public static ControllerActionDescriptor GetControllerActionDescriptor(this HttpContext httpContext)
    {
        return httpContext.GetEndpoint()
            ?.Metadata.FirstOrDefault(u => u is ControllerActionDescriptor) as ControllerActionDescriptor;
    }

    /// <summary>
    /// 读取 Body 内容
    /// </summary>
    /// <remarks>需先在 Startup 的 Configure 中注册 app.EnableBuffering()</remarks>
    /// <param name="httpContext">当前请求上下文</param>
    /// <returns>表示异步读取 Body 内容的任务，任务结果为读取到的 Body 内容</returns>
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
    /// <param name="httpRequest">当前 HTTP 请求</param>
    /// <returns>表示异步读取 Body 内容的任务，任务结果为读取到的 Body 内容</returns>
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
    /// <param name="httpContext">当前请求上下文</param>
    /// <returns>完整请求地址</returns>
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
    /// <param name="httpRequest">当前 HTTP 请求</param>
    /// <returns>完整请求地址</returns>
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
    /// <param name="httpContext">当前请求上下文</param>
    /// <param name="refererHeaderKey">包含来源地址的请求头名称</param>
    /// <returns>来源地址</returns>
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
    /// <remarks>
    /// 示例
    /// return200StatusCodes = [401, 403]
    /// adaptStatusCodes = [[401, 200], [403, 200]]
    /// </remarks>
    /// <param name="httpContext">当前请求上下文</param>
    /// <param name="statusCode">HTTP 状态码</param>
    /// <param name="return200StatusCodes">设置返回 200 状态码列表。只支持 400+(404 除外) 状态码</param>
    /// <param name="adaptStatusCodes">适配（篡改）状态码。只支持 400+(404 除外) 状态码</param>
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
            // 判断当前状态码是否存在与 200 状态码列表中
            if (return200StatusCodes.Contains(statusCode))
            {
                httpContext.Response.StatusCode = StatusCodes.Status200OK;
            }
        }
    }
}
