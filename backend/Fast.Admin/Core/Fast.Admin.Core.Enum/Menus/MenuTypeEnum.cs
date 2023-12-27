namespace Fast.Admin.Core.Enum.Menus;

/// <summary>
/// 菜单类型枚举
/// </summary>
[FastEnum("菜单类型枚举")]
public enum MenuTypeEnum
{
    /// <summary>
    /// 目录
    /// </summary>
    [Description("目录")]
    Catalog = 1,

    /// <summary>
    /// 菜单
    /// </summary>
    [Description("菜单")]
    Menu = 2,

    /// <summary>
    /// 内链
    /// </summary>
    [Description("内链")]
    Internal = 3,

    /// <summary>
    /// 外链
    /// </summary>
    [Description("外链")]
    Outside = 4
}