using Fast.Admin.Core.Enum.Http;

namespace Fast.Core.AdminModel.Sys.Api;

/// <summary>
/// 系统接口信息表Model类
/// </summary>
[SugarTable("Sys_Api_Info", "系统接口信息表")]
[SugarDbType]
public class SysApiInfoModel : BaseEntity
{
    /// <summary>
    /// 接口地址
    /// </summary>
    [SugarColumn(ColumnDescription = "接口地址", ColumnDataType = "Nvarchar(100)", IsNullable = false)]
    public string ApiUrl { get; set; }

    /// <summary>
    /// 接口名称
    /// </summary>
    [SugarColumn(ColumnDescription = "接口名称", ColumnDataType = "Nvarchar(100)", IsNullable = false)]
    public string ApiName { get; set; }

    /// <summary>
    /// 接口请求方式
    /// </summary>
    [SugarColumn(ColumnDescription = "接口请求方式", ColumnDataType = "Nvarchar(10)", IsNullable = false)]
    public string ApiMethod { get; set; }

    /// <summary>
    /// 接口操作方式
    /// </summary>
    [SugarColumn(ColumnDescription = "接口操作方式", ColumnDataType = "tinyint", IsNullable = false)]
    public HttpRequestActionEnum ApiAction { get; set; }

    /// <summary>
    /// 接口分组Id
    /// </summary>
    [SugarColumn(ColumnDescription = "接口分组Id", IsNullable = false)]
    public long ApiGroupId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序", IsNullable = false)]
    public int Sort { get; set; }
}