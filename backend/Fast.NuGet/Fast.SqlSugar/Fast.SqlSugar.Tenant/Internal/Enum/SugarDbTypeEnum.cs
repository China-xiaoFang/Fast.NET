using System.ComponentModel;

namespace Fast.SqlSugar.Tenant.Internal.Enum;

/// <summary>
/// Db类型枚举父类
/// </summary>
public enum SugarDbTypeEnum
{
    /// <summary>
    /// 默认库
    /// </summary>
    [Description("默认库")]
    Default = 0,

    /// <summary>
    /// 租户库
    /// </summary>
    [Description("租户库")]
    Tenant = 1,

    /// <summary>
    /// Admin库
    /// </summary>
    [Description("Admin库")]
    Admin = 2,

    /// <summary>
    /// User库
    /// </summary>
    [Description("User库")]
    User = 6,

    /// <summary>
    /// System库
    /// </summary>
    [Description("System库")]
    System = 11,

    /// <summary>
    /// Log库
    /// </summary>
    [Description("Log库")]
    Log = 22,

    /// <summary>
    /// Config库
    /// </summary>
    [Description("Config库")]
    Config = 33,

    /// <summary>
    /// Data库
    /// </summary>
    [Description("Data库")]
    Data = 44,

    /// <summary>
    /// Other库
    /// </summary>
    [Description("Other库")]
    Other = 99,
}