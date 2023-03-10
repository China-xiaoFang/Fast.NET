using Fast.Cache.Service;
using Fast.Core.Internal.AttributeFilter;
using Fast.Core.Util.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GetXnRestfulResult = Fast.Core.Internal.Restful.Extension.Extension;

namespace Fast.Core.Internal.Middleware;

/// <summary>
/// 接口限流中间件
/// </summary>
public class RequestLimitMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// 默认限制次数
    /// </summary>
    private const int _defaultLimit = 1;

    /// <summary>
    /// 默认限制秒
    /// </summary>
    public const int _defaultSecond = 1;

    /// <summary>
    /// 并发控制配置
    /// </summary>
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// 缓存前缀
    /// </summary>
    private const string _cachePrefix = "requestLimit:";

    private readonly ICache _cache;

    public RequestLimitMiddleware(RequestDelegate next, ICache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 获取租户Id
        var tenantId = GlobalContext.GetTenantId(false);
        // 获取用户Id
        var userId = GlobalContext.UserId;
        // 获取公网信息
        var wanInfo = await HttpNewUtil.WanInfo(HttpNewUtil.Ip);

        // 限制次数
        var limitCount = _defaultLimit;
        // 限制描述
        var limitSecond = _defaultSecond;
        // 缓存Key
        var limitKey = $"{context.Request.Method}{context.Request.Path}:";

        // 获取请求限制特性
        var requestLimitAttribute = context.GetMetadata<RequestLimitAttribute>();

        // 判断是否存在特性，如果不存在则使用默认的配置
        if (requestLimitAttribute != null)
        {
            if (requestLimitAttribute.IsCheck)
            {
                limitCount = requestLimitAttribute.Count;
                limitSecond = requestLimitAttribute.Second;
                // ReSharper disable once ConvertSwitchStatementToSwitchExpression
                switch (requestLimitAttribute.RequestLimitType)
                {
                    case RequestLimitTypeEnum.Tenant:
                        limitKey += $"{tenantId}";
                        break;
                    case RequestLimitTypeEnum.User:
                        limitKey += $"{tenantId}:{userId}";
                        break;
                    case RequestLimitTypeEnum.Ip:
                        limitKey += $"{wanInfo.Ip}";
                        break;
                    default:
                        limitKey += $"{GlobalContext.UUID}";
                        break;
                }
            }
        }
        else
        {
            // 如果没有配置规则，则默认根据UUID来
            limitKey += $"{GlobalContext.UUID}";
        }

        CancellationToken cancellation = default;

        cancellation.ThrowIfCancellationRequested();

        // 控制并发
        await _semaphore.WaitAsync(cancellation);

        // 组装缓存Key
        var cacheKey = $"{_cachePrefix}{limitKey}";

        try
        {
            // 获取已限制次数
            var limit = await _cache.GetAsync<int?>(cacheKey);

            // 没有限制次数
            if (limit == null)
            {
                // 写入缓存
                await _cache.SetAsync(cacheKey, 1, new TimeSpan(0, 0, limitSecond));
                // 抛给下一个过滤器
                await _next(context);
            }
            else
            {
                // 判断旧的限制次数 + 1 是否小于等于总的限制次数
                if (limit + 1 <= limitCount)
                {
                    // 抛给下一个过滤器
                    await _next(context);
                }
                else
                {
                    // 限流，抛出StatusCode为429的异常
                    await context.Response.WriteAsJsonAsync(
                        GetXnRestfulResult.GetXnRestfulResult(StatusCodes.Status429TooManyRequests, false, null, "429 频繁请求"),
                        App.GetOptions<JsonOptions>()?.JsonSerializerOptions, cancellationToken: cancellation);
                }
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
}