using System.Reflection;
using Fast.EventBus.Attributes;
using Fast.EventBus.Contexts;

namespace Fast.EventBus.Factories;

/// <summary>
/// 事件总线工厂接口
/// </summary>
public interface IEventBusFactory
{
    /// <summary>
    /// 添加事件订阅者
    /// </summary>
    /// <param name="eventId">事件 Id</param>
    /// <param name="handler">事件订阅委托</param>
    /// <param name="attribute"><see cref="EventSubscribeAttribute"/> 特性对象</param>
    /// <param name="handlerMethod"><see cref="MethodInfo"/> 对象</param>
    /// <param name="cancellationToken">取消任务 Token</param>
    /// <returns></returns>
    Task Subscribe(string eventId, Func<EventHandlerExecutingContext, Task> handler, EventSubscribeAttribute attribute = default,
        MethodInfo handlerMethod = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除事件订阅者
    /// </summary>
    /// <param name="eventId">事件 Id</param>
    /// <param name="cancellationToken">取消任务 Token</param>
    /// <returns></returns>
    Task Unsubscribe(string eventId, CancellationToken cancellationToken = default);
}