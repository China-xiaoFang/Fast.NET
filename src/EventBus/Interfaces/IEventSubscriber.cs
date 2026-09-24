// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

namespace Fast.EventBus;

/// <summary>
/// 事件订阅者依赖接口
/// </summary>
/// <remarks>
/// <para>可自定义事件处理方法，但须符合 <see cref="Func{EventSubscribeExecutingContext, Task}"/> 签名</para>
/// <para>通常只做依赖查找，不做服务调用</para>
/// </remarks>
[SuppressSniffer]
public interface IEventSubscriber
{
    /*
     * // 事件处理程序定义规范
     * [EventSubscribe(YourEventID)]
     * public Task YourHandler(EventHandlerExecutingContext context)
     * {
     *     // To Do...
     * }
     */
}
