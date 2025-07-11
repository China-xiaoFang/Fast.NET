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

using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Fast.EventBus;

/// <summary>
/// <see cref="IServiceCollection"/> 动态Api 拓展类
/// </summary>
[SuppressSniffer]
public static class IServiceCollectionExtension
{
    /// <summary>
    /// 添加事件总线服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddEventBus(this IServiceCollection services)
    {
        Debugging.Info("Registering event bus......");

        // 创建默认内存通道事件源对象，超过 n 条待处理消息，第 n+1 条将进入等待，默认为 3000
        var defaultStorerOfChannel = new ChannelEventSourceStorer(3000);

        // 注册后台任务队列接口/实例为单例，采用工厂方式创建
        services.AddSingleton<IEventSourceStorer>(_ => { return defaultStorerOfChannel; });

        // 注册默认内存通道事件发布者
        services.AddSingleton<IEventPublisher, ChannelEventPublisher>();

        // 注册事件总线工厂
        services.AddSingleton<IEventBusFactory, EventBusFactory>();

        #region 构建事件总线服务

        var entryAssemblyType = MAppContext.EffectiveTypes.ToList();

        // 查找所有继承了 IEventSubscriber 的类
        var iEventSubscriberTypes =
            entryAssemblyType.Where(wh => typeof(IEventSubscriber).IsAssignableFrom(wh) && !wh.IsInterface);

        // 注册事件订阅者
        foreach (var iEventSubscriberType in iEventSubscriberTypes)
        {
            services.AddSingleton(typeof(IEventSubscriber), iEventSubscriberType);
        }

        // 查找继承了 IEventHandlerMonitor 的类
        var iEventHandlerMonitorType =
            entryAssemblyType.FirstOrDefault(f => typeof(IEventHandlerMonitor).IsAssignableFrom(f) && !f.IsInterface);

        // 注册事件监视器
        if (iEventHandlerMonitorType != null)
        {
            services.AddSingleton(typeof(IEventHandlerMonitor), iEventHandlerMonitorType);
        }

        // 查找继承了 IEventFallbackPolicy 的类
        var iEventFallbackPolicyType =
            entryAssemblyType.FirstOrDefault(f => typeof(IEventFallbackPolicy).IsAssignableFrom(f) && !f.IsInterface);

        // 注册事件重试策略
        if (iEventFallbackPolicyType != null)
        {
            services.AddSingleton(typeof(IEventFallbackPolicy), iEventFallbackPolicyType);
        }

        #endregion

        // 通过工厂模式创建
        services.AddHostedService(serviceProvider =>
        {
            // 创建事件总线后台服务对象
            var eventBusHostedService = ActivatorUtilities.CreateInstance<EventBusHostedService>(serviceProvider);

            // 订阅未察觉任务异常事件
            eventBusHostedService.UnobservedTaskException += (_, _) => { };

            return eventBusHostedService;
        });

        return services;
    }
}