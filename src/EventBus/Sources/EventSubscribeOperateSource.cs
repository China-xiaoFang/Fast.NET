// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using System.Reflection;
using System.Text.Json.Serialization;

namespace Fast.EventBus;

/// <summary>
/// 事件总线订阅管理事件源
/// </summary>
internal sealed class EventSubscribeOperateSource : IEventSource
{
    /// <inheritdoc />
    public string EventId { get; set; }

    /// <inheritdoc />
    public object Payload { get; set; }

    /// <inheritdoc />
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

    /// <inheritdoc />
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
    /// 实际事件Id
    /// </summary>
    internal string SubscribeEventId { get; set; }

    /// <summary>
    /// 事件订阅器操作选项
    /// </summary>
    internal EventSubscribeOperates Operate { get; set; }
}
