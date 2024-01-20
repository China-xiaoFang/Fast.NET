namespace Fast.Admin.Entity.System.Menu;

/// <summary>
/// 系统按钮表Model类
/// </summary>
[SugarTable("Sys_Button", "系统按钮表")]
[SugarDbType]
public class SysButtonModel : BaseEntity
{
    /// <summary>
    /// 按钮编码
    /// </summary>
    [SugarColumn(ColumnDescription = "按钮编码", ColumnDataType = "Nvarchar(100)", IsNullable = false)]
    public string ButtonCode { get; set; }

    /// <summary>
    /// 按钮名称
    /// </summary>
    [SugarColumn(ColumnDescription = "按钮名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string ButtonName { get; set; }

    /// <summary>
    /// 菜单Id
    /// </summary>
    [SugarColumn(ColumnDescription = "菜单Id", IsNullable = false)]
    public long MenuId { get; set; }

    /// <summary>
    /// 接口Id
    /// </summary>
    [SugarColumn(ColumnDescription = "接口Id", IsNullable = false)]
    public long ApiId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序", IsNullable = false)]
    public int Sort { get; set; }
}