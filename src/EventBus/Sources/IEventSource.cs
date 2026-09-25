// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

namespace Fast.EventBus;

/// <summary>
/// 事件源（事件承载对象）依赖接口
/// </summary>
[SuppressSniffer]
public interface IEventSource
{
    /// <summary>
    /// 事件Id
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
