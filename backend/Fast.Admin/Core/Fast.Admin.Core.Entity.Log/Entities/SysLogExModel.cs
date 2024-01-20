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

namespace Fast.Admin.Core.Entity.Log.Entities;

/// <summary>
/// <see cref="SysLogExModel"/> 系统异常日志表Model类
/// </summary>
[SugarTable("Sys_Log_Ex", "系统异常日志表")]
[SugarDbType(FastDbTypeEnum.SysCoreLog)]
public class SysLogExModel : BaseIdentityRecordEntity, IBaseTEntity
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
    /// 类名
    /// </summary>
    [SugarColumn(ColumnDescription = "类名", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string ClassName { get; set; }

    /// <summary>
    /// 方法名
    /// </summary>
    [SugarColumn(ColumnDescription = "方法名", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string MethodName { get; set; }

    /// <summary>
    /// 异常信息
    /// </summary>
    [SugarColumn(ColumnDescription = "异常信息", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string ExceptionMsg { get; set; }

    /// <summary>
    /// 异常源
    /// </summary>
    [SugarColumn(ColumnDescription = "异常源", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string ExceptionSource { get; set; }

    /// <summary>
    /// 异常堆栈信息
    /// </summary>
    [SugarColumn(ColumnDescription = "异常堆栈信息", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string ExceptionStackTrace { get; set; }

    /// <summary>
    /// 参数对象
    /// </summary>
    [SugarColumn(ColumnDescription = "参数对象", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string ParamsObj { get; set; }

    /// <summary>
    /// 异常时间
    /// </summary>
    [SugarColumn(ColumnDescription = "异常时间", ColumnDataType = "datetimeoffset", IsNullable = false)]
    public DateTime ExceptionTime { get; set; }

    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id", IsNullable = true, CreateTableFieldSort = 997)]
    public long TenantId { get; set; }
}