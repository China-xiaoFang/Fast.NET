using System.ComponentModel;
using Fast.Core.Internal.AttributeFilter;

namespace Fast.Core.AdminFactory.EnumFactory;

/// <summary>
/// 账号类型枚举
/// </summary>
[FastEnum("账号类型枚举")]
public enum AdminTypeEnum
{
    /// <summary>
    /// 超级管理员
    /// </summary>
    [Description("超级管理员")]
    SuperAdmin = 1,

    /// <summary>
    /// 管理员
    /// 每个租户有一个账号，方便管理租户系统
    /// </summary>
    [Description("系统管理员")]
    SystemAdmin = 2,

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