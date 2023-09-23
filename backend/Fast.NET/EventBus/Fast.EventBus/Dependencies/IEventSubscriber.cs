namespace Fast.EventBus.Dependencies;

/// <summary>
/// 事件订阅者依赖接口
/// </summary>
/// <remarks>
/// <para>可自定义事件处理方法，但须符合 Func{EventSubscribeExecutingContext, Task} 签名</para>
/// <para>通常只做依赖查找，不做服务调用</para>
/// </remarks>
public interface IEventSubscriber
{
    /*
     * // 事件处理程序定义规范
     * [EventSubscribe(YourEventID)]
     * public Task YourHandler(EventHandlerExecutingContext context)
     * {
     *     // To Do...
     * }
     */
}