using System.ComponentModel;

namespace Fast.Admin.Model.Enum;

/// <summary>
/// App应用枚举
/// </summary>
[FastEnum("App应用枚举")]
public enum AppTypeEnum
{
    /// <summary>
    /// Web管理后台
    /// </summary>
    [Description("Web管理后台")]
    WebAdmin = 1,
}