using Fast.CorsAccessor.Options;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.CorsAccessor.Extensions;

/// <summary>
/// 跨域访问服务拓展类
/// </summary>
public static class CorsAccessorIServiceCollectionExtension
{
    /// <summary>
    /// 配置跨域
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration"></param>
    /// <param name="corsOptionsHandler"></param>
    /// <param name="corsPolicyBuilderHandler"></param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddCorsAccessor(this IServiceCollection services, IConfiguration configuration,
        Action<CorsOptions> corsOptionsHandler = default, Action<CorsPolicyBuilder> corsPolicyBuilderHandler = default)
    {
        // 获取跨域配置选项
        var corsAccessorSettings = configuration.GetSection("CorsAccessorSettings").Get<CorsAccessorSettingsOptions>();

        // 加载默认构造函数
        var postConfigure = typeof(CorsAccessorSettingsOptions).GetMethod(nameof(CorsAccessorSettingsOptions.PostConfigure));

        corsAccessorSettings ??= Activator.CreateInstance<CorsAccessorSettingsOptions>();
        postConfigure?.Invoke(corsAccessorSettings, new object[] {corsAccessorSettings, configuration});

        // 添加跨域服务
        services.AddCors(options =>
        {
            // 添加策略跨域
            options.AddPolicy(corsAccessorSettings.PolicyName, builder =>
            {
                // 设置跨域策略
                corsAccessorSettings.SetCorsPolicy(builder, corsAccessorSettings);

                // 添加自定义配置
                corsPolicyBuilderHandler?.Invoke(builder);
            });

            // 添加自定义配置
            corsOptionsHandler?.Invoke(options);
        });

        return services;
    }
}