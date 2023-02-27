using System.ComponentModel.DataAnnotations;
using Fast.Core.AdminFactory.EnumFactory;
using Fast.SqlSugar.Tenant.BaseModel.Dto;

namespace Fast.Admin.Service.Tenant.Dto;

/// <summary>
/// 查询租户输入
/// </summary>
public class QueryTenantInput : PageInputBase
{
    /// <summary>
    /// 公司名称
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// 公司简称
    /// </summary>
    public string ShortName { get; set; }

    /// <summary>
    /// 管理员名称
    /// </summary>
    public virtual string AdminName { get; set; }

    /// <summary>
    /// 电话号码
    /// </summary>
    public string Phone { get; set; }
}

/// <summary>
/// 添加租户信息输入
/// </summary>
public class AddTenantInput
{
    /// <summary>
    /// 租户公司名称
    /// </summary>
    [Required(ErrorMessage = "租户公司名称不能为空")]
    public string Name { get; set; }

    /// <summary>
    /// 租户公司简称
    /// </summary>
    [Required(ErrorMessage = "租户公司简称不能为空")]
    public string ShortName { get; set; }

    /// <summary>
    /// 租户管理员名称
    /// </summary>
    [Required(ErrorMessage = "租户管理员名称不能为空")]
    public string AdminName { get; set; }

    /// <summary>
    /// 租户管理员邮箱
    /// </summary>
    [Required(ErrorMessage = "租户管理员邮箱不能为空")]
    public string Email { get; set; }

    /// <summary>
    /// 租户电话
    /// </summary>
    [Required(ErrorMessage = "租户电话不能为空")]
    public string Phone { get; set; }

    /// <summary>
    /// 租户类型
    /// </summary>
    [Required(ErrorMessage = "租户类型不能为空")]
    public TenantTypeEnum TenantType { get; set; }

    /// <summary>
    /// WebUrl
    /// </summary>
    [Required(ErrorMessage = "租户Web Url不能为空")]
    public List<string> WebUrl { get; set; }

    /// <summary>
    /// LogoUrl
    /// </summary>
    [Required(ErrorMessage = "Logo Url不能为空")]
    public string LogoUrl { get; set; }
}

/// <summary>
/// 初始化租户信息输入
/// </summary>
public class InitTenantInfoInput
{
    /// <summary>
    /// 租户Id不能为空
    /// </summary>
    [Required(ErrorMessage = "租户Id不能为空")]
    public long Id { get; set; }
}