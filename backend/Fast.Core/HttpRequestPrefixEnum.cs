namespace Fast.Core;

/// <summary>
/// Http请求前缀枚举
/// </summary>
public enum HttpRequestPrefixEnum
{
    /// <summary>
    /// 登录
    /// </summary>
    login,

    /// <summary>
    /// 查询
    /// </summary>
    list,

    /// <summary>
    /// 分页
    /// </summary>
    page,

    /// <summary>
    /// 添加
    /// </summary>
    add,

    /// <summary>
    /// 编辑
    /// </summary>
    edit,

    /// <summary>
    /// 删除
    /// </summary>
    delete,

    /// <summary>
    /// 导出
    /// </summary>
    export,

    /// <summary>
    /// 导入
    /// </summary>
    import,
}