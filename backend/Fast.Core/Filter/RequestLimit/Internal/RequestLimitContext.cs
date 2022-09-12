namespace Fast.Core.Filter.RequestLimit.Internal;

/// <summary>
/// 请求限制上下文
/// </summary>
public class RequestLimitContext
{
    /// <summary>
    /// 限制Key
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 限制秒
    /// </summary>
    public int Second { get; set; }

    /// <summary>
    /// 限制次数
    /// </summary>
    public int Count { get; set; }

    public RequestLimitContext(string key, int second, int count)
    {
        Key = key;
        Second = second;
        Count = count;
    }
}