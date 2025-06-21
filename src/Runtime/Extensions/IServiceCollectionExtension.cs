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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Fast.Runtime;

/// <summary>
/// <see cref="IServiceCollection"/> 拓展类
/// </summary>
[SuppressSniffer]
public static class IServiceCollectionExtension
{
    /// <summary>
    /// 添加选项配置
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="path"><see cref="string"/> 配置中对应的Key</param>
    /// <returns></returns>
    public static IServiceCollection AddConfigurableOptions<TOptions>(this IServiceCollection services, string path = null)
        where TOptions : class, new()
    {
        // 获取配置选项名称
        path ??= MAppContext.GetOptionName<TOptions>();

        // 配置验证
        var optionsConfigure = services.AddOptions<TOptions>()
            .BindConfiguration(path, options =>
            {
                // 绑定私有变量
                options.BindNonPublicProperties = true;
            })
            .ValidateDataAnnotations();

        // 获取类型
        var optionsType = typeof(TOptions);

        // 复杂后期配置
        var postConfigureInterface = optionsType.GetInterfaces()
            .FirstOrDefault(f => typeof(IPostConfigure).IsAssignableFrom(f));

        if (postConfigureInterface != null)
        {
            var postConfigureMethod = optionsType.GetMethod(nameof(IPostConfigure.PostConfigure));

            if (postConfigureMethod != null)
            {
                optionsConfigure.PostConfigure(options => postConfigureMethod.Invoke(options, Array.Empty<object>()));
            }
        }

        return services;
    }

    /// <summary>
    /// 注册 Mvc 过滤器
    /// </summary>
    /// <typeparam name="TFilter"></typeparam>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="configure"></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddMvcFilter<TFilter>(this IServiceCollection services, Action<MvcOptions> configure = null)
        where TFilter : IFilterMetadata
    {
        services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add<TFilter>();

            // 其他额外配置
            configure?.Invoke(options);
        });

        return services;
    }

    /// <summary>
    /// 注册 Mvc 过滤器
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="filter"></param>
    /// <param name="configure"></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddMvcFilter(this IServiceCollection services, IFilterMetadata filter,
        Action<MvcOptions> configure = null)
    {
        services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add(filter);

            // 其他额外配置
            configure?.Invoke(options);
        });

        return services;
    }
}