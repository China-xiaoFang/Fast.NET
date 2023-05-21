using System.ComponentModel;
using Fast.Iaas.Internal;

namespace Fast.SqlSugar.Enum;

/// <summary>
/// Sugar数据库类型
/// </summary>
[FastEnum("差异日志类型")]
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
    Tenant = 1
}