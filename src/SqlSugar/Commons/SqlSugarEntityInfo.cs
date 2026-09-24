// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

namespace Fast.SqlSugar;

/// <summary>
/// SqlSugar 实体信息
/// </summary>
[SuppressSniffer]
public sealed class SqlSugarEntityInfo
{
    /// <summary>
    /// 数据库表名称
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// 数据库表描述
    /// </summary>
    public string TableDescription { get; set; }

    /// <summary>
    /// 实体类型
    /// </summary>
    public Type EntityType { get; set; }

    /// <summary>
    /// 是否分表
    /// </summary>
    public bool IsSplitTable { get; set; }

    /// <summary>
    /// <see cref="SugarDbTypeAttribute"/> 特性的 Type 属性
    /// </summary>
    public object SugarDbType { get; set; }
}
