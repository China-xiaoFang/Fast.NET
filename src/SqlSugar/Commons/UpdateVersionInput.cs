// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System.ComponentModel.DataAnnotations;

namespace Fast.SqlSugar;

/// <summary>
/// 行版本更新输入
/// </summary>
[SuppressSniffer]
public class UpdateVersionInput
{
    /// <summary>
    /// 更新版本控制字段
    /// </summary>
    [LongRequired(ErrorMessage = "更新版本控制字段不能为空", AllowZero = true)]
    public long RowVersion { get; set; }
}
