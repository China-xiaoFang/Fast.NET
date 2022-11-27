using System.ComponentModel;

namespace Fast.Core.AdminFactory.EnumFactory;

/// <summary>
/// App应用枚举
/// </summary>
[FastEnum("App应用枚举")]
public enum AppTypeEnum
{
    /// <summary>
    /// 系统后台
    /// </summary>
    [Description("系统后台")]
    FastNet = 1,
}