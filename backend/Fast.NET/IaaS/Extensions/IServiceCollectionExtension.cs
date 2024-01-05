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

using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

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
        path ??= IaaSContext.GetOptionName<TOptions>();

        // 配置验证
        var optionsConfigure = services.AddOptions<TOptions>().BindConfiguration(path, options =>
        {
            // 绑定私有变量
            options.BindNonPublicProperties = true;
        }).ValidateDataAnnotations();

        // 获取类型
        var optionsType = typeof(TOptions);

        // 复杂后期配置
        var postConfigureInterface = optionsType.GetInterfaces().FirstOrDefault(f => typeof(IPostConfigure).IsAssignableFrom(f));

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
}