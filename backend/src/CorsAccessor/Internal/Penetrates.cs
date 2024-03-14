using Fast.CorsAccessor.Options;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Fast.CorsAccessor.Internal;

/// <summary>
/// <see cref="Penetrates"/> 常量、公共方法配置类
/// </summary>
internal static class Penetrates
{
    /// <summary>
    /// 默认跨域导出响应头 Key
    /// </summary>
    /// <remarks>解决 ajax，XMLHttpRequest，axios 不能获取请求头问题</remarks>
    private static readonly string[] _defaultExposedHeaders = {"access-token", "x-access-token"};

    /// <summary>
    /// 设置跨域策略
    /// </summary>
    /// <param name="builder"><see cref="CorsPolicyBuilder"/></param>
    /// <param name="corsAccessorSettings"><see cref="CorsAccessorSettingsOptions"/></param>
    /// <param name="isMiddleware"><see cref="bool"/></param>
    internal static void SetCorsPolicy(CorsPolicyBuilder builder, CorsAccessorSettingsOptions corsAccessorSettings,
        bool isMiddleware = false)
    {
        // 判断是否设置了来源，因为 AllowAnyOrigin 不能和 AllowCredentials一起公用
        var isNotSetOrigins = corsAccessorSettings.WithOrigins == null || corsAccessorSettings.WithOrigins.Length == 0;

        // https://docs.microsoft.com/zh-cn/aspnet/core/signalr/security?view=aspnetcore-6.0
        var isSupportSignalR = isMiddleware && corsAccessorSettings.SignalRSupport == true;

        // 设置总是允许跨域源配置
        builder.SetIsOriginAllowed(_ => true);

        // 如果没有配置来源，则允许所有来源
        if (isNotSetOrigins)
        {
            // 解决 SignalR  不能配置允许所有源问题
            if (!isSupportSignalR)
                builder.AllowAnyOrigin();
        }
        else
            builder.WithOrigins(corsAccessorSettings.WithOrigins).SetIsOriginAllowedToAllowWildcardSubdomains();

        // 如果没有配置请求标头，则允许所有表头，包含处理 SignalR 情况
        if ((corsAccessorSettings.WithHeaders == null || corsAccessorSettings.WithHeaders.Length == 0) || isSupportSignalR)
            builder.AllowAnyHeader();
        else
            builder.WithHeaders(corsAccessorSettings.WithHeaders);

        // 如果没有配置任何请求谓词，则允许所有请求谓词
        if (corsAccessorSettings.WithMethods == null || corsAccessorSettings.WithMethods.Length == 0)
            builder.AllowAnyMethod();
        else
        {
            // 解决 SignalR 必须允许 GET POST 问题
            if (isSupportSignalR)
            {
                builder.WithMethods(corsAccessorSettings.WithMethods.Concat(new[] {"GET", "POST"})
                    .Distinct(StringComparer.OrdinalIgnoreCase).ToArray());
            }
            else
                builder.WithMethods(corsAccessorSettings.WithMethods);
        }

        // 配置跨域凭据，包含处理 SignalR 情况
        if ((corsAccessorSettings.AllowCredentials == true && !isNotSetOrigins) || isSupportSignalR)
            builder.AllowCredentials();

        // 配置响应头，如果前端不能获取自定义的 header 信息，必须配置该项，默认配置了 access-token 和 x-access-token，可取消默认行为
        var exposedHeaders = corsAccessorSettings.FixedClientToken == true ? _defaultExposedHeaders.ToList() : new List<string>();
        if (corsAccessorSettings.WithExposedHeaders != null && corsAccessorSettings.WithExposedHeaders.Length > 0)
        {
            exposedHeaders.AddRange(corsAccessorSettings.WithExposedHeaders);
            exposedHeaders = exposedHeaders.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        if (exposedHeaders.Any())
            builder.WithExposedHeaders(exposedHeaders.ToArray());

        // 设置预检过期时间，如果不设置默认为 24小时
        builder.SetPreflightMaxAge(TimeSpan.FromSeconds(corsAccessorSettings.SetPreflightMaxAge ?? 24 * 60 * 60));
    }
}