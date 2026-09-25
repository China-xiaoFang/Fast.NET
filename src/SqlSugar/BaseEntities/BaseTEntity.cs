// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using SqlSugar;

namespace Fast.SqlSugar;

/// <summary>
/// 租户 Entity 基类
/// </summary>
[SuppressSniffer]
public class BaseTEntity : BaseEntity, IBaseTEntity
{
    /// <inheritdoc />
    [SugarColumn(ColumnDescription = "租户Id", CreateTableFieldSort = 997)]
    public virtual long TenantId { get; set; }
}
