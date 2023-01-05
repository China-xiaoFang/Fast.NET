using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Core.ServiceCollection;

/// <summary>
/// 远程请求
/// </summary>
public static class RemoteRequest
{
    /// <summary>
    /// 注册远程请求
    /// </summary>
    /// <param name="service"></param>
    public static void AddRemoteRequest(this IServiceCollection service)
    {
        service.AddRemoteRequest(option =>
        {
            // The weather forecast GZIP.
            option.AddHttpClient("weatherCdn", c => { c.BaseAddress = new Uri("http://wthrcdn.etouch.cn/"); })
                .ConfigurePrimaryHttpMessageHandler(_ =>
                    new HttpClientHandler {AutomaticDecompression = DecompressionMethods.GZip});
        });
    }
}