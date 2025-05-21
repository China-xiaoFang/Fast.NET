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

using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Fast.EventBus;

/// <summary>
/// <see cref="EventHandlerWrapper"/> 事件处理程序包装类
/// </summary>
/// <remarks>主要用于主机服务启动时将所有处理程序和事件 Id 进行包装绑定</remarks>
internal sealed class EventHandlerWrapper
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eventId">事件Id</param>
    internal EventHandlerWrapper(string eventId)
    {
        EventId = eventId;
    }

    /// <summary>
    /// 事件 Id
    /// </summary>
    internal string EventId { get; set; }

    /// <summary>
    /// 事件处理程序
    /// </summary>
    internal Func<EventHandlerExecutingContext, Task> Handler { get; set; }

    /// <summary>
    /// 触发的方法
    /// </summary>
    internal MethodInfo HandlerMethod { get; set; }

    /// <summary>
    /// 订阅特性
    /// </summary>
    internal EventSubscribeAttribute Attribute { get; set; }

    /// <summary>
    /// 是否启用执行完成触发 GC 回收
    /// </summary>
    public bool GCCollect { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>数值越大的先执行</remarks>
    public int Order { get; set; } = 0;

    /// <summary>
    /// 是否符合条件执行处理程序
    /// </summary>
    /// <remarks>支持正则表达式</remarks>
    /// <param name="eventId">事件 Id</param>
    /// <returns><see cref="bool"/></returns>
    internal bool ShouldRun(string eventId)
    {
        return EventId == eventId;
    }
}