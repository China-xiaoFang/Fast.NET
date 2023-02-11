using System.ComponentModel;
using Fast.Core.Internal.AttributeFilter;

namespace Fast.Core.AdminFactory.EnumFactory;

/// <summary>
/// Http请求前缀枚举
/// </summary>
[FastEnum("Http请求前缀枚举")]
public enum HttpRequestPrefixEnum
{
    /// <summary>
    /// 登录
    /// </summary>
    [Description("登录")]
    login,

    /// <summary>
    /// 查询
    /// </summary>
    [Description("查询")]
    list,

    /// <summary>
    /// 分页
    /// </summary>
    [Description("分页")]
    page,

    /// <summary>
    /// 添加
    /// </summary>
    [Description("添加")]
    add,

    /// <summary>
    /// 编辑
    /// </summary>
    [Description("编辑")]
    edit,

    /// <summary>
    /// 删除
    /// </summary>
    [Description("删除")]
    delete,

    /// <summary>
    /// 导出
    /// </summary>
    [Description("导出")]
    export,

    /// <summary>
    /// 导入
    /// </summary>
    [Description("导入")]
    import,
}