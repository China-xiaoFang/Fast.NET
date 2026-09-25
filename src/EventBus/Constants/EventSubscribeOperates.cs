// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

namespace Fast.EventBus;

/// <summary>
/// 事件订阅器操作选项
/// </summary>
/// <remarks>控制动态新增/删除事件订阅器</remarks>
internal enum EventSubscribeOperates
{
    /// <summary>
    /// 添加一条订阅器
    /// </summary>
    Append,

    /// <summary>
    /// 删除一条订阅器
    /// </summary>
    Remove
}
