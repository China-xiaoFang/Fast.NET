using System;
using System.Threading;

namespace Fast.EventBus.Sources;

/// <summary>
/// 事件源（事件承载对象）依赖接口
/// </summary>
public interface IEventSource
{
    /// <summary>
    /// 事件 Id
    /// </summary>
    string EventId { get; }

    /// <summary>
    /// 事件承载（携带）数据
    /// </summary>
    object Payload { get; }

    /// <summary>
    /// 事件创建时间
    /// </summary>
    DateTime CreatedTime { get; }

    /// <summary>
    /// 取消任务 Token
    /// </summary>
    /// <remarks>用于取消本次消息处理</remarks>
    CancellationToken CancellationToken { get; }
}