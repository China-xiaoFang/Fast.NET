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

// ReSharper disable once CheckNamespace

namespace Fast.EventBus;

/// <summary>
/// <see cref="ChannelEventPublisher"/> 基于内存通道事件发布者（默认实现）
/// </summary>
internal sealed class ChannelEventPublisher : IEventPublisher
{
    /// <summary>
    /// 事件源存储器
    /// </summary>
    private readonly IEventSourceStorer _eventSourceStorer;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eventSourceStorer">事件源存储器</param>
    public ChannelEventPublisher(IEventSourceStorer eventSourceStorer)
    {
        _eventSourceStorer = eventSourceStorer;
    }

    /// <summary>
    /// 发布一条消息
    /// </summary>
    /// <param name="eventSource">事件源</param>
    /// <returns><see cref="Task"/> 实例</returns>
    public async Task PublishAsync(IEventSource eventSource)
    {
        await _eventSourceStorer.WriteAsync(eventSource, eventSource.CancellationToken);
    }

    /// <summary>
    /// 延迟发布一条消息
    /// </summary>
    /// <param name="eventSource">事件源</param>
    /// <param name="delay">延迟数（毫秒）</param>
    /// <returns><see cref="Task"/> 实例</returns>
    public Task PublishDelayAsync(IEventSource eventSource, long delay)
    {
        // 创建新线程
        Task.Factory.StartNew(async () =>
        {
            // 延迟 delay 毫秒
            await Task.Delay(TimeSpan.FromMilliseconds(delay), eventSource.CancellationToken);

            await _eventSourceStorer.WriteAsync(eventSource, eventSource.CancellationToken);
        }, eventSource.CancellationToken);

        return Task.CompletedTask;
    }

    /// <summary>
    /// 发布一条消息
    /// </summary>
    /// <param name="eventId">事件 Id</param>
    /// <param name="payload">事件承载（携带）数据</param>
    /// <param name="cancellationToken"> 取消任务 Token</param>
    /// <returns></returns>
    public async Task PublishAsync(string eventId, object payload = null, CancellationToken cancellationToken = default)
    {
        await PublishAsync(new ChannelEventSource(eventId, payload, cancellationToken));
    }

    /// <summary>
    /// 发布一条消息
    /// </summary>
    /// <param name="eventId">事件 Id</param>
    /// <param name="payload">事件承载（携带）数据</param>
    /// <param name="cancellationToken"> 取消任务 Token</param>
    /// <returns></returns>
    public async Task PublishAsync(Enum eventId, object payload = null, CancellationToken cancellationToken = default)
    {
        await PublishAsync(new ChannelEventSource(eventId, payload, cancellationToken));
    }

    /// <summary>
    /// 延迟发布一条消息
    /// </summary>
    /// <param name="eventId">事件 Id</param>
    /// <param name="delay">延迟数（毫秒）</param>
    /// <param name="payload">事件承载（携带）数据</param>
    /// <param name="cancellationToken"> 取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    public async Task PublishDelayAsync(string eventId, long delay, object payload = null,
        CancellationToken cancellationToken = default)
    {
        await PublishDelayAsync(new ChannelEventSource(eventId, payload, cancellationToken), delay);
    }

    /// <summary>
    /// 延迟发布一条消息
    /// </summary>
    /// <param name="eventId">事件 Id</param>
    /// <param name="delay">延迟数（毫秒）</param>
    /// <param name="payload">事件承载（携带）数据</param>
    /// <param name="cancellationToken"> 取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    public async Task PublishDelayAsync(Enum eventId, long delay, object payload = null,
        CancellationToken cancellationToken = default)
    {
        await PublishDelayAsync(new ChannelEventSource(eventId, payload, cancellationToken), delay);
    }
}