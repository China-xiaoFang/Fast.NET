namespace Fast.Admin.Service.Auth.Dto;

/// <summary>
/// Web站点初始化
/// </summary>
public class WebSiteInitOutput
{
    /// <summary>
    /// LogoUrl
    /// </summary>
    public string LogoUrl { get; set; }

    /// <summary>
    /// 中文名称
    /// </summary>
    public string ChName { get; set; }

    /// <summary>
    /// 英文名称
    /// </summary>
    public string EnName { get; set; }

    /// <summary>
    /// 租户公司中文简称
    /// </summary>
    public string ChShortName { get; set; }

    /// <summary>
    /// 租户公司英文简称
    /// </summary>
    public string EnShortName { get; set; }

    /// <summary>
    /// 租户Id，Base64加密
    /// </summary>
    public string TenantId { get; set; }
}