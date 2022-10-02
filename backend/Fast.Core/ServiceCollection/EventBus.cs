using Fast.Core.EventSubscriber;
using Furion.Logging;

namespace Fast.Core.ServiceCollection;

/// <summary>
/// 事件总线
/// </summary>
public static class EventBus
{
    /// <summary>
    /// 添加事件总线服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="isRun"></param>
    public static void AddEventBusService(this IServiceCollection services, bool isRun = true)
    {
        if (isRun)
        {
            services.AddEventBus(eventBuilder =>
            {
                // Register as a Log subscriber.
                eventBuilder.AddSubscriber<LogEventSubscriber>();
                // Subscribing to EventBus caught no exception.
                eventBuilder.UnobservedTaskExceptionHandler = (sender, eventArgs) =>
                {
                    Log.Error(eventArgs.Exception.ToString());
                };
            });
        }
    }
}