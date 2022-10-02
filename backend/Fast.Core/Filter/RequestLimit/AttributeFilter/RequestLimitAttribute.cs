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

    /// <summary>
    /// 是否检查
    /// </summary>
    public bool IsCheck { get; set; }

    /// <summary>
    /// 请求限制类型
    /// </summary>
    public RequestLimitTypeEnum RequestLimitType { get; set; }

    public RequestLimitAttribute(int second, int count, string? key,
        RequestLimitTypeEnum requestLimitType = RequestLimitTypeEnum.User, bool isCheck = true)
    {
        Key = key;
        Second = second;
        Count = count;
        IsCheck = isCheck;
        RequestLimitType = requestLimitType;
    }
}

/// <summary>
/// 请求限制类型枚举
/// </summary>
public enum RequestLimitTypeEnum
{
    /// <summary>
    /// 租户
    /// </summary>
    Tenant = 1,

    /// <summary>
    /// 用户
    /// 一般为租户下的用户
    /// </summary>
    User = 2,

    /// <summary>
    /// Ip
    /// </summary>
    Ip = 3,

    /// <summary>
    /// 其他，自定义
    /// </summary>
    Other = 99,
}