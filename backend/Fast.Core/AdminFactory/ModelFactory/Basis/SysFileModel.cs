namespace Fast.Core.AdminFactory.ModelFactory.Basis;

/// <summary>
/// 系统文件表Model类
/// </summary>
[SugarTable("Sys_File", "系统文件表")]
[DataBaseType(SysDataBaseTypeEnum.Tenant)]
public class SysFileModel : BaseEntity
{
    /// <summary>
    /// 存储位置
    /// </summary>
    [SugarColumn(ColumnDescription = "存储位置", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string Location { get; set; }

    /// <summary>
    /// 存储桶
    /// </summary>
    [SugarColumn(ColumnDescription = "存储桶", ColumnDataType = "Nvarchar(100)", IsNullable = false)]
    public string Bucket { get; set; }

    /// <summary>
    /// 文件名称
    /// </summary>
    [SugarColumn(ColumnDescription = "文件名称", ColumnDataType = "Nvarchar(100)", IsNullable = false)]
    public string Name { get; set; }

    /// <summary>
    /// 文件后缀
    /// </summary>
    [SugarColumn(ColumnDescription = "文件后缀", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string Suffix { get; set; }

    /// <summary>
    /// 文件大小kb
    /// </summary>
    [SugarColumn(ColumnDescription = "文件大小kb", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string SizeKb { get; set; }

    /// <summary>
    /// 文件大小（格式化后）
    /// </summary>
    [SugarColumn(ColumnDescription = "文件大小（格式化后）", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string SizeInfo { get; set; }

    /// <summary>
    /// 文件存储路径
    /// </summary>
    [SugarColumn(ColumnDescription = "文件存储路径", ColumnDataType = "Nvarchar(200)", IsNullable = false)]
    public string StoragePath { get; set; }

    /// <summary>
    /// 文件下载路径
    /// </summary>
    [SugarColumn(ColumnDescription = "文件下载路径", ColumnDataType = "Nvarchar(200)", IsNullable = false)]
    public string DownloadPath { get; set; }

    /// <summary>
    /// 图片缩略图
    /// </summary>
    [SugarColumn(ColumnDescription = "文件下载路径", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string Thumbnail { get; set; }
}