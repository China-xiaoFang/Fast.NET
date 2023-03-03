using System.ComponentModel;
using Fast.Core.Internal.AttributeFilter;

namespace Fast.Core.AdminFactory.EnumFactory;

/// <summary>
/// 系统配置类型枚举
/// </summary>
[FastEnum("系统配置类型枚举")]
public enum SysConfigTypeEnum
{
    /// <summary>
    /// 系统配置
    /// </summary>
    [Description("系统配置")]
    System = 0,

    /// <summary>
    /// 租户配置
    /// </summary>
    [Description("租户配置")]
    Tenant = 1,

    /// <summary>
    /// 其他配置
    /// </summary>
    [Description("其他配置")]
    Other = 99,
}

/*
 * 系统配置，是指一些只能查看，不能修改的配置，程序启动的时候会自动更新此配置，不能修改
 * 租户配置，是指租户可以随意修改的配置，默认放在租户库，数据源
 * 其他配置，是指租户自定的配置，目前没想好怎么使用
 */