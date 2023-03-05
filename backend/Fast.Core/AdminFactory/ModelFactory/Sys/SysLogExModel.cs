using Fast.Core.BaseModel;
using Fast.SqlSugar.Tenant.AttributeFilter;
using Fast.SqlSugar.Tenant.BaseModel.Interface;

namespace Fast.Core.AdminFactory.ModelFactory.Sys;

/// <summary>
/// 系统异常日志表Model类
/// </summary>
[SugarTable("Sys_Log_Ex", "系统异常日志表")]
[SugarDbType]
public class SysLogExModel : BaseLogEntity, IBaseTenant
{
    /// <summary>
    /// 操作人账号
    /// </summary>
    [SugarColumn(ColumnDescription = "操作人账号", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string Account { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "姓名", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string UserName { get; set; }

    /// <summary>
    /// 类名
    /// </summary>
    [SugarColumn(ColumnDescription = "类名", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string ClassName { get; set; }

    /// <summary>
    /// 方法名
    /// </summary>
    [SugarColumn(ColumnDescription = "方法名", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string MethodName { get; set; }

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

    /// <summary>
    /// 参数对象
    /// </summary>
    [SugarColumn(ColumnDescription = "参数对象", ColumnDataType = "Nvarchar(max)", IsNullable = true)]
    public string ParamsObj { get; set; }

    /// <summary>
    /// 异常时间
    /// </summary>
    [SugarColumn(ColumnDescription = "异常时间", ColumnDataType = "datetimeoffset", IsNullable = false)]
    public DateTime ExceptionTime { get; set; }

    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id")]
    public long TenantId { get; set; }
}