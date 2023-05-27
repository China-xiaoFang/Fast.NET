using Fast.Iaas.BaseModel.Dto;

namespace Fast.Admin.Service.SysButton.Dto;

/// <summary>
/// 系统按钮输出
/// </summary>
public class SysButtonOutput : BaseOutput
{
    /// <summary>
    /// 按钮编码
    /// </summary>
    public string ButtonCode { get; set; }

    /// <summary>
    /// 按钮名称
    /// </summary>
    public string ButtonName { get; set; }

    /// <summary>
    /// 接口地址
    /// </summary>
    public string ApiUrl { get; set; }

    /// <summary>
    /// 接口名称
    /// </summary>
    public string ApiName { get; set; }

    /// <summary>
    /// 接口请求方式
    /// </summary>
    public string ApiMethod { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序", IsNullable = false)]
    public int Sort { get; set; }
}