// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using SqlSugar;

namespace Fast.SqlSugar;

/// <summary>
/// 从库连接信息
/// </summary>
[SuppressSniffer]
public class SlaveConnectionInfo : DbConnectionInfo
{
    /// <summary>
    /// 从库命中率
    /// </summary>
    /// <remarks>
    /// <para>为 0 则不命中</para>
    /// <para>建议相加不超过 10</para>
    /// </remarks>
    [SugarColumn(ColumnDescription = "从库命中率")]
    public virtual int HitRate { get; set; }
}
