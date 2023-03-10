using System.ComponentModel;
using Fast.SDK.Common.AttributeFilter;

namespace Fast.SDK.Common.EnumFactory;

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
    Auth = 1,

    /// <summary>
    /// 查询
    /// </summary>
    [Description("查询")]
    Query = 2,

    /// <summary>
    /// 添加
    /// </summary>
    [Description("添加")]
    Add = 3,

    /// <summary>
    /// 更新
    /// </summary>
    [Description("更新")]
    Update = 4,

    /// <summary>
    /// 删除
    /// </summary>
    [Description("删除")]
    Delete = 5,

    /// <summary>
    /// 下载
    /// </summary>
    [Description("下载")]
    Download = 6,

    /// <summary>
    /// 上传
    /// </summary>
    [Description("上传")]
    Upload = 7,

    /// <summary>
    /// 导出
    /// </summary>
    [Description("导出")]
    Export = 8,

    /// <summary>
    /// 导入
    /// </summary>
    [Description("导入")]
    Import = 9,
}