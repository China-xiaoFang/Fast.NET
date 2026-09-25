// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

namespace Fast.Swagger;

/// <summary>
/// 分组附加信息
/// </summary>
[SuppressSniffer]
public sealed class GroupExtraInfo
{
    /// <summary>
    /// 分组名
    /// </summary>
    public string Group { get; internal set; }

    /// <summary>
    /// 分组排序
    /// </summary>
    public int Order { get; internal set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    public bool Visible { get; internal set; }
}
