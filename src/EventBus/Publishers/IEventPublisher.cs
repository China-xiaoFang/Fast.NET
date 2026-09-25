// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

namespace Fast.EventBus;

/// <summary>
/// 事件发布服务依赖接口
/// </summary>
[SuppressSniffer]
public interface IEventPublisher
{
    /// <summary>
    /// 发布一条消息
    /// </summary>
    /// <param name="eventSource">要发布的事件消息</param>
    /// <returns>表示异步“发布一条消息”操作的任务</returns>
    Task PublishAsync(IEventSource eventSource);

    /// <summary>
    /// 延迟发布一条消息
    /// </summary>
    /// <param name="eventSource">要发布的事件消息</param>
    /// <param name="delay">延迟执行时长</param>
    /// <returns>表示异步“延迟发布一条消息”操作的任务</returns>
    Task PublishDelayAsync(IEventSource eventSource, long delay);

    /// <summary>
    /// 发布一条消息
    /// </summary>
    /// <param name="eventId">日志事件标识</param>
    /// <param name="payload">要写入令牌的载荷</param>
    /// <param name="cancellationToken">用于取消异步操作的令牌</param>
    /// <returns>表示异步“发布一条消息”操作的任务</returns>
    Task PublishAsync(string eventId, object payload = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布一条消息
    /// </summary>
    /// <param name="eventId">日志事件标识</param>
    /// <param name="payload">要写入令牌的载荷</param>
    /// <param name="cancellationToken">用于取消异步操作的令牌</param>
    /// <returns>表示异步“发布一条消息”操作的任务</returns>
    Task PublishAsync(Enum eventId, object payload = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 延迟发布一条消息
    /// </summary>
    /// <param name="eventId">日志事件标识</param>
    /// <param name="delay">延迟执行时长</param>
    /// <param name="payload">要写入令牌的载荷</param>
    /// <param name="cancellationToken">用于取消异步操作的令牌</param>
    /// <returns>表示异步“延迟发布一条消息”操作的任务</returns>
    Task PublishDelayAsync(string eventId, long delay, object payload = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 延迟发布一条消息
    /// </summary>
    /// <param name="eventId">日志事件标识</param>
    /// <param name="delay">延迟执行时长</param>
    /// <param name="payload">要写入令牌的载荷</param>
    /// <param name="cancellationToken">用于取消异步操作的令牌</param>
    /// <returns>表示异步“延迟发布一条消息”操作的任务</returns>
    Task PublishDelayAsync(Enum eventId, long delay, object payload = null, CancellationToken cancellationToken = default);
}
