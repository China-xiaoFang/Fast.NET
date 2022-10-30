namespace Fast.RequestLimit.Filter;

/// <summary>
/// 异步请求验证过滤器接口
/// </summary>
public interface IRequestLimitFilter
{
    /// <summary>
    /// 异步调用
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    Task<bool> InvokeAsync(RequestLimitContext context, CancellationToken cancellation = default);

    /// <summary>
    /// 检验/检查/校验后
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    Task AfterCheckAsync(RequestLimitContext context, CancellationToken cancellation = default);
}