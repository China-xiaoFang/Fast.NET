using Fast.SqlSugar.Attributes;
using Fast.SqlSugar.BaseModel.Interface;
using Fast.SqlSugar.Enum;

namespace Fast.Admin.Model.Model.Sys.Log;

/// <summary>
/// 任务调度执行日志表Model类
/// </summary>
[SugarTable("Sys_Log_Scheduler_Job", "任务调度执行日志表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class SysLogSchedulerJobModel : IPrimaryKeyEntity<Guid>, IDbEntity
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [SugarColumn(ColumnDescription = "Id主键", IsPrimaryKey = true)]
    // 注意是在这里定义你的公共实体
    public virtual Guid Id { get; set; }

    /// <summary>
    /// 任务Id
    /// </summary>
    [SugarColumn(ColumnDescription = "任务Id", ColumnDataType = "Nvarchar(200)", IsNullable = false)]
    public string JobId { get; set; }

    /// <summary>
    /// 是否正在执行
    /// </summary>
    [SugarColumn(ColumnDescription = "是否正在执行", ColumnDataType = "tinyint", IsNullable = false)]
    public YesOrNotEnum Running { get; set; }

    /// <summary>
    /// 是否执行成功
    /// </summary>
    [SugarColumn(ColumnDescription = "是否执行成功", ColumnDataType = "tinyint", IsNullable = true)]
    public YesOrNotEnum? Success { get; set; }

    /// <summary>
    /// 最近运行时间
    /// </summary>
    [SugarColumn(ColumnDescription = "最近运行时间", ColumnDataType = "datetimeoffset", IsNullable = true)]
    public DateTime? LastRunTime { get; set; }

    /// <summary>
    /// 下一次运行时间
    /// </summary>
    [SugarColumn(ColumnDescription = "下一次运行时间", ColumnDataType = "datetimeoffset", IsNullable = true)]
    public DateTime? NextRunTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间", ColumnDataType = "datetimeoffset", IsNullable = false)]
    public virtual DateTime? UpdatedTime { get; set; }

    /// <summary>
    /// 异常信息
    /// </summary>
    [SugarColumn(ColumnDescription = "异常信息", ColumnDataType = "Nvarchar(max)", IsNullable = true)]
    public string ExceptionMsg { get; set; }

    /// <summary>
    /// 异常源
    /// </summary>
    [SugarColumn(ColumnDescription = "异常源", ColumnDataType = "Nvarchar(max)", IsNullable = true)]
    public string ExceptionSource { get; set; }

    /// <summary>
    /// 异常堆栈信息
    /// </summary>
    [SugarColumn(ColumnDescription = "异常堆栈信息", ColumnDataType = "Nvarchar(max)", IsNullable = true)]
    public string ExceptionStackTrace { get; set; }
}