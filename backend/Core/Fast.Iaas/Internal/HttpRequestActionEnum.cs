namespace Fast.Iaas.Internal{

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
    [Description("查询")]
    Query = 3,

    /// <summary>
    /// 详情
    /// </summary>
    [Description("详情")]
    Detail = 4,

    /// <summary>
    /// 添加
    /// </summary>
    [Description("添加")]
    Add = 5,

    /// <summary>
    /// 更新
    /// </summary>
    [Description("更新")]
    Update = 6,

    /// <summary>
    /// 删除
    /// </summary>
    [Description("删除")]
    Delete = 7,

    /// <summary>
    /// 下载
    /// </summary>
    [Description("下载")]
    Download = 8,

    /// <summary>
    /// 上传
    /// </summary>
    [Description("上传")]
    Upload = 9,

    /// <summary>
    /// 导出
    /// </summary>
    [Description("导出")]
    Export = 10,

    /// <summary>
    /// 导入
    /// </summary>
    [Description("导入")]
    Import = 11,
}
}