// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
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
/// <see cref="SysLogVisModel"/> 系统访问日志表Model类
/// </summary>
[SugarTable("Sys_Log_Vis_{year}{month}{day}", "系统访问日志表")]
[SplitTable(SplitType.Month)]
[SugarDbType(FastDbTypeEnum.SysCoreLog)]
public class SysLogVisModel : BaseSnowflakeRecordEntity, IBaseTEntity
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
    public string UserJobNo { get; set; }

    /// <summary>
    /// 访问类型
    /// </summary>
    [SugarColumn(ColumnDescription = "访问类型", ColumnDataType = "tinyint", IsNullable = true)]
    public VisitTypeEnum VisitType { get; set; }

    /// <summary>
    /// 访问时间
    /// </summary>
    [SplitField]
    [SugarColumn(ColumnDescription = "访问时间", ColumnDataType = "datetimeoffset", IsNullable = false)]
    public DateTime VisTime { get; set; }

    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id", IsNullable = true, CreateTableFieldSort = 997)]
    public long TenantId { get; set; }
}