namespace Fast.Core.Internal.Restful.Internal;

/// <summary>
/// 统一分页返回结果类
/// </summary>
/// <typeparam name="T"></typeparam>
public class PageResult<T>
{
    /// <summary>
    /// 当前页
    /// </summary>
    public int PageNo { get; set; }

    /// <summary>
    /// 当前页码
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPage { get; set; }

    /// <summary>
    /// 总条数
    /// </summary>
    public int TotalRows { get; set; }

    /// <summary>
    /// Data
    /// </summary>
    public IEnumerable<T> Rows { get; set; }
}