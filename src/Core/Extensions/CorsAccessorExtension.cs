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

using Fast.Runtime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Fast.NET.Core;

/// <summary>
/// <see cref="CorsAccessorExtension"/> 跨域处理 拓展类
/// </summary>
[SuppressSniffer]
public static class CorsAccessorExtension
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
            builder.WithOrigins(corsAccessorSettings.WithOrigins)
                .SetIsOriginAllowedToAllowWildcardSubdomains();

        // 如果没有配置请求标头，则允许所有表头，包含处理 SignalR 情况
        if (corsAccessorSettings.WithHeaders == null || corsAccessorSettings.WithHeaders.Length == 0 || isSupportSignalR)
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
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray());
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
            exposedHeaders = exposedHeaders.Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        if (exposedHeaders.Any())
            builder.WithExposedHeaders(exposedHeaders.ToArray());

        // 设置预检过期时间，如果不设置默认为 24小时
        builder.SetPreflightMaxAge(TimeSpan.FromSeconds(corsAccessorSettings.SetPreflightMaxAge ?? 24 * 60 * 60));
    }

    /// <summary>
    /// 添加跨域服务
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/></param>
    /// <returns><see cref="WebApplicationBuilder"/></returns>
    public static WebApplicationBuilder AddCorsAccessor(this WebApplicationBuilder builder)
    {
        builder.Services.AddCorsAccessor(builder.Configuration);

        return builder;
    }

    /// <summary>
    /// 添加跨域服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    /// <param name="section"><see cref="string"/>
    /// <para>Json配置文件节点的Key</para>
    /// <para>默认值：CorsAccessorSettings</para>
    /// </param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddCorsAccessor(this IServiceCollection services, IConfiguration configuration,
        string section = "CorsAccessorSettings")
    {
        Debugging.Info("Registering for the Cors accessor service......");

        // 配置验证
        services.AddConfigurableOptions<CorsAccessorSettingsOptions>(section);

        // 获取跨域配置选项
        var corsAccessorSettings = configuration.GetSection(section)
            .Get<CorsAccessorSettingsOptions>()
            .LoadPostConfigure();

        // 添加跨域服务
        services.AddCors(options =>
        {
            // 添加策略跨域
            options.AddPolicy(corsAccessorSettings.PolicyName, configurePolicy =>
            {
                // 设置跨域策略
                SetCorsPolicy(configurePolicy, corsAccessorSettings);
            });
        });

        // 注册 CorsAccessor Startup 过滤器
        services.AddTransient(typeof(IStartupFilter), typeof(CorsAccessorStartupFilter));

        return services;
    }

    /// <summary>
    /// 添加跨域服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="optionAction"><see cref="Action{T}"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddCorsAccessor(this IServiceCollection services,
        Action<CorsAccessorSettingsOptions> optionAction)
    {
        Debugging.Info("Registering for the Cors accessor service......");

        // 配置验证
        services.Configure(optionAction);

        var corsAccessorSettings = new CorsAccessorSettingsOptions();
        optionAction.Invoke(corsAccessorSettings);

        // 添加跨域服务
        services.AddCors(options =>
        {
            // 添加策略跨域
            options.AddPolicy(corsAccessorSettings.PolicyName, configurePolicy =>
            {
                // 设置跨域策略
                SetCorsPolicy(configurePolicy, corsAccessorSettings);
            });
        });

        // 注册 CorsAccessor Startup 过滤器
        services.AddTransient(typeof(IStartupFilter), typeof(CorsAccessorStartupFilter));

        return services;
    }
}