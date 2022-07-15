namespace Fast.NET.Core.AdminFactory.EnumFactory;

/// <summary>
/// 账号类型
/// </summary>
public enum AdminTypeEnum
{
    /// <summary>
    /// 超级管理员
    /// </summary>
    [Description("超级管理员")]
    SuperAdmin = 1,

    /// <summary>
    /// 管理员
    /// </summary>
    [Description("管理员")]
    Admin = 2,

    /// <summary>
    /// 租户管理员
    /// </summary>
    [Description("租户管理员")]
    TenantAdmin = 3,

    /// <summary>
    /// 普通账号
    /// </summary>
    [Description("普通账号")]
    None = 4
}