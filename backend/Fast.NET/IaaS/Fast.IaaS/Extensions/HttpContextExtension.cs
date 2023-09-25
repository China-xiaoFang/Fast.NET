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
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Fast.IaaS.Extensions;

/// <summary>
/// <see cref="HttpContext"/> 拓展类
/// </summary>
public static class HttpContextExtension
{
    /// <summary>
    /// 获取 Action 特性
    /// </summary>
    /// <typeparam name="TAttribute"></typeparam>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <returns><see cref="TAttribute"/></returns>
    public static TAttribute GetMetadata<TAttribute>(this HttpContext httpContext) where TAttribute : class
    {
        return httpContext.GetEndpoint()?.Metadata?.GetMetadata<TAttribute>();
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
    /// 设置规范化文档自动登录
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="accessToken"></param>
    public static void SignInToSwagger(this HttpContext httpContext, string accessToken)
    {
        // 设置 Swagger 刷新自动授权
        httpContext.Response.Headers["access-token"] = accessToken;
    }

    /// <summary>
    /// 设置规范化文档退出登录
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    public static void SignOutToSwagger(this HttpContext httpContext)
    {
        httpContext.Response.Headers["access-token"] = "invalid_token";
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
    /// <param name="request"><see cref="HttpRequest"/></param>
    /// <remarks>需先在 Startup 的 Configure 中注册 app.EnableBuffering()</remarks>
    /// <returns><see cref="string"/></returns>
    public static async Task<string> ReadBodyContentAsync(this HttpRequest request)
    {
        request.Body.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
        var body = await reader.ReadToEndAsync();

        request.Body.Seek(0, SeekOrigin.Begin);
        return body;
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
    /// 请求 UserAgent 信息
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="userAgentHeaderKey">默认从 “User-Agent” 获取</param>
    /// <returns><see cref="string"/></returns>
    public static string UserAgent(this HttpContext httpContext, string userAgentHeaderKey = "User-Agent")
    {
        return httpContext?.Request.Headers[userAgentHeaderKey];
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
}