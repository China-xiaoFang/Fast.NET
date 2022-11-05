namespace Fast.Core.RequestLimit.Internal;

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

    /// <summary>
    /// 是否检查
    /// </summary>
    public bool IsCheck { get; set; }

    public RequestLimitContext(int second, int count, string? key, bool isCheck = true)
    {
        Key = key;
        Second = second;
        Count = count;
        IsCheck = isCheck;
    }
}