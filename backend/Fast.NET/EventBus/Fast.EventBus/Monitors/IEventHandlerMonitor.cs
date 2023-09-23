using Fast.EventBus.Contexts;

namespace Fast.EventBus.Monitors;

/// <summary>
/// 事件处理程序监视器
/// </summary>
public interface IEventHandlerMonitor
{
    /// <summary>
    /// 事件处理程序执行前
    /// </summary>
    /// <param name="context">上下文</param>
    /// <returns><see cref="Task"/> 实例</returns>
    Task OnExecutingAsync(EventHandlerExecutingContext context);

    /// <summary>
    /// 事件处理程序执行后
    /// </summary>
    /// <param name="context">上下文</param>
    /// <returns><see cref="Task"/> 实例</returns>
    Task OnExecutedAsync(EventHandlerExecutedContext context);
}