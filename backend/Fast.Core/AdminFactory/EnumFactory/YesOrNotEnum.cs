using System.ComponentModel;

namespace Fast.Core.AdminFactory.EnumFactory;

/// <summary>
/// 是否枚举
/// </summary>
public enum YesOrNotEnum
{
    /// <summary>
    /// 是
    /// </summary>
    [Description("是")]
    Y = 0,

    /// <summary>
    /// 否
    /// </summary>
    [Description("否")]
    N = 1
}