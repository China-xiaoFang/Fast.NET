namespace Fast.Admin.Entity.System.Config;

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
    /// 名称
    /// </summary>
    [SugarColumn(ColumnDescription = "名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string Name { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    [SugarColumn(ColumnDescription = "值", ColumnDataType = "Nvarchar(200)", IsNullable = false)]
    public string Value { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(ColumnDescription = "描述", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string Description { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string Remark { get; set; }
}