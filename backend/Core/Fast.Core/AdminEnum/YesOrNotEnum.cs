using System.ComponentModel;
using Fast.Iaas.Attributes;

namespace Fast.Core.AdminEnum;

/// <summary>
/// 是否枚举
/// </summary>
[FastEnum("是否枚举")]
public enum YesOrNotEnum
{
    /// <summary>
    /// 是
    /// </summary>
    [Description("是")]
    Y = 1,

    /// <summary>
    /// 否
    /// </summary>
    [Description("否")]
    N = 0
}