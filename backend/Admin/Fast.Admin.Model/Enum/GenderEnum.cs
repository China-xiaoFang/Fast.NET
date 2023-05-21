using System.ComponentModel;

namespace Fast.Admin.Model.Enum;

/// <summary>
/// 性别枚举
/// </summary>
[FastEnum("性别枚举")]
public enum GenderEnum
{
    /// <summary>
    /// 未知
    /// </summary>
    [Description("未知")]
    Unknown = 0,

    /// <summary>
    /// 男
    /// </summary>
    [Description("男")]
    Man = 1,

    /// <summary>
    /// 女
    /// </summary>
    [Description("女")]
    Woman = 2
}