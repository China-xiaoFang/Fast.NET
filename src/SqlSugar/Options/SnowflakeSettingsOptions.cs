// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using System.ComponentModel.DataAnnotations;
using Fast.Runtime;

namespace Fast.SqlSugar;

/// <summary>
/// 雪花Id配置
/// </summary>
[SuppressSniffer]
public class SnowflakeSettingsOptions : IPostConfigure
{
    /// <summary>
    /// 工作Id
    /// </summary>
    [Required]
    public ushort? WorkerId { get; set; }

    /// <inheritdoc />
    public void PostConfigure()
    {
        WorkerId ??= 1;
    }
}
