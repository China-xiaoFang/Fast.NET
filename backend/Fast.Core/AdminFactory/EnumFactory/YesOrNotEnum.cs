using System.ComponentModel;
using Fast.SDK.Common.AttributeFilter;

namespace Fast.Core.AdminFactory.EnumFactory;

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