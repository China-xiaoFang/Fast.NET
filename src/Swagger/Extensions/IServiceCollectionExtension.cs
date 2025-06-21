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
        Penetrates.SwaggerSettings = configuration.GetSection(section)
            .Get<SwaggerSettingsOptions>()
            .LoadPostConfigure();

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