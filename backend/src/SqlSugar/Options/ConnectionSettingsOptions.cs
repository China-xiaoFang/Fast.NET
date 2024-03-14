﻿// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Fast.IaaS;
using Fast.SqlSugar.Handlers;
using SqlSugar;

namespace Fast.SqlSugar.Options;

/// <summary>
/// <see cref="ConnectionSettingsOptions"/> 连接字符串配置
/// </summary>
[SuppressSniffer]
public class ConnectionSettingsOptions : DbConnectionInfo
{
    /// <summary>
    /// SqlSugarClient 连接Id
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public string ConnectionId { get; set; }

    ///// <summary>
    ///// 系统数据库类型
    ///// </summary>
    //[SugarColumn(ColumnDescription = "系统数据库类型", ColumnDataType = "tinyint", IsNullable = false)]
    //public int SugarSysDbType { get; set; }

    ///// <summary>
    ///// 系统数据库类型名称
    ///// </summary>
    //[SugarColumn(ColumnDescription = "系统数据库类型名称", ColumnDataType = "NVARCHAR(50)", IsNullable = true)]
    //public string SugarDbTypeName { get; set; }

    /// <summary>
    /// 数据库类型，用于区分使用的是那个类型的数据库
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库类型", ColumnDataType = "tinyint", IsNullable = false)]
    public DbType DbType { get; set; }

    /// <summary>
    /// 超时时间，单位秒
    /// </summary>
    [SugarColumn(ColumnDescription = "超时时间，单位秒", IsNullable = false)]
    public int CommandTimeOut { get; set; }

    /// <summary>
    /// SqlSugar Sql执行最大秒数，如果超过记录警告日志
    /// </summary>
    [SugarColumn(ColumnDescription = "SqlSugar Sql执行最大秒数，如果超过记录警告日志", IsNullable = false)]
    public double SugarSqlExecMaxSeconds { get; set; }

    /// <summary>
    /// 差异日志
    /// </summary>
    [SugarColumn(ColumnDescription = "差异日志", IsNullable = false)]
    public bool DiffLog { get; set; }

    /// <summary>
    /// 禁用 SqlSugar 的 Aop
    /// <remarks>如果是通过 <see cref="ISqlSugarEntityHandler"/> 进行保存日志到数据库中，必须要将相关 AOP 中涉及到的日志表，单独进行分库设置，并且禁用 AOP，不然会导致死循环的问题。</remarks>
    /// </summary>
    [SugarColumn(ColumnDescription = "差异日志", IsNullable = false)]
    public bool DisableAop { get; set; }

    /// <summary>
    /// 从库信息
    /// <remarks>一般默认库或者主库不建议设置从库</remarks>
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public virtual List<SlaveConnectionInfo> SlaveConnectionList { get; set; }
}