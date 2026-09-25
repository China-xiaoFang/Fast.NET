// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

namespace Fast.EventBus;

/// <summary>
/// 事件处理程序监视器
/// </summary>
[SuppressSniffer]
public interface IEventHandlerMonitor
{
    /// <summary>
    /// 事件处理程序执行前
    /// </summary>
    /// <param name="context">当前事件处理上下文</param>
    /// <returns>表示异步“事件处理程序执行前”操作的任务</returns>
    Task OnExecutingAsync(EventHandlerExecutingContext context);

    /// <summary>
    /// 事件处理程序执行后
    /// </summary>
    /// <param name="context">当前事件处理上下文</param>
    /// <returns>表示异步“事件处理程序执行后”操作的任务</returns>
    Task OnExecutedAsync(EventHandlerExecutedContext context);
}
