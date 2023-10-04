﻿// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
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

// ReSharper disable once CheckNamespace
namespace Fast.EventBus;

/// <summary>
/// 基于内存通道事件发布者（默认实现）
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
    public async Task PublishAsync(string eventId, object payload = default, CancellationToken cancellationToken = default)
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
    public async Task PublishAsync(Enum eventId, object payload = default, CancellationToken cancellationToken = default)
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
    public async Task PublishDelayAsync(string eventId, long delay, object payload = default,
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
    public async Task PublishDelayAsync(Enum eventId, long delay, object payload = default,
        CancellationToken cancellationToken = default)
    {
        await PublishDelayAsync(new ChannelEventSource(eventId, payload, cancellationToken), delay);
    }
}