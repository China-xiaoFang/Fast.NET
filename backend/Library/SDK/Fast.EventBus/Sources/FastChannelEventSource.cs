using System;
using System.Text.Json.Serialization;
using System.Threading;
using Fast.EventBus.Extensions;

namespace Fast.EventBus.Sources;

/// <summary>
/// Fast.NET 自定义事件总线载体
/// </summary>
public sealed class FastChannelEventSource : IEventSource
{
    /// <summary>
    /// 事件Id
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
    /// 取消任务Token
    /// </summary>
    [JsonIgnore]
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// 事件执行租户Id
    /// </summary>
    public long? TenantId { get; set; }

    public FastChannelEventSource()
    {
    }

    public FastChannelEventSource(string eventId)
    {
        EventId = eventId;
    }

    public FastChannelEventSource(string eventId, object payload) : this(eventId)
    {
        Payload = payload;
    }

    public FastChannelEventSource(string eventId, object payload, CancellationToken cancellationToken) : this(eventId, payload)
    {
        CancellationToken = cancellationToken;
    }

    public FastChannelEventSource(Enum eventId) : this(eventId.ParseToString())
    {
    }

    public FastChannelEventSource(Enum eventId, object payload) : this(eventId.ParseToString(), payload)
    {
    }

    public FastChannelEventSource(Enum eventId, object payload, CancellationToken cancellationToken) : this(
        eventId.ParseToString(), payload, cancellationToken)
    {
    }

    public FastChannelEventSource(string eventId, long? tenantId)
    {
        EventId = eventId;
        TenantId = tenantId;
    }

    public FastChannelEventSource(string eventId, long? tenantId, object payload) : this(eventId, tenantId)
    {
        Payload = payload;
    }

    public FastChannelEventSource(string eventId, long? tenantId, object payload, CancellationToken cancellationToken) : this(
        eventId, tenantId, payload)
    {
        CancellationToken = cancellationToken;
    }

    public FastChannelEventSource(Enum eventId, long? tenantId) : this(eventId.ParseToString(), tenantId)
    {
    }

    public FastChannelEventSource(Enum eventId, long? tenantId, object payload) : this(eventId.ParseToString(), tenantId, payload)
    {
    }

    public FastChannelEventSource(Enum eventId, long? tenantId, object payload, CancellationToken cancellationToken) : this(
        eventId.ParseToString(), tenantId, payload, cancellationToken)
    {
    }
}