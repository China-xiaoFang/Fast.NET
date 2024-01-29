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

using System.Reflection;
using System.Text.Json.Serialization;
using Fast.EventBus.Constants;
using Fast.EventBus.Contexts;

namespace Fast.EventBus.Sources;

/// <summary>
/// <see cref="EventSubscribeOperateSource"/> 事件总线订阅管理事件源
/// </summary>
internal sealed class EventSubscribeOperateSource : IEventSource
{
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

    /// <summary>
    /// 事件处理程序
    /// </summary>
    internal Func<EventHandlerExecutingContext, Task> Handler { get; set; }

    /// <summary>
    /// 订阅特性
    /// </summary>
    internal EventSubscribeAttribute Attribute { get; set; }

    /// <summary>
    /// 触发的方法
    /// </summary>
    internal MethodInfo HandlerMethod { get; set; }

    /// <summary>
    /// 实际事件 Id
    /// </summary>
    internal string SubscribeEventId { get; set; }

    /// <summary>
    /// 事件订阅器操作选项
    /// </summary>
    internal EventSubscribeOperates Operate { get; set; }
}