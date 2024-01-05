namespace Fast.Core.AdminModel.Sys;

/// <summary>
/// 租户信息表Model类
/// </summary>
[SugarTable("Sys_Tenant", "租户信息表")]
[SugarDbType]
public class SysTenantModel : BaseEntity
{
    /// <summary>
    /// 租户公司中文名称
    /// </summary>
    [SugarColumn(ColumnDescription = "租户公司中文名称", ColumnDataType = "Nvarchar(50)", IsNullable = false,
        UniqueGroupNameList = new[] {nameof(ChName)})]
    public string ChName { get; set; }

    /// <summary>
    /// 租户公司英文名称（拼音）
    /// </summary>
    [SugarColumn(ColumnDescription = "租户公司英文名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string EnName { get; set; }

    /// <summary>
    /// 租户公司中文简称
    /// </summary>
    [SugarColumn(ColumnDescription = "租户公司中文简称", ColumnDataType = "Nvarchar(30)", IsNullable = false)]
    public string ChShortName { get; set; }

    /// <summary>
    /// 租户公司英文简称
    /// </summary>
    [SugarColumn(ColumnDescription = "租户公司英文简称", ColumnDataType = "Nvarchar(30)", IsNullable = false)]
    public string EnShortName { get; set; }

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
    [SugarColumn(ColumnDescription = "租户类型", ColumnDataType = "tinyint", IsNullable = false)]
    public TenantTypeEnum TenantType { get; set; }

    /// <summary>
    /// LogoUrl
    /// </summary>
    [SugarColumn(ColumnDescription = "LogoUrl", ColumnDataType = "Nvarchar(max)", IsNullable = false)]
    public string LogoUrl { get; set; }

    /// <summary>
    /// 系统管理员用户
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public TenUserModel SystemAdminUser { get; set; }

    /// <summary>
    /// 租户管理员用户
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public TenUserModel TenantAdminUser { get; set; }

    /// <summary>
    /// App授权信息
    /// </summary>
    [Navigate(NavigateType.OneToMany, nameof(SysTenantAppInfoModel.TenantId), nameof(Id))]
    public List<SysTenantAppInfoModel> AppList { get; set; }

    /// <summary>
    /// 数据库信息
    /// </summary>
    [Navigate(NavigateType.OneToMany, nameof(SysTenantDataBaseModel.TenantId), nameof(Id))]
    public List<SysTenantDataBaseModel> DataBaseList { get; set; }
}