using System.ComponentModel;
using Fast.Core.Internal.AttributeFilter;

namespace Fast.Core.AdminFactory.EnumFactory;

/// <summary>
/// Http请求行为枚举
/// </summary>
[FastEnum("Http请求行为枚举")]
public enum HttpRequestActionEnum
{
    /// <summary>
    /// 登录
    /// </summary>
    [Description("登录")]
    Auth,

    /// <summary>
    /// 查询
    /// </summary>
    [Description("查询")]
    Query,

    /// <summary>
    /// 添加
    /// </summary>
    [Description("添加")]
    Add,

    /// <summary>
    /// 编辑
    /// </summary>
    [Description("编辑")]
    Edit,

    /// <summary>
    /// 删除
    /// </summary>
    [Description("删除")]
    Delete,

    /// <summary>
    /// 导出
    /// </summary>
    [Description("导出")]
    Export,

    /// <summary>
    /// 导入
    /// </summary>
    [Description("导入")]
    Import,
}