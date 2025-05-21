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

using System.Text.Json.Serialization;

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