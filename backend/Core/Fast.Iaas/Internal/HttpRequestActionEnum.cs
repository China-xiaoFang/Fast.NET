namespace Fast.Iaas.Internal;

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
    /// 分页查询
    /// </summary>
    [Description("分页查询")]
    Page = 2,

    /// <summary>
    /// 查询
    /// </summary>
    [Description("分页查询")]
    Query = 3,

    /// <summary>
    /// 添加
    /// </summary>
    [Description("添加")]
    Add = 4,

    /// <summary>
    /// 更新
    /// </summary>
    [Description("更新")]
    Update = 5,

    /// <summary>
    /// 删除
    /// </summary>
    [Description("删除")]
    Delete = 6,

    /// <summary>
    /// 下载
    /// </summary>
    [Description("下载")]
    Download = 7,

    /// <summary>
    /// 上传
    /// </summary>
    [Description("上传")]
    Upload = 8,

    /// <summary>
    /// 导出
    /// </summary>
    [Description("导出")]
    Export = 9,

    /// <summary>
    /// 导入
    /// </summary>
    [Description("导入")]
    Import = 10,
}