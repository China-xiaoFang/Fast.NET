// Apache开源许可证
//
// 版权所有 © 2018-Now 小方
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

using Fast.IaaS;
using Fast.JwtBearer.Handlers;
using Fast.JwtBearer.Internal;
using Fast.JwtBearer.Options;
using Fast.JwtBearer.Providers;
using Fast.JwtBearer.Services;
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
    /// 添加JwtBearer加密解密服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    /// <param name="section"><see cref="string"/>
    /// <para>Json配置文件节点的Key</para>
    /// <remarks>默认值：JWTSettings</remarks>
    /// </param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddJwtBearerCrypto(this IServiceCollection services, IConfiguration configuration,
        string section = "JWTSettings")
    {
        // 配置验证
        services.AddConfigurableOptions<JWTSettingsOptions>(section);

        Penetrates.JWTSettings = configuration.GetSection(section).Get<JWTSettingsOptions>().LoadPostConfigure();

        // 添加加密解密服务
        services.AddSingleton<IJwtBearerCryptoService, JwtBearerCryptoService>();

        return services;
    }

    /// <summary>
    /// 添加JwtBearer加密解密服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="optionAction"><see cref="Action{T}"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddJwtBearerCrypto(this IServiceCollection services, Action<JWTSettingsOptions> optionAction)
    {
        // 配置验证
        services.Configure(optionAction);

        var jwtSettings = new JWTSettingsOptions();
        optionAction.Invoke(jwtSettings);

        Penetrates.JWTSettings = jwtSettings;

        // 添加加密解密服务
        services.AddSingleton<IJwtBearerCryptoService, JwtBearerCryptoService>();

        return services;
    }

    /// <summary>
    /// 添加JwtBearer服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    /// <param name="section"><see cref="string"/>
    /// <para>Json配置文件节点的Key</para>
    /// <remarks>默认值：JWTSettings</remarks>
    /// </param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddJwtBearer(this IServiceCollection services, IConfiguration configuration,
        string section = "JWTSettings")
    {
        Debugging.Info("Registering jwt bearer......");

        // 添加加密解密服务
        services.AddJwtBearerCrypto(configuration, section);

        // 查找Jwt验证提供器实现类
        var jwtBearerHandle =
            IaaSContext.EffectiveTypes.FirstOrDefault(f => typeof(IJwtBearerHandle).IsAssignableFrom(f) && !f.IsInterface);

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
        var authenticationBuilder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = Penetrates.CreateTokenValidationParameters(Penetrates.JWTSettings);
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
        // 添加加密解密服务
        services.AddJwtBearerCrypto(optionAction);

        // 查找Jwt验证提供器实现类
        var jwtBearerHandle =
            IaaSContext.EffectiveTypes.FirstOrDefault(f => typeof(IJwtBearerHandle).IsAssignableFrom(f) && !f.IsInterface);

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
        var authenticationBuilder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = Penetrates.CreateTokenValidationParameters(Penetrates.JWTSettings);
        });

        return services;
    }
}