using Fast.Core.ServiceCollection.RequestLimit.Internal;
using Furion.DependencyInjection;

namespace Fast.Core.ServiceCollection.RequestLimit.Filter;

/// <summary>
/// 异步请求验证过滤器实现
/// 单例注入
/// </summary>
public class RequestLimitFilter : IRequestLimitFilter, ISingleton
{
    private static ICache _cache => App.GetService<ICache>();

    /// <summary>
    /// 缓存前缀
    /// </summary>
    private const string _cachePrefix = "requestLimit:";

    /// <summary>
    /// 并发控制配置
    /// </summary>
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// 异步调用
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public async Task<bool> InvokeAsync(RequestLimitContext context, CancellationToken cancellation = default)
    {
        if (!context.IsCheck)
            return true;

        cancellation.ThrowIfCancellationRequested();

        // 控制并发
        await _semaphore.WaitAsync(cancellation);

        // 获取缓存Key
        var cacheKey = $"{_cachePrefix}{context.Key}";

        try
        {
            // 获取已限制次数
            var limit = await _cache.GetAsync<int?>(cacheKey);

            if (limit != null)
            {
                return limit + 1 <= context.Count;
            }
            else
            {
                return true;
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// 异步过程
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public async Task AfterCheckAsync(RequestLimitContext context, CancellationToken cancellation = default)
    {
        if (!context.IsCheck)
            return;

        cancellation.ThrowIfCancellationRequested();

        // 控制并发
        await _semaphore.WaitAsync(cancellation);

        // 获取缓存Key
        var cacheKey = $"{_cachePrefix}{context.Key}";

        try
        {
            // 获取已限制次数
            var limit = await _cache.GetAsync<int?>(cacheKey);

            if (limit != null)
            {
                // 写入缓存
                await _cache.SetAsync(cacheKey, limit + 1, new TimeSpan(0, 0, context.Second));
            }
            else
            {
                // 写入缓存
                await _cache.SetAsync(cacheKey, 1, new TimeSpan(0, 0, context.Second));
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
}