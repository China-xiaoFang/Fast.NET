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
    /// 添加缓存服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="section">
    /// <see cref="string"/>
    /// <para>Json配置文件节点的Key</para>
    /// <para>默认值：RedisSettings</para>
    /// </param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddCache(this IServiceCollection services, string section = "RedisSettings")
    {
        // 配置验证
        services.AddConfigurableOptions<RedisSettingsOptions>(section);

        // 添加默认缓存服务
        services.AddSingleton<ICache, Cache>();

        // 查找所有集成了 ICacheContextLocator 类的缓存上下文定位器
        var cacheContextLocatorType = typeof(ICacheContextLocator);
        var cacheContextLocatorTypes =
            MAppContext.EffectiveTypes.Where(wh => cacheContextLocatorType.IsAssignableFrom(wh) && !wh.IsInterface);

        // 循环所有上下文，注册单例服务
        foreach (var contextLocatorType in cacheContextLocatorTypes)
        {
            services.AddSingleton(typeof(ICache<>).MakeGenericType(contextLocatorType),
                typeof(Cache<>).MakeGenericType(contextLocatorType));
        }

        return services;
    }

    /// <summary>
    /// 添加缓存服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="optionAction"><see cref="Action{T}"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddCache(this IServiceCollection services, Action<List<RedisSettingsOptions>> optionAction)
    {
        // 配置验证
        services.Configure(optionAction);

        // 添加默认缓存服务
        services.AddSingleton<ICache, Cache>();

        // 查找所有集成了 ICacheContextLocator 类的缓存上下文定位器
        var cacheContextLocatorType = typeof(ICacheContextLocator);
        var cacheContextLocatorTypes =
            MAppContext.EffectiveTypes.Where(wh => cacheContextLocatorType.IsAssignableFrom(wh) && !wh.IsInterface);

        // 循环所有上下文，注册单例服务
        foreach (var contextLocatorType in cacheContextLocatorTypes)
        {
            services.AddSingleton(typeof(ICache<>).MakeGenericType(contextLocatorType),
                typeof(Cache<>).MakeGenericType(contextLocatorType));
        }

        return services;
    }
}