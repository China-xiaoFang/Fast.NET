
namespace Fast.Core.AdminModel.Sys.Config;

/// <summary>
/// 系统配置表Model类
/// </summary>
[SugarTable("Sys_Config", "系统配置表")]
[SugarDbType]
public class SysConfigModel : BaseEntity
{
    /// <summary>
    /// 编码
    /// </summary>
    [SugarColumn(ColumnDescription = "编码", ColumnDataType = "Nvarchar(50)", IsNullable = false,
        UniqueGroupNameList = new[] {nameof(Code)})]
    public string Code { get; set; }

    /// <summary>
    /// 中文名称
    /// </summary>
    [SugarColumn(ColumnDescription = "中文名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string ChName { get; set; }

    /// <summary>
    /// 英文名称
    /// </summary>
    [SugarColumn(ColumnDescription = "英文名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string EnName { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    [SugarColumn(ColumnDescription = "值", ColumnDataType = "Nvarchar(200)", IsNullable = false)]
    public string Value { get; set; }
}