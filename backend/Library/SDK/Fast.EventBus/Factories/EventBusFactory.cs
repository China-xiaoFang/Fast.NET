using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Fast.EventBus.Attributes;
using Fast.EventBus.Constants;
using Fast.EventBus.Contexts;
using Fast.EventBus.Sources;
using Fast.EventBus.Storers;

namespace Fast.EventBus.Factories;

/// <summary>
/// 事件总线工厂默认实现
/// </summary>
internal class EventBusFactory : IEventBusFactory
{
    /// <summary>
    /// 事件源存储器
    /// </summary>
    private readonly IEventSourceStorer _eventSourceStorer;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eventSourceStorer">事件源存储器</param>
    public EventBusFactory(IEventSourceStorer eventSourceStorer)
    {
        _eventSourceStorer = eventSourceStorer;
    }

    /// <summary>
    /// 添加事件订阅者
    /// </summary>
    /// <param name="eventId">事件 Id</param>
    /// <param name="handler">事件订阅委托</param>
    /// <param name="attribute"><see cref="EventSubscribeAttribute"/> 特性对象</param>
    /// <param name="handlerMethod"><see cref="MethodInfo"/> 对象</param>
    /// <param name="cancellationToken">取消任务 Token</param>
    /// <returns></returns>
    public async Task Subscribe(string eventId, Func<EventHandlerExecutingContext, Task> handler,
        EventSubscribeAttribute attribute = default, MethodInfo handlerMethod = default,
        CancellationToken cancellationToken = default)
    {
        // 空检查
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        await _eventSourceStorer.WriteAsync(
            new EventSubscribeOperateSource
            {
                SubscribeEventId = eventId,
                Attribute = attribute,
                Handler = handler,
                HandlerMethod = handlerMethod,
                Operate = EventSubscribeOperates.Append
            }, cancellationToken);
    }

    /// <summary>
    /// 删除事件订阅者
    /// </summary>
    /// <param name="eventId">事件 Id</param>
    /// <param name="cancellationToken">取消任务 Token</param>
    /// <returns></returns>
    public async Task Unsubscribe(string eventId, CancellationToken cancellationToken = default)
    {
        // 空检查
        if (eventId == null)
            throw new ArgumentNullException(nameof(eventId));

        await _eventSourceStorer.WriteAsync(
            new EventSubscribeOperateSource {SubscribeEventId = eventId, Operate = EventSubscribeOperates.Remove}, default);
    }
}