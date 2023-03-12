using System.ComponentModel;
using Fast.SDK.Common.AttributeFilter;

namespace Fast.Admin.Model.Enum;

/// <summary>
/// 差异日志类型
/// </summary>
[FastEnum("差异日志类型")]
public enum DiffLogTypeEnum
{
    /// <summary>
    /// 添加
    /// </summary>
    [Description("添加")]
    Insert = 1,

    /// <summary>
    /// 更新
    /// </summary>
    [Description("更新")]
    Update = 2,

    /// <summary>
    /// 删除
    /// </summary>
    [Description("删除")]
    Delete = 3,

    /// <summary>
    /// 未知
    /// </summary>
    [Description("未知")]
    None = 9,
}