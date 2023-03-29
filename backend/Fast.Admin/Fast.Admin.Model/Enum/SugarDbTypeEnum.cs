using System.ComponentModel;

namespace Fast.Admin.Model.Enum;

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
    /// Log库
    /// </summary>
    [Description("Log库")]
    Log = 2,
}