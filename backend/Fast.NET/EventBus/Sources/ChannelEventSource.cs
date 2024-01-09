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

using System.Text.Json.Serialization;
using Fast.EventBus.Extensions;
using Fast.IaaS;

// ReSharper disable once CheckNamespace
namespace Fast.EventBus;

/// <summary>
/// <see cref="ChannelEventSource"/> 内存通道事件源（事件承载对象）
/// </summary>
[SuppressSniffer]
public sealed class ChannelEventSource : IEventSource
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public ChannelEventSource()
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eventId">事件 Id</param>
    public ChannelEventSource(string eventId)
    {
        EventId = eventId;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eventId">事件 Id</param>
    /// <param name="payload">事件承载（携带）数据</param>
    public ChannelEventSource(string eventId, object payload) : this(eventId)
    {
        Payload = payload;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eventId">事件 Id</param>
    /// <param name="payload">事件承载（携带）数据</param>
    /// <param name="cancellationToken">取消任务 Token</param>
    public ChannelEventSource(string eventId, object payload, CancellationToken cancellationToken) : this(eventId, payload)
    {
        CancellationToken = cancellationToken;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eventId">事件 Id</param>
    public ChannelEventSource(Enum eventId) : this(eventId.EventBusToString())
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eventId">事件 Id</param>
    /// <param name="payload">事件承载（携带）数据</param>
    public ChannelEventSource(Enum eventId, object payload) : this(eventId.EventBusToString(), payload)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eventId">事件 Id</param>
    /// <param name="payload">事件承载（携带）数据</param>
    /// <param name="cancellationToken">取消任务 Token</param>
    public ChannelEventSource(Enum eventId, object payload, CancellationToken cancellationToken) : this(
        eventId.EventBusToString(), payload, cancellationToken)
    {
    }

    /// <summary>
    /// 事件 Id
    /// </summary>
    public string EventId { get; set; }

    /// <summary>
    /// 事件承载（携带）数据
    /// </summary>
    public object Payload { get; set; }

    /// <summary>
    /// 事件创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 取消任务 Token
    /// </summary>
    /// <remarks>用于取消本次消息处理</remarks>
    [JsonIgnore]
    public CancellationToken CancellationToken { get; set; }
}