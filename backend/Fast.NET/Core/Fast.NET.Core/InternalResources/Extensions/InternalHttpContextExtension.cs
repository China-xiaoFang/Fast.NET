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
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Fast.NET;

/// <summary>
/// <see cref="HttpContext"/> 内部拓展类
/// </summary>
/// <exclude />
internal static class InternalHttpContextExtension
{
    /// <summary>
    /// 判断是否是 WebSocket 请求
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="bool"/></returns>
    /// <exclude />
    internal static bool IsWebSocketRequest(this HttpContext httpContext)
    {
        return httpContext.WebSockets.IsWebSocketRequest || httpContext.Request.Path == "/ws";
    }

    /// <summary>
    /// 获取 Action 特性
    /// </summary>
    /// <typeparam name="TAttribute"></typeparam>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns></returns>
    /// <exclude />
    internal static TAttribute GetMetadata<TAttribute>(this HttpContext httpContext) where TAttribute : class
    {
        return httpContext.GetEndpoint()?.Metadata?.GetMetadata<TAttribute>();
    }

    /// <summary>
    /// 获取 Action 特性
    /// </summary>
    /// <param name="metadata"><see cref="EndpointMetadataCollection"/></param>
    /// <param name="attributeType"><see cref="Type"/></param>
    /// <returns><see cref="object"/></returns>
    /// <exclude />
    internal static object GetMetadata(this EndpointMetadataCollection metadata, Type attributeType)
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
    /// <exclude />
    internal static object GetMetadata(this HttpContext httpContext, Type attributeType)
    {
        return httpContext.GetEndpoint()?.Metadata.GetMetadata(attributeType);
    }

    /// <summary>
    /// 设置规范化文档自动登录
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="accessToken"></param>
    /// <exclude />
    internal static void SignInToSwagger(this HttpContext httpContext, string accessToken)
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
    /// <exclude />
    internal static void SignOutToSwagger(this HttpContext httpContext)
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
    /// <exclude />
    internal static string LanIpv4(this HttpContext httpContext)
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
    /// <exclude />
    internal static string LanIpv6(this HttpContext httpContext)
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
    /// <exclude />
    internal static string LocalIpv4(this HttpContext httpContext)
    {
        return httpContext.Connection.LocalIpAddress?.MapToIPv4()?.ToString();
    }

    /// <summary>
    /// 本机 IPv6 地址
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="string"/></returns>
    /// <exclude />
    internal static string LocalIpv6(this HttpContext httpContext)
    {
        return httpContext.Connection.LocalIpAddress?.MapToIPv6()?.ToString();
    }

    /// <summary>
    /// 远程 Ipv4 地址
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="string"/></returns>
    /// <exclude />
    internal static string RemoteIpv4(this HttpContext httpContext)
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
    /// <exclude />
    internal static string RemoteIpv6(this HttpContext httpContext)
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
    /// <exclude />
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
    /// <exclude />
    public static UserAgentInfo RequestUserAgentInfo(this HttpContext httpContext)
    {
        // 获取用户代理字符串
        var userAgent = httpContext.RequestUserAgent();

        try
        {
            // 判断是否安装了 UAParser 程序集
            var uaParserAssembly =
                InternalPenetrates.Assemblies.FirstOrDefault(f => f.GetName().Name?.Equals("UAParser") == true);

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
    /// <exclude />
    internal static WanNetIPInfo RemoteIpv4Info(this HttpContext httpContext, string ip = null)
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
    /// <exclude />
    internal static async Task<WanNetIPInfo> RemoteIpv4InfoAsync(this HttpContext httpContext, string ip = null)
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
                await InternalRemoteRequestUtil.GetAsync($"http://whois.pconline.com.cn/ipJson.jsp?ip={ip}", timeout: 30);
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

            if (ipInfoDictionary.TryGetValue("addr", out var resAddr))
            {
                result.Address = resAddr;
            }
        }

        // 放入内存缓存中，设置过期时间为24个小时
        _memoryCache?.Set($"Fast.NET:Http:RemoteIpv4Info:{ip}", result, TimeSpan.FromHours(24));

        return result;
    }
}