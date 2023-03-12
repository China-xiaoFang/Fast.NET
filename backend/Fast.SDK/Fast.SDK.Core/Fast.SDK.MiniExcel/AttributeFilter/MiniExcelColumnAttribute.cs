namespace Fast.SDK.MiniExcel.AttributeFilter;

/// <summary>
/// MiniExcel 列导出属性
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class MiniExcelColumn : Attribute
{
    public MiniExcelColumn()
    {
    }

    public MiniExcelColumn(string ColumnDescription)
    {
        this.ColumnDescription = ColumnDescription;
    }

    public MiniExcelColumn(string ColumnDescription, int Sort)
    {
        this.ColumnDescription = ColumnDescription;
        this.Sort = Sort;
    }

    /// <summary>
    /// 列描述
    /// </summary>
    public string ColumnDescription { get; set; }

    /// <summary>
    /// 导出导入是否忽略，优先级高于 IsExportIgnore 和 IsImportIgnore
    /// 默认False
    /// </summary>
    public bool IsIgnore { get; set; } = false;

    /// <summary>
    /// 导出是否忽略
    /// 默认False
    /// </summary>
    public bool IsExportIgnore { get; set; } = false;

    /// <summary>
    /// 导入是否忽略
    /// </summary>
    public bool IsImportIgnore { get; set; } = false;

    /// <summary>
    /// 是否深度处理List，只处理一层，如果为True，则循环List进行处理
    /// 默认False
    /// 注意：开启深度处理List如果为对象
    /// </summary>
    public bool IsListDeep { get; set; } = false;

    /// <summary>
    /// List集合是否换行
    /// 默认只支持 string int bool double decimal dateTime long
    /// 暂且不支持Object类型
    /// </summary>
    public bool IsListLine { get; set; } = false;

    /// <summary>
    /// 是否为Json字符串
    /// 默认False
    /// </summary>
    public bool IsJson { get; set; } = false;

    /// <summary>
    /// 排序
    /// 从1开始
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// True描述
    /// 默认是
    /// </summary>
    public string TrueDesc { get; set; }

    /// <summary>
    /// False描述
    /// 默认否
    /// </summary>
    public string FalseDesc { get; set; }

    /// <summary>
    /// DateTime转换
    /// 默认 yyyy-MM-dd HH:mm:ss
    /// </summary>
    public string ConvertDateTime { get; set; } = "yyyy-MM-dd HH:mm:ss";
}