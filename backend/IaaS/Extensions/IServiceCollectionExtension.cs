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

using Microsoft.AspNetCore.Hosting;
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

    /// <summary>
    /// 添加启动过滤器
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns></returns>
    public static IServiceCollection AddStartupFilter(this IServiceCollection services)
    {
        var iStartupFilterType = typeof(IStartupFilter);

        // 查找所有继承了 IStartupFilter 的类型
        var iStartupFilterTypes = IaaSContext.EffectiveTypes.Where(wh =>
            iStartupFilterType.IsAssignableFrom(wh) && wh.IsClass && !wh.IsInterface && !wh.IsAbstract).Select(sl =>
        {
            var iStartupFilter = Activator.CreateInstance(sl) as IStartupFilter;

            // 默认为 -1；
            var order = -1;
            // 尝试获取Order值
            var orderProperty = sl.GetProperty("Order");

            if (orderProperty != null && orderProperty.PropertyType == typeof(int))
            {
                var orderVal = orderProperty.GetValue(iStartupFilter)?.ToString();
                if (!orderVal.IsEmpty())
                {
                    order = orderVal.ParseToInt();
                }
            }

            return new {Type = sl, Order = order};
        }).OrderByDescending(ob => ob.Order).Select(sl => sl.Type);

        foreach (var startupFilterType in iStartupFilterTypes)
        {
            // 注册 Startup 过滤器
            services.AddTransient(typeof(IStartupFilter), startupFilterType);
        }

        return services;
    }
}