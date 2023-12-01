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

using Fast.IaaS;
using Fast.JwtBearer.Handlers;
using Fast.JwtBearer.Internal;
using Fast.JwtBearer.Options;
using Fast.JwtBearer.Providers;
using Fast.JwtBearer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fast.JwtBearer.Extensions;

/// <summary>
/// <see cref="IServiceCollection"/> JWT 授权服务拓展类
/// </summary>
public static class JwtBearerIServiceCollectionExtension
{
    /// <summary>
    /// 添加 JWT 授权
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"><see cref="IConfiguration"/> 配置项，建议通过框架自带的 App.Configuration 传入，否则会在内部自动解析 IConfiguration 性能会很低</param>
    /// <param name="enableGlobalAuthorize"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddJwt(this IServiceCollection services, IConfiguration configuration = null,
        bool enableGlobalAuthorize = false)
    {
        // 处理 IConfiguration
        if (configuration == null)
        {
            // 构建新的服务对象
            var serviceProvider = services.BuildServiceProvider();
            configuration = serviceProvider.GetService<IConfiguration>();
            // 释放服务对象
            serviceProvider.Dispose();
        }

        // 配置验证
        services.AddOptions<JWTSettingsOptions>().BindConfiguration("JWTSettings").ValidateDataAnnotations();

        // 添加加密解密服务
        services.AddSingleton<IJwtBearerCryptoService, JwtBearerCryptoService>();

        // 查找Jwt验证提供器实现类
        var jwtBearerHandle =
            FastContext.EffectiveTypes.FirstOrDefault(f => typeof(IJwtBearerHandle).IsAssignableFrom(f) && !f.IsInterface);

        if (jwtBearerHandle != null)
        {
            // 注册Jwt验证提供器实现类
            services.AddSingleton(typeof(IJwtBearerHandle), jwtBearerHandle);
        }

        // 注册授权策略提供器
        services.TryAddSingleton<IAuthorizationPolicyProvider, AppAuthorizationPolicyProvider>();

        // 注册策略授权处理程序
        services.TryAddSingleton<IAuthorizationHandler, AppAuthorizationHandler>();

        //启用全局授权
        if (enableGlobalAuthorize)
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
            options.TokenValidationParameters =
                Penetrates.CreateTokenValidationParameters(configuration.GetSection("JWTSettings").Get<JWTSettingsOptions>());
        });

        return authenticationBuilder;
    }
}