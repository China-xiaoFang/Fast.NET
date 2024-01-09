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

using Fast.EventBus.Factories;
using Fast.EventBus.HostedServices;
using Fast.EventBus.Interfaces;
using Fast.EventBus.Monitors;
using Fast.EventBus.Policies;
using Fast.EventBus.Publishers;
using Fast.EventBus.Storers;
using Fast.IaaS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.EventBus.Injections;

/// <summary>
/// <see cref="EventBusInjection"/> 统一返回注入
/// </summary>
public class EventBusInjection : IHostingStartup
{
    /// <summary>
    /// 排序
    /// </summary>
#pragma warning disable CA1822
    public int Order => 69922;
#pragma warning restore CA1822

    /// <summary>
    /// 配置
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((hostContext, services) =>
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

            // 查找所有继承了 IEventSubscriber 的类
            var iEventSubscriberTypes =
                IaaSContext.EffectiveTypes.Where(wh => typeof(IEventSubscriber).IsAssignableFrom(wh) && !wh.IsInterface);

            // 注册事件订阅者
            foreach (var iEventSubscriberType in iEventSubscriberTypes)
            {
                services.AddSingleton(typeof(IEventSubscriber), iEventSubscriberType);
            }

            // 查找继承了 IEventHandlerMonitor 的类
            var iEventHandlerMonitorType =
                IaaSContext.EffectiveTypes.FirstOrDefault(f =>
                    typeof(IEventHandlerMonitor).IsAssignableFrom(f) && !f.IsInterface);

            // 注册事件监视器
            if (iEventHandlerMonitorType != null)
            {
                services.AddSingleton(typeof(IEventHandlerMonitor), iEventHandlerMonitorType);
            }

            // 查找继承了 IEventFallbackPolicy 的类
            var iEventFallbackPolicyType =
                IaaSContext.EffectiveTypes.FirstOrDefault(f =>
                    typeof(IEventFallbackPolicy).IsAssignableFrom(f) && !f.IsInterface);

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
                eventBusHostedService.UnobservedTaskException += (obj, args) => { };

                return eventBusHostedService;
            });
        });
    }
}