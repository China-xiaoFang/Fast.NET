using System.ComponentModel;

namespace Fast.Admin.Model.Enum;

/// <summary>
/// 数据权限类型枚举
/// </summary>
[FastEnum("数据权限类型枚举")]
public enum DataScopeTypeEnum
{
    /// <summary>
    /// 全部数据
    /// </summary>
    [Description("全部数据")]
    All = 1,

    /// <summary>
    /// 本部门及以下数据
    /// </summary>
    [Description("本部门及以下数据")]
    DeptWithChild = 2,

    /// <summary>
    /// 本部门数据
    /// </summary>
    [Description("本部门数据")]
    Dept = 3,

    /// <summary>
    /// 仅本人数据
    /// </summary>
    [Description("仅本人数据")]
    Self = 4,

    /// <summary>
    /// 自定义数据
    /// </summary>
    [Description("自定义数据")]
    Define = 5
}