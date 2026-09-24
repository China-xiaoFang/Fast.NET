// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

namespace Fast.EventBus;

/// <summary>
/// 事件重试失败回调服务
/// </summary>
[SuppressSniffer]
public interface IEventFallbackPolicy
{
    /// <summary>
    /// 重试失败回调
    /// </summary>
    /// <param name="context">当前事件处理上下文</param>
    /// <param name="ex">触发当前处理流程的异常</param>
    /// <returns>表示异步“重试失败回调”操作的任务</returns>
    Task CallbackAsync(EventHandlerExecutingContext context, Exception ex);
}
