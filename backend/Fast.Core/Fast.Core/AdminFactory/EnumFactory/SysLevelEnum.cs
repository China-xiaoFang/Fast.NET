using System.ComponentModel;
using Fast.Core.Internal.AttributeFilter;

namespace Fast.Core.AdminFactory.EnumFactory;

/// <summary>
/// 系统级别枚举
/// </summary>
[FastEnum("系统级别枚举")]
public enum SysLevelEnum
{
    /// <summary>
    /// 默认级
    /// </summary>
    [Description("默认级")]
    Default = 0,

    /// <summary>
    /// 系统级
    /// </summary>
    [Description("系统级")]
    System = 1,

    /// <summary>
    /// 自定义级
    /// </summary>
    [Description("自定义级")]
    Custom = 9
}