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
/// <see cref="EventSubscribeAttribute"/> 事件处理程序特性
/// <remarks>
/// <para>作用于 <see cref="IEventSubscriber"/> 实现类实例方法</para>
/// <para>支持多个事件 Id 触发同一个事件处理程序</para>
/// </remarks>
/// </summary>
[SuppressSniffer]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class EventSubscribeAttribute : Attribute
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eventId">事件 Id</param>
    /// <remarks>只支持事件类型和 Enum 类型</remarks>
    public EventSubscribeAttribute(object eventId)
    {
        if (eventId is string eventIdStr)
        {
            EventId = eventIdStr;
        }
        else if (eventId is Enum eventIdEnum)
        {
            EventId = eventIdEnum.EventBusToString();
        }
        else
            throw new ArgumentException("Only support string or Enum data type.");
    }

    /// <summary>
    /// 事件 Id
    /// </summary>
    public string EventId { get; set; }

    /// <summary>
    /// 是否启用执行完成触发 GC 回收
    /// </summary>
    /// <remarks>bool 类型，默认为 null</remarks>
    public object GCCollect { get; set; } = null;

    /// <summary>
    /// 重试次数
    /// </summary>
    public int NumRetries { get; set; } = 0;

    /// <summary>
    /// 重试间隔时间
    /// </summary>
    /// <remarks>默认1000毫秒</remarks>
    public int RetryTimeout { get; set; } = 1000;

    /// <summary>
    /// 可以指定特定异常类型才重试
    /// </summary>
    public Type[] ExceptionTypes { get; set; }

    /// <summary>
    /// 重试失败策略配置
    /// </summary>
    public Type FallbackPolicy { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>数值越大的先执行</remarks>
    public int Order { get; set; } = 0;
}