using Fast.Iaas.Attributes;
using Fast.Iaas.BaseModel;

namespace Fast.Admin.Model.Model.Tenant;

/// <summary>
/// 租户文件表Model类
/// </summary>
[SugarTable("Ten_File", "租户文件表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenFileModel : BaseEntity
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
    public string FileName { get; set; }

    /// <summary>
    /// 文件后缀
    /// </summary>
    [SugarColumn(ColumnDescription = "文件后缀", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string FileSuffix { get; set; }

    /// <summary>
    /// 文件大小kb
    /// </summary>
    [SugarColumn(ColumnDescription = "文件大小kb", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string FileSizeKb { get; set; }

    /// <summary>
    /// 文件大小（格式化后）
    /// </summary>
    [SugarColumn(ColumnDescription = "文件大小（格式化后）", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string FileSizeInfo { get; set; }

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
    [SugarColumn(ColumnDescription = "文件下载路径", ColumnDataType = "Nvarchar(max)", IsNullable = true)]
    public string Thumbnail { get; set; }
}