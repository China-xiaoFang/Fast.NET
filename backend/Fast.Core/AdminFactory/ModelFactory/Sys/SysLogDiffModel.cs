using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.BaseModel;
using Fast.SqlSugar.Tenant.AttributeFilter;
using Fast.SqlSugar.Tenant.Internal.Enum;

namespace Fast.Core.AdminFactory.ModelFactory.Sys;

/// <summary>
/// 系统差异日志表Model类
/// </summary>
[SugarTable("Sys_Log_Diff_{year}{month}{day}", "系统差异日志表")]
[SplitTable(SplitType.Month)]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class SysLogDiffModel : BaseLogEntity
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
    /// 差异描述
    /// </summary>
    [SugarColumn(ColumnDescription = "差异描述", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string DiffDescription { get; set; }

    /// <summary>
    /// 表名称
    /// </summary>
    [SugarColumn(ColumnDescription = "表名称", ColumnDataType = "Nvarchar(100)", IsNullable = false)]
    public string TableName { get; set; }

    /// <summary>
    /// 表描述
    /// </summary>
    [SugarColumn(ColumnDescription = "表描述", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string TableDescription { get; set; }

    /// <summary>
    /// 旧的列信息
    /// </summary>
    [SugarColumn(ColumnDescription = "旧的列信息", ColumnDataType = "Nvarchar(MAX)", IsNullable = true, IsJson = true)]
    public List<List<DiffLogColumnInfo>> AfterColumnInfo { get; set; }

    /// <summary>
    /// 新的列信息
    /// </summary>
    [SugarColumn(ColumnDescription = "新的列信息", ColumnDataType = "Nvarchar(MAX)", IsNullable = true, IsJson = true)]
    public List<List<DiffLogColumnInfo>> BeforeColumnInfo { get; set; }

    /// <summary>
    /// 执行的纯Sql语句
    /// </summary>
    [SugarColumn(ColumnDescription = "执行的纯Sql语句", ColumnDataType = "Nvarchar(MAX)", IsNullable = false)]
    public string ExecuteSql { get; set; }

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
}