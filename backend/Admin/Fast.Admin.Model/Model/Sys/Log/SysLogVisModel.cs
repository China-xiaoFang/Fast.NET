using Fast.Admin.Model.BaseModel;
using Fast.Admin.Model.Enum;
using SqlSugar;

namespace Fast.Admin.Model.Model.Sys.Log;

/// <summary>
/// 系统访问日志表Model类
/// </summary>
[SugarTable("Sys_Log_Vis_{year}{month}{day}", "系统访问日志表")]
[SplitTable(SplitType.Month)]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class SysLogVisModel : BaseLogEntity
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [SugarColumn(ColumnDescription = "Id主键", IsPrimaryKey = true)]
    public new long Id { get; set; }

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
    [SplitField]
    [SugarColumn(ColumnDescription = "访问时间", ColumnDataType = "datetimeoffset", IsNullable = false)]
    public DateTime VisTime { get; set; }
}