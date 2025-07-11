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

// ReSharper disable once CheckNamespace
namespace Fast.JwtBearer;

/// <summary>
/// <see cref="IServiceCollection"/> 动态Api 拓展类
/// </summary>
[SuppressSniffer]
public static class IServiceCollectionExtension
{
    /// <summary>
    /// 添加JwtBearer设置
    /// </summary>
    /// <remarks>适用于只使用工具类</remarks>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    /// <param name="section">
    /// <see cref="string"/>
    /// <para>Json配置文件节点的Key</para>
    /// <para>默认值：JWTSettings</para>
    /// </param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddJwtBearerSetting(this IServiceCollection services, IConfiguration configuration,
        string section = "JWTSettings")
    {
        // 配置验证
        services.AddConfigurableOptions<JWTSettingsOptions>(section);

        Penetrates.JWTSettings = configuration.GetSection(section)
            .Get<JWTSettingsOptions>()
            .LoadPostConfigure();

        return services;
    }

    /// <summary>
    /// 添加JwtBearer设置
    /// </summary>
    /// <remarks>适用于只使用工具类</remarks>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="optionAction"><see cref="Action{T}"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddJwtBearerSetting(this IServiceCollection services,
        Action<JWTSettingsOptions> optionAction)
    {
        // 配置验证
        services.Configure(optionAction);

        var jwtSettings = new JWTSettingsOptions();
        optionAction.Invoke(jwtSettings);

        Penetrates.JWTSettings = jwtSettings;

        return services;
    }

    /// <summary>
    /// 添加JwtBearer授权
    /// </summary>
    /// <remarks>适用于自定义验证</remarks>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    /// <param name="section">
    /// <see cref="string"/>
    /// <para>Json配置文件节点的Key</para>
    /// <para>默认值：JWTSettings</para>
    /// </param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration,
        string section = "JWTSettings")
    {
        Debugging.Info("Registering jwt bearer......");

        services.AddJwtBearerSetting(configuration, section);

        // 添加默认授权
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = JwtBearerUtil.CreateTokenValidationParameters(Penetrates.JWTSettings);
            });

        return services;
    }

    /// <summary>
    /// 添加JwtBearer授权
    /// </summary>
    /// <remarks>适用于自定义验证</remarks>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="optionAction"><see cref="Action{T}"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services,
        Action<JWTSettingsOptions> optionAction)
    {
        Debugging.Info("Registering jwt bearer......");

        services.AddJwtBearerSetting(optionAction);

        // 添加默认授权
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = JwtBearerUtil.CreateTokenValidationParameters(Penetrates.JWTSettings);
            });

        return services;
    }

    /// <summary>
    /// 添加JwtBearer服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    /// <param name="section">
    /// <see cref="string"/>
    /// <para>Json配置文件节点的Key</para>
    /// <para>默认值：JWTSettings</para>
    /// </param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddJwtBearer(this IServiceCollection services, IConfiguration configuration,
        string section = "JWTSettings")
    {
        Debugging.Info("Registering jwt bearer......");

        // 配置验证
        services.AddConfigurableOptions<JWTSettingsOptions>(section);

        Penetrates.JWTSettings = configuration.GetSection(section)
            .Get<JWTSettingsOptions>()
            .LoadPostConfigure();

        // 查找Jwt验证提供器实现类
        var jwtBearerHandle =
            MAppContext.EffectiveTypes.FirstOrDefault(f => typeof(IJwtBearerHandle).IsAssignableFrom(f) && !f.IsInterface);

        if (jwtBearerHandle != null)
        {
            // 注册Jwt验证提供器实现类，这里使用作用域注入
            services.AddScoped(typeof(IJwtBearerHandle), jwtBearerHandle);
        }

        // 注册授权策略提供器
        services.TryAddSingleton<IAuthorizationPolicyProvider, AppAuthorizationPolicyProvider>();

        // 注册策略授权处理程序
        services.TryAddSingleton<IAuthorizationHandler, AppAuthorizationHandler>();

        //启用全局授权
        if (Penetrates.JWTSettings.Enable.HasValue && Penetrates.JWTSettings.Enable.Value)
        {
            services.Configure<MvcOptions>(options => { options.Filters.Add(new AuthorizeFilter()); });
        }

        // 添加默认授权
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = JwtBearerUtil.CreateTokenValidationParameters(Penetrates.JWTSettings);
            });

        return services;
    }

    /// <summary>
    /// 添加JwtBearer服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="optionAction"><see cref="Action{T}"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddJwtBearer(this IServiceCollection services, Action<JWTSettingsOptions> optionAction)
    {
        // 配置验证
        services.Configure(optionAction);

        var jwtSettings = new JWTSettingsOptions();
        optionAction.Invoke(jwtSettings);

        Penetrates.JWTSettings = jwtSettings;

        // 查找Jwt验证提供器实现类
        var jwtBearerHandle =
            MAppContext.EffectiveTypes.FirstOrDefault(f => typeof(IJwtBearerHandle).IsAssignableFrom(f) && !f.IsInterface);

        if (jwtBearerHandle != null)
        {
            // 注册Jwt验证提供器实现类，这里使用作用域注入
            services.AddScoped(typeof(IJwtBearerHandle), jwtBearerHandle);
        }

        // 注册授权策略提供器
        services.TryAddSingleton<IAuthorizationPolicyProvider, AppAuthorizationPolicyProvider>();

        // 注册策略授权处理程序
        services.TryAddSingleton<IAuthorizationHandler, AppAuthorizationHandler>();

        //启用全局授权
        if (Penetrates.JWTSettings.Enable.HasValue && Penetrates.JWTSettings.Enable.Value)
        {
            services.Configure<MvcOptions>(options => { options.Filters.Add(new AuthorizeFilter()); });
        }

        // 添加默认授权
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = JwtBearerUtil.CreateTokenValidationParameters(Penetrates.JWTSettings);
            });

        return services;
    }
}