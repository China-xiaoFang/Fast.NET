#nullable enable
namespace Fast.Core.Filter.RequestLimit.AttributeFilter;

/// <summary>
/// 请求限制特性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequestLimitAttribute : Attribute
{
    /// <summary>
    /// 限制Key，默认为接口请求相对地址
    /// 也可以传入一个Name，获取请求参数中的数据做判断
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// 限制秒
    /// </summary>
    public int Second { get; set; }

    /// <summary>
    /// 限制次数
    /// </summary>
    public int Count { get; set; }

    public RequestLimitAttribute(int second, int count, string? key)
    {
        Key = key;
        Second = second;
        Count = count;
    }
}