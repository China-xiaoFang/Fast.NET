namespace Fast.Core.AdminFactory.EnumFactory;

/// <summary>
/// 角色类型枚举
/// </summary>
public enum RoleTypeEnum
{
    /// <summary>
    /// 租户普通角色
    /// </summary>
    [Description("租户普通角色")]
    NormalRole = 0,

    /// <summary>
    /// 租户管理员角色
    /// </summary>
    [Description("租户管理员角色")]
    AdminRole = 1,
}