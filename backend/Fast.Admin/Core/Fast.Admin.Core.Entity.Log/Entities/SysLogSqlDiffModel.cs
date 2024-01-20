// Apache开源许可证
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

using Fast.Admin.Core.Enum.Db;
using Fast.Admin.Core.Enum.System;

namespace Fast.Admin.Core.Entity.Log.Entities;

/// <summary>
/// <see cref="SysLogSqlDiffModel"/> 系统Sql差异日志表Model类
/// </summary>
[SugarTable("Sys_Log_Sql_Diff_{year}{month}{day}", "系统Sql差异日志表")]
[SplitTable(SplitType.Month)]
[SugarDbType(FastDbTypeEnum.SysCoreLog)]
public class SysLogSqlDiffModel : BaseSnowflakeRecordEntity
{
    /// <summary>
    /// 操作人账号
    /// </summary>
    [SugarColumn(ColumnDescription = "操作人账号", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string Account { get; set; }

    /// <summary>
    /// 操作人工号
    /// </summary>
    [SugarColumn(ColumnDescription = "操作人工号", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string JobNumber { get; set; }

    /// <summary>
    /// 表名称
    /// </summary>
    [SugarColumn(ColumnDescription = "表名称", ColumnDataType = "Nvarchar(MAX)", IsNullable = false)]
    public string TableName { get; set; }

    /// <summary>
    /// 表描述
    /// </summary>
    [SugarColumn(ColumnDescription = "表描述", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string TableDescription { get; set; }

    /// <summary>
    /// 差异描述
    /// </summary>
    [SugarColumn(ColumnDescription = "差异描述", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string DiffDescription { get; set; }

    /// <summary>
    /// 旧的列信息
    /// </summary>
    [SugarColumn(ColumnDescription = "旧的列信息", ColumnDataType = "Nvarchar(MAX)", IsNullable = true, IsJson = true)]
    public List<List<DiffLogColumnInfo>> BeforeColumnList { get; set; }

    /// <summary>
    /// 新的列信息
    /// </summary>
    [SugarColumn(ColumnDescription = "新的列信息", ColumnDataType = "Nvarchar(MAX)", IsNullable = true, IsJson = true)]
    public List<List<DiffLogColumnInfo>> AfterColumnList { get; set; }

    /// <summary>
    /// 执行秒数
    /// </summary>
    [SugarColumn(ColumnDescription = "执行秒数", IsNullable = true)]
    public double? ExecuteSeconds { get; set; }

    /// <summary>
    /// 原始Sql
    /// </summary>
    [SugarColumn(ColumnDescription = "原始Sql", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string RawSql { get; set; }

    /// <summary>
    /// Sql参数
    /// </summary>
    [SugarColumn(ColumnDescription = "Sql参数", ColumnDataType = "Nvarchar(MAX)", IsNullable = true, IsJson = true)]
    public SugarParameter[] Parameters { get; set; }

    /// <summary>
    /// 纯Sql，参数化之后的Sql
    /// </summary>
    [SugarColumn(ColumnDescription = "纯Sql，参数化之后的Sql", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string PureSql { get; set; }

    /// <summary>
    /// 差异日志类型
    /// </summary>
    [SugarColumn(ColumnDescription = "差异日志类型", ColumnDataType = "tinyint", IsNullable = true)]
    public DiffLogTypeEnum DiffType { get; set; }

    /// <summary>
    /// 差异时间
    /// </summary>
    [SplitField]
    [SugarColumn(ColumnDescription = "差异时间", ColumnDataType = "datetimeoffset", IsNullable = false)]
    public DateTime DiffTime { get; set; }

    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id", IsNullable = true, CreateTableFieldSort = 997)]
    public long? TenantId { get; set; }
}