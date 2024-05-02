// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
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
using Fast.Swagger.Builders;
using Fast.Swagger.Internal;
using Fast.Swagger.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

// ReSharper disable once CheckNamespace
namespace Fast.Swagger;

/// <summary>
/// <see cref="IServiceCollection"/> Swagger 拓展类
/// </summary>
[SuppressSniffer]
public static class IServiceCollectionExtension
{
    /// <summary>
    /// 添加 Swagger 服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    /// <param name="section"><see cref="string"/></param>
    /// <param name="configure"></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddSwaggerDocuments(this IServiceCollection services, IConfiguration configuration,
        string section = "SwaggerSettings", Action<SwaggerGenOptions> configure = null)
    {
        Debugging.Info("Registering swagger......");

        // 配置验证
        services.AddConfigurableOptions<SwaggerSettingsOptions>(section);

        // 获取Swagger文档配置选项
        Penetrates.SwaggerSettings = configuration.GetSection(section).Get<SwaggerSettingsOptions>().LoadPostConfigure();

#if !NET5_0
        services.AddEndpointsApiExplorer();
#endif

        // 判断是否启用规范化文档
        if (Penetrates.SwaggerSettings.Enable!.Value)
        {
            // 添加Swagger生成器服务
            services.AddSwaggerGen(options => SwaggerDocumentBuilder.BuildGen(options, configure));
        }

        return services;
    }

    /// <summary>
    /// 添加 Swagger 服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="optionAction"><see cref="Action{T}"/></param>
    /// <param name="configure"></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddSwaggerDocuments(this IServiceCollection services,
        Action<SwaggerSettingsOptions> optionAction, Action<SwaggerGenOptions> configure = null)
    {
        Debugging.Info("Registering swagger......");

        services.Configure(optionAction);

        var swaggerSettings = new SwaggerSettingsOptions();
        optionAction.Invoke(swaggerSettings);

        Penetrates.SwaggerSettings = swaggerSettings;

#if !NET5_0
        services.AddEndpointsApiExplorer();
#endif

        // 判断是否启用规范化文档
        if (Penetrates.SwaggerSettings.Enable!.Value)
        {
            // 添加Swagger生成器服务
            services.AddSwaggerGen(options => SwaggerDocumentBuilder.BuildGen(options, configure));
        }

        return services;
    }
}