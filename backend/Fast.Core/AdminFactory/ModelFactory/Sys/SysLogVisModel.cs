using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.BaseModel;
using Fast.SqlSugar.Tenant.AttributeFilter;
using Fast.SqlSugar.Tenant.Internal.Enum;

namespace Fast.Core.AdminFactory.ModelFactory.Sys;

/// <summary>
/// 系统访问日志表Model类
/// </summary>
[SugarTable("Sys_Log_Vis", "系统访问日志表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class SysLogVisModel : BaseLogEntity
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
    public string Name { get; set; }

    /// <summary>
    /// 是否执行成功
    /// </summary>
    [SugarColumn(ColumnDescription = "是否执行成功", ColumnDataType = "tinyint", IsNullable = true)]
    public YesOrNotEnum Success { get; set; }

    /// <summary>
    /// 具体消息
    /// </summary>
    [SugarColumn(ColumnDescription = "具体消息", ColumnDataType = "Nvarchar(max)", IsNullable = true)]
    public string Message { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [SugarColumn(ColumnDescription = "地址", ColumnDataType = "Nvarchar(500)", IsNullable = true)]
    public string Location { get; set; }

    /// <summary>
    /// 访问类型
    /// </summary>
    [SugarColumn(ColumnDescription = "访问类型", ColumnDataType = "tinyint", IsNullable = true)]
    public LoginTypeEnum VisType { get; set; }

    /// <summary>
    /// 访问时间
    /// </summary>
    [SugarColumn(ColumnDescription = "访问时间", ColumnDataType = "datetimeoffset", IsNullable = false)]
    public DateTime VisTime { get; set; }
}