namespace Fast.Admin.Service.SysButton.Dto;

/// <summary>
/// 系统按钮输出
/// </summary>
public class SysButtonOutput : BaseOutput
{
    /// <summary>
    /// 编码
    /// </summary>
    [SugarColumn(ColumnDescription = "编码", ColumnDataType = "Nvarchar(100)", IsNullable = false)]
    public string Code { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [SugarColumn(ColumnDescription = "名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string Name { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序", IsNullable = false)]
    public int Sort { get; set; }
}