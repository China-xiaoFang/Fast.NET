using System.ComponentModel;
using Fast.SDK.Common.AttributeFilter;

namespace Fast.Admin.Model.Enum;

/// <summary>
/// 环境枚举
/// </summary>
[FastEnum("环境枚举")]
public enum EnvironmentEnum
{
    /// <summary>
    /// 生产
    /// </summary>
    [Description("生产")]
    Production = 1,

    /// <summary>
    /// 预生产
    /// </summary>
    [Description("预生产")]
    PreProduction = 2,

    /// <summary>
    /// 演示
    /// </summary>
    [Description("演示")]
    Demonstration = 3,

    /// <summary>
    /// 测试
    /// </summary>
    [Description("测试")]
    Test = 4,

    /// <summary>
    /// 开发
    /// </summary>
    [Description("开发")]
    Development = 5,
}