using System.ComponentModel;

namespace Fast.Core.AdminFactory.EnumFactory;

/// <summary>
/// 租户类型枚举
/// </summary>
[FastEnum("租户类型枚举")]
public enum TenantTypeEnum
{
    /// <summary>
    /// 系统租户
    /// </summary>
    [Description("系统租户")]
    System = 1,

    /// <summary>
    /// 普通租户
    /// </summary>
    [Description("普通租户")]
    Common = 2
}