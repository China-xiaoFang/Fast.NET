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

using Fast.Cache.Implements;
using Fast.Cache.Options;
using Fast.IaaS;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Fast.Cache;

/// <summary>
/// <see cref="IServiceCollection"/> Cache 拓展类
/// </summary>
[SuppressSniffer]
public static class IServiceCollectionExtension
{
    /// <summary>
    /// 添加Redis缓存服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="section"><see cref="string"/>
    /// <para>Json配置文件节点的Key</para>
    /// <remarks>默认值：RedisSettings</remarks>
    /// </param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddCache(this IServiceCollection services, string section = "RedisSettings")
    {
        Debugging.Info("Registering cache......");

        // 配置验证
        services.AddConfigurableOptions<List<RedisSettingsOptions>>(section);

        // 添加默认缓存服务
        services.AddSingleton<ICache, Implements.Cache>();

        // 查找所有集成了 ICacheContextLocator 类的缓存上下文定位器
        var cacheContextLocatorType = typeof(ICacheContextLocator);
        var cacheContextLocatorTypes =
            IaaSContext.EffectiveTypes.Where(wh => cacheContextLocatorType.IsAssignableFrom(wh) && !wh.IsInterface);

        // 循环所有上下文，注册单例服务
        foreach (var contextLocatorType in cacheContextLocatorTypes)
        {
            services.AddSingleton(typeof(ICache<>).MakeGenericType(contextLocatorType),
                typeof(Cache<>).MakeGenericType(contextLocatorType));
        }

        return services;
    }

    /// <summary>
    /// 添加Redis缓存服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="optionAction"><see cref="Action{T}"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddCache(this IServiceCollection services, Action<RedisSettingsOptions> optionAction)
    {
        Debugging.Info("Registering cache......");

        // 配置验证
        services.Configure(optionAction);

        // 添加默认缓存服务
        services.AddSingleton<ICache, Implements.Cache>();

        // 查找所有集成了 ICacheContextLocator 类的缓存上下文定位器
        var cacheContextLocatorType = typeof(ICacheContextLocator);
        var cacheContextLocatorTypes =
            IaaSContext.EffectiveTypes.Where(wh => cacheContextLocatorType.IsAssignableFrom(wh) && !wh.IsInterface);

        // 循环所有上下文，注册单例服务
        foreach (var contextLocatorType in cacheContextLocatorTypes)
        {
            services.AddSingleton(typeof(ICache<>).MakeGenericType(contextLocatorType),
                typeof(Cache<>).MakeGenericType(contextLocatorType));
        }

        return services;
    }
}