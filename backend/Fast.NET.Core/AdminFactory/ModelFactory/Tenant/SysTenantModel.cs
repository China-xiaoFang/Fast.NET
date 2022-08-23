using Fast.NET.Core.AdminFactory.ModelFactory.Basis;

namespace Fast.NET.Core.AdminFactory.ModelFactory.Tenant;

/// <summary>
/// 租户信息表Model类
/// </summary>
[SugarTable("Sys_Tenant", "租户信息表")]
[DataBaseType]
public class SysTenantModel : BaseEntity
{
    /// <summary>
    /// 租户公司名称
    /// </summary>
    [SugarColumn(ColumnDescription = "租户公司名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string Name { get; set; }

    /// <summary>
    /// 租户公司简称
    /// </summary>
    [SugarColumn(ColumnDescription = "租户公司简称", ColumnDataType = "Nvarchar(30)", IsNullable = false)]
    public string ShortName { get; set; }

    /// <summary>
    /// 租户密钥
    /// </summary>
    [SugarColumn(ColumnDescription = "租户密钥", ColumnDataType = "Nvarchar(32)", IsNullable = false)]
    public string Secret { get; set; }

    /// <summary>
    /// 租户管理员名称
    /// </summary>
    [SugarColumn(ColumnDescription = "租户管理员名称", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string AdminName { get; set; }

    /// <summary>
    /// 租户管理员邮箱
    /// </summary>
    [SugarColumn(ColumnDescription = "租户管理员邮箱", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string Email { get; set; }

    /// <summary>
    /// 租户电话
    /// </summary>
    [SugarColumn(ColumnDescription = "租户电话", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string Phone { get; set; }

    /// <summary>
    /// 租户类型
    /// </summary>
    [SugarColumn(ColumnDescription = "租户类型", IsNullable = false)]
    public TenantTypeEnum TenantType { get; set; }

    /// <summary>
    /// WebUrl
    /// 必须采用Https
    /// </summary>
    [SugarColumn(ColumnDescription = "WebUrl", ColumnDataType = "Nvarchar(max)", IsNullable = false, IsJson = true)]
    public List<string> WebUrl { get; set; }

    /// <summary>
    /// LogoUrl
    /// </summary>
    [SugarColumn(ColumnDescription = "LogoUrl", ColumnDataType = "Nvarchar(max)", IsNullable = false)]
    public string LogoUrl { get; set; }

    /// <summary>
    /// 租户系统管理员用户
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public SysUserModel SysAdminUserModel { get; set; }

    /// <summary>
    /// App授权信息
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysTenantAppInfoModel> AppList { get; set; }
}