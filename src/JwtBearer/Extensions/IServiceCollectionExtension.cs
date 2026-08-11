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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fast.JwtBearer;

/// <summary>
/// 为 <see cref="IServiceCollection"/> 提供动态 API 扩展方法。
/// </summary>
[SuppressSniffer]
public static class IServiceCollectionExtension
{
    /// <summary>
    /// 添加 JwtBearer 设置。
    /// </summary>
    /// <remarks>适用于只使用工具类。</remarks>
    /// <param name="services">要添加服务的服务集合。</param>
    /// <param name="configuration">用于读取模块设置的配置。</param>
    /// <param name="section">JSON 配置文件节点的 Key 默认值：JWTSettings。</param>
    /// <returns>返回 <paramref name="services"/>，便于链式调用。</returns>
    public static IServiceCollection AddJwtBearerSetting(this IServiceCollection services, IConfiguration configuration,
        string section = "JWTSettings")
    {
        services.AddConfigurableOptions<JWTSettingsOptions>(section);

        Penetrates.JWTSettings = configuration.GetSection(section)
            .Get<JWTSettingsOptions>()
            .LoadPostConfigure();

        // 未配置 Redis 等 IDistributedCache 实现时提供进程内回退。
        // AddDistributedMemoryCache 使用 TryAdd 注册，不会覆盖用户已经配置的缓存实现。
        services.AddDistributedMemoryCache();

        return services;
    }

    /// <summary>
    /// 添加 JwtBearer 设置。
    /// </summary>
    /// <remarks>适用于只使用工具类。</remarks>
    /// <param name="services">要添加服务的服务集合。</param>
    /// <param name="optionAction">JWT 配置操作。</param>
    /// <returns>返回 <paramref name="services"/>，便于链式调用。</returns>
    public static IServiceCollection AddJwtBearerSetting(this IServiceCollection services,
        Action<JWTSettingsOptions> optionAction)
    {
        services.Configure(optionAction);

        var jwtSettings = new JWTSettingsOptions();
        optionAction.Invoke(jwtSettings);

        Penetrates.JWTSettings = jwtSettings.LoadPostConfigure();

        // 未配置 Redis 等 IDistributedCache 实现时提供进程内回退。
        services.AddDistributedMemoryCache();

        return services;
    }

    /// <summary>
    /// 添加 JwtBearer 授权。
    /// </summary>
    /// <remarks>适用于自定义验证。</remarks>
    /// <param name="services">要添加服务的服务集合。</param>
    /// <param name="configuration">用于读取模块设置的配置。</param>
    /// <param name="section">JSON 配置文件节点的 Key 默认值：JWTSettings。</param>
    /// <returns>返回 <paramref name="services"/>，便于链式调用。</returns>
    public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration,
        string section = "JWTSettings")
    {
        Debugging.Info("Registering jwt bearer......");

        services.AddJwtBearerSetting(configuration, section);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = JwtBearerUtil.CreateTokenValidationParameters(Penetrates.JWTSettings);

                // 保留调用方已配置的 Token 提取逻辑，并确保其优先执行。
                var onMessageReceived = options.Events.OnMessageReceived;
                options.Events.OnMessageReceived = async context =>
                {
                    await onMessageReceived(context);

                    if (!string.IsNullOrEmpty(context.Token) || !context.Request.Path.StartsWithSegments("/hubs"))
                    {
                        return;
                    }

                    // 仅补充从标准 access_token 查询参数提取 Token，不改变 Hub 端点的授权要求。
                    var accessToken = context.Request.Query["access_token"]
                        .ToString();
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }
                };
            });

        return services;
    }

    /// <summary>
    /// 添加 JwtBearer 授权。
    /// </summary>
    /// <remarks>适用于自定义验证。</remarks>
    /// <param name="services">要添加服务的服务集合。</param>
    /// <param name="optionAction">JWT 配置操作。</param>
    /// <returns>返回 <paramref name="services"/>，便于链式调用。</returns>
    public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services,
        Action<JWTSettingsOptions> optionAction)
    {
        Debugging.Info("Registering jwt bearer......");

        services.AddJwtBearerSetting(optionAction);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = JwtBearerUtil.CreateTokenValidationParameters(Penetrates.JWTSettings);

                // 保留调用方已配置的 Token 提取逻辑，并确保其优先执行。
                var onMessageReceived = options.Events.OnMessageReceived;
                options.Events.OnMessageReceived = async context =>
                {
                    await onMessageReceived(context);

                    if (!string.IsNullOrEmpty(context.Token) || !context.Request.Path.StartsWithSegments("/hubs"))
                    {
                        return;
                    }

                    // 仅补充从标准 access_token 查询参数提取 Token，不改变 Hub 端点的授权要求。
                    var accessToken = context.Request.Query["access_token"]
                        .ToString();
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }
                };
            });

        return services;
    }

    /// <summary>
    /// 添加 JwtBearer 服务。
    /// </summary>
    /// <param name="services">要添加服务的服务集合。</param>
    /// <param name="configuration">用于读取模块设置的配置。</param>
    /// <param name="section">JSON 配置文件节点的 Key 默认值：JWTSettings。</param>
    /// <returns>返回 <paramref name="services"/>，便于链式调用。</returns>
    public static IServiceCollection AddJwtBearer(this IServiceCollection services, IConfiguration configuration,
        string section = "JWTSettings")
    {
        Debugging.Info("Registering jwt bearer......");

        services.AddJwtBearerSetting(configuration, section);

        // 解析当前请求使用的 JWT 验证处理器。
        var jwtBearerHandle =
            MAppContext.EffectiveTypes.FirstOrDefault(f => typeof(IJwtBearerHandle).IsAssignableFrom(f) && !f.IsInterface);

        if (jwtBearerHandle != null)
        {
            // JWT 验证处理器按请求作用域解析，避免跨请求共享状态。
            services.AddScoped(typeof(IJwtBearerHandle), jwtBearerHandle);
        }

        // 注册授权策略提供器
        services.TryAddSingleton<IAuthorizationPolicyProvider, AppAuthorizationPolicyProvider>();

        // 注册策略授权处理程序
        services.TryAddSingleton<IAuthorizationHandler, AppAuthorizationHandler>();

        // 未显式标记匿名访问的端点统一要求授权。
        if (Penetrates.JWTSettings.Enable.HasValue && Penetrates.JWTSettings.Enable.Value)
        {
            services.Configure<MvcOptions>(options => { options.Filters.Add(new AuthorizeFilter()); });
        }

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = JwtBearerUtil.CreateTokenValidationParameters(Penetrates.JWTSettings);

                // 保留调用方已配置的 Token 提取逻辑，并确保其优先执行。
                var onMessageReceived = options.Events.OnMessageReceived;
                options.Events.OnMessageReceived = async context =>
                {
                    await onMessageReceived(context);

                    if (!string.IsNullOrEmpty(context.Token) || !context.Request.Path.StartsWithSegments("/hubs"))
                    {
                        return;
                    }

                    // 仅补充从标准 access_token 查询参数提取 Token，不改变 Hub 端点的授权要求。
                    var accessToken = context.Request.Query["access_token"]
                        .ToString();
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }
                };
            });

        return services;
    }

    /// <summary>
    /// 添加 JwtBearer 服务。
    /// </summary>
    /// <param name="services">要添加服务的服务集合。</param>
    /// <param name="optionAction">JWT 配置操作。</param>
    /// <returns>返回 <paramref name="services"/>，便于链式调用。</returns>
    public static IServiceCollection AddJwtBearer(this IServiceCollection services, Action<JWTSettingsOptions> optionAction)
    {
        services.AddJwtBearerSetting(optionAction);

        // 解析当前请求使用的 JWT 验证处理器。
        var jwtBearerHandle =
            MAppContext.EffectiveTypes.FirstOrDefault(f => typeof(IJwtBearerHandle).IsAssignableFrom(f) && !f.IsInterface);

        if (jwtBearerHandle != null)
        {
            // JWT 验证处理器按请求作用域解析，避免跨请求共享状态。
            services.AddScoped(typeof(IJwtBearerHandle), jwtBearerHandle);
        }

        // 注册授权策略提供器
        services.TryAddSingleton<IAuthorizationPolicyProvider, AppAuthorizationPolicyProvider>();

        // 注册策略授权处理程序
        services.TryAddSingleton<IAuthorizationHandler, AppAuthorizationHandler>();

        // 未显式标记匿名访问的端点统一要求授权。
        if (Penetrates.JWTSettings.Enable.HasValue && Penetrates.JWTSettings.Enable.Value)
        {
            services.Configure<MvcOptions>(options => { options.Filters.Add(new AuthorizeFilter()); });
        }

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = JwtBearerUtil.CreateTokenValidationParameters(Penetrates.JWTSettings);

                // 保留调用方已配置的 Token 提取逻辑，并确保其优先执行。
                var onMessageReceived = options.Events.OnMessageReceived;
                options.Events.OnMessageReceived = async context =>
                {
                    await onMessageReceived(context);

                    if (!string.IsNullOrEmpty(context.Token) || !context.Request.Path.StartsWithSegments("/hubs"))
                    {
                        return;
                    }

                    // 仅补充从标准 access_token 查询参数提取 Token，不改变 Hub 端点的授权要求。
                    var accessToken = context.Request.Query["access_token"]
                        .ToString();
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }
                };
            });

        return services;
    }
}
