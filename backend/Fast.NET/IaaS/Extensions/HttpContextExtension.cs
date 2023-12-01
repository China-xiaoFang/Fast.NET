// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
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
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="HttpContext"/> 拓展类
/// </summary>
[SuppressSniffer]
public static class HttpContextExtension
{
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
        return httpContext.GetEndpoint()?.Metadata?.GetMetadata<TAttribute>();
    }

    /// <summary>
    /// 获取 Action 特性
    /// </summary>
    /// <param name="metadata"><see cref="EndpointMetadataCollection"/></param>
    /// <param name="attributeType"><see cref="Type"/></param>
    /// <returns><see cref="object"/></returns>
    public static object GetMetadata(this EndpointMetadataCollection metadata, Type attributeType)
    {
        return metadata?.GetType()?.GetMethod(nameof(EndpointMetadataCollection.GetMetadata))?.MakeGenericMethod(attributeType)
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
        return httpContext.GetEndpoint()?.Metadata.GetMetadata(attributeType);
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
        return httpContext.Connection.LocalIpAddress?.MapToIPv4()?.ToString();
    }

    /// <summary>
    /// 本机 IPv6 地址
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="string"/></returns>
    public static string LocalIpv6(this HttpContext httpContext)
    {
        return httpContext.Connection.LocalIpAddress?.MapToIPv6()?.ToString();
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

        var remoteIpv4 = httpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString();

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

        return remoteIpv4 ?? string.Empty;
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

        var remoteIpv4 = httpContext.Connection.RemoteIpAddress?.MapToIPv6()?.ToString();

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
    /// <remarks>注：如果需要正常解析，需要引用 "UAParser" 程序集，否则会返回 null</remarks>
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="UserAgentInfo"/></returns>
    public static UserAgentInfo RequestUserAgentInfo(this HttpContext httpContext)
    {
        // 获取用户代理字符串
        var userAgent = httpContext.RequestUserAgent();

        try
        {
            // 判断是否安装了 UAParser 程序集
            var uaParserAssembly = FastContext.Assemblies.FirstOrDefault(f => f.GetName().Name?.Equals("UAParser") == true);

            if (uaParserAssembly == null)
            {
                return default;
            }

            // 加载 UAParser 的 Parser 类型
            var uaParserParserType = Reflect.GetType(uaParserAssembly, "UAParser.Parser");

            if (uaParserParserType == null)
            {
                return default;
            }

            // 加载 Parser 类型 的 GetDefault() 方法
            var uaParserParserGetDefaultMethod = uaParserParserType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(f => f.Name == "GetDefault");

            if (uaParserParserGetDefaultMethod == null)
            {
                return default;
            }

            // 调用 Parser 类型 的 GetDefault() 方法
            var parser = uaParserParserGetDefaultMethod.Invoke(null, new object[] {null});

            // 加载 Parser 类型 的 Parse() 方法
            var uaParserParserParseMethod =
                uaParserParserType.GetMethods(BindingFlags.Public).FirstOrDefault(f => f.Name == "Parse");

            if (uaParserParserParseMethod == null)
            {
                return default;
            }

            // 调用 Parser 类型 的 Parse() 方法，解析用户代理字符串
            dynamic clientInfo = uaParserParserParseMethod.Invoke(parser, new object[] {userAgent});

            if (clientInfo == null)
            {
                return default;
            }

            return new UserAgentInfo
            {
                Device = clientInfo.Device.ToString(), OS = clientInfo.OS.ToString(), Browser = clientInfo.UA.ToString()
            };
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// 远程 Ipv4 地址信息
    /// <remarks>自带内存缓存，缓存过期时间为24小时（注：需要注入内存缓存，如不注入，则默认不走缓存）</remarks>
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="ip"><see cref="string"/> 要的IP地址信息，默认为 null，如果为 null，默认获取当前远程的 Ipv4 地址</param>
    /// <returns><see cref="WanNetIPInfo"/></returns>
    /// <exception cref="Exception"></exception>
    public static WanNetIPInfo RemoteIpv4Info(this HttpContext httpContext, string ip = null)
    {
        return httpContext.RemoteIpv4InfoAsync(ip).Result;
    }

    /// <summary>
    /// 远程 Ipv4 地址信息
    /// <remarks>自带内存缓存，缓存过期时间为24小时（注：需要注入内存缓存，如不注入，则默认不走缓存）</remarks>
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="ip"><see cref="string"/> 要的IP地址信息，默认为 null，如果为 null，默认获取当前远程的 Ipv4 地址</param>
    /// <returns><see cref="WanNetIPInfo"/></returns>
    /// <exception cref="Exception"></exception>
    public static async Task<WanNetIPInfo> RemoteIpv4InfoAsync(this HttpContext httpContext, string ip = null)
    {
        // 判断是否传入IP地址
        ip ??= httpContext.RemoteIpv4();

        // 获取内存缓存服务
        var _memoryCache = httpContext.RequestServices.GetService<IMemoryCache>();

        // 优先从内存缓存中获取
        var result = _memoryCache?.Get<WanNetIPInfo>($"Fast.NET:Http:RemoteIpv4Info:{ip}");

        if (result == null)
        {
            var (responseContent, _) =
                await RemoteRequestUtil.GetAsync($"http://whois.pconline.com.cn/ipJson.jsp?ip={ip}", timeout: 30);
            var ipInfo = responseContent[
                (responseContent.IndexOf("IPCallBack(", StringComparison.Ordinal) + "IPCallBack(".Length)..].TrimEnd();
            ipInfo = ipInfo[..^3];

            var ipInfoDictionary = JsonSerializer.Deserialize<IDictionary<string, string>>(ipInfo);

            result = new WanNetIPInfo();

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

        // 放入内存缓存中，设置过期时间为24个小时
        _memoryCache?.Set($"Fast.NET:Http:RemoteIpv4Info:{ip}", result, TimeSpan.FromHours(24));

        return result;
    }

    /// <summary>
    /// 获取 控制器/Action 描述器
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="ControllerActionDescriptor"/></returns>
    public static ControllerActionDescriptor GetControllerActionDescriptor(this HttpContext httpContext)
    {
        return httpContext.GetEndpoint()?.Metadata?.FirstOrDefault(u => u is ControllerActionDescriptor) as
            ControllerActionDescriptor;
    }

    /// <summary>
    /// 读取 Body 内容
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <remarks>需先在 Startup 的 Configure 中注册 app.EnableBuffering()</remarks>
    /// <returns><see cref="string"/></returns>
    public static async Task<string> ReadBodyContentAsync(this HttpContext httpContext)
    {
        if (httpContext == null)
            return default;
        return await httpContext.Request.ReadBodyContentAsync();
    }

    /// <summary>
    /// 读取 Body 内容
    /// </summary>
    /// <param name="httpRequest"><see cref="HttpRequest"/></param>
    /// <remarks>需先在 Startup 的 Configure 中注册 app.EnableBuffering()</remarks>
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
            return new StringBuilder().Append(request.Scheme).Append("://").Append(request.Host).Append(request.PathBase)
                .Append(request.Path).Append(request.QueryString).ToString();
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
            return new StringBuilder().Append(httpRequest.Scheme).Append("://").Append(httpRequest.Host)
                .Append(httpRequest.PathBase).Append(httpRequest.Path).Append(httpRequest.QueryString).ToString();
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
            return request.Headers[refererHeaderKey].ToString();
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