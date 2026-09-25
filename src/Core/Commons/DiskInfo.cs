// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

namespace Fast.NET.Core;

/// <summary>
/// 磁盘信息
/// </summary>
[SuppressSniffer]
public class DiskInfo
{
    /// <summary>
    /// 磁盘名
    /// </summary>
    public string DiskName { get; set; }

    /// <summary>
    /// 类型名
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// 总剩余
    /// </summary>
    public decimal TotalFree { get; set; }

    /// <summary>
    /// 总量
    /// </summary>
    public decimal TotalSize { get; set; }

    /// <summary>
    /// 已使用
    /// </summary>
    public decimal Used { get; set; }

    /// <summary>
    /// 可使用
    /// </summary>
    public decimal AvailableFreeSpace { get; set; }

    /// <summary>
    /// 使用百分比
    /// </summary>
    public decimal AvailablePercent { get; set; }
}
