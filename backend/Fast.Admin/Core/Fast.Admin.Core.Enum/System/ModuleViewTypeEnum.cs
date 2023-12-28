namespace Fast.Admin.Core.Enum.System;

/// <summary>
/// 模块查看类型枚举
/// </summary>
[FastEnum("模块查看类型枚举")]
public enum ModuleViewTypeEnum
{
    /// <summary>
    /// 超级管理员
    /// </summary>
    [Description("超级管理员")]
    SuperAdmin = 1,

    /// <summary>
    /// 系统管理员
    /// 只有超级管理员和管理员可以查看
    /// </summary>
    [Description("系统管理员")]
    SystemAdmin = 3,

    /// <summary>
    /// 管理员
    /// 只要是管理员都可以查看
    /// </summary>
    [Description("管理员")]
    Admin = 3,

    /// <summary>
    /// 全部
    /// </summary>
    [Description("全部")]
    All = 4
}