// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using SqlSugar;

namespace Fast.SqlSugar;

/// <summary>
/// 连接字符串配置
/// </summary>
[SuppressSniffer]
public class ConnectionSettingsOptions : DbConnectionInfo
{
    /// <summary>
    /// SqlSugarClient 连接Id
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public string ConnectionId { get; set; }

    /// <summary>
    /// 数据库类型，用于区分使用的是那个类型的数据库
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库类型")]
    public DbType? DbType { get; set; }

    /// <summary>
    /// 超时时间，单位为秒
    /// </summary>
    [SugarColumn(ColumnDescription = "超时时间，单位秒")]
    public int? CommandTimeOut { get; set; }

    /// <summary>
    /// SqlSugar SQL 执行最大秒数，如果超过记录警告日志
    /// </summary>
    [SugarColumn(ColumnDescription = "SqlSugar Sql执行最大秒数，如果超过记录警告日志")]
    public int? SugarSqlExecMaxSeconds { get; set; }

    /// <summary>
    /// 差异日志
    /// </summary>
    [SugarColumn(ColumnDescription = "差异日志")]
    public bool? DiffLog { get; set; }

    /// <summary>
    /// 禁用 SqlSugar 的 AOP
    /// </summary>
    /// <remarks>
    /// <para>如果是通过 <see cref="ISqlSugarEntityHandler"/> 进行保存日志到数据库中</para>
    /// <para>必须要将相关 AOP 中涉及到的日志表，单独进行分库设置，并且禁用 AOP</para>
    /// <para>或通过 new <see cref="SqlSugarClient"/>() 的方式进行保存。不然会存在死循环的问题</para>
    /// </remarks>
    [SugarColumn(ColumnDescription = "差异日志")]
    public bool? DisableAop { get; set; }

    /// <summary>
    /// 从库信息
    /// </summary>
    /// <remarks>一般默认库或者主库不建议设置从库</remarks>
    [SugarColumn(IsIgnore = true)]
    public virtual List<SlaveConnectionInfo> SlaveConnectionList { get; set; }

    /// <inheritdoc />
    public override void PostConfigure()
    {
        base.PostConfigure();
        DbType ??= global::SqlSugar.DbType.SqlServer;
        CommandTimeOut ??= 60;
        SugarSqlExecMaxSeconds ??= 30;
        DiffLog ??= false;
        DisableAop ??= true;
    }
}
