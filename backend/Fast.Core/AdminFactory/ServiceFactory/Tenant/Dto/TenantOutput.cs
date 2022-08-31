namespace Fast.Core.AdminFactory.ServiceFactory.Tenant.Dto;

/// <summary>
/// 租户输出
/// </summary>
public class TenantOutput : OutputInputBase
{
    /// <summary>
    /// 租户公司名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 租户公司简称
    /// </summary>
    public string ShortName { get; set; }

    /// <summary>
    /// 租户密钥
    /// </summary>
    public string Secret { get; set; }

    /// <summary>
    /// 租户管理员名称
    /// </summary>
    public string AdminName { get; set; }

    /// <summary>
    /// 租户管理员邮箱
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 租户电话
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// 租户类型
    /// </summary>
    public TenantTypeEnum TenantType { get; set; }

    /// <summary>
    /// WebUrl
    /// 必须采用Https
    /// </summary>
    public List<string> WebUrl { get; set; }

    /// <summary>
    /// LogoUrl
    /// </summary>
    public string LogoUrl { get; set; }
}