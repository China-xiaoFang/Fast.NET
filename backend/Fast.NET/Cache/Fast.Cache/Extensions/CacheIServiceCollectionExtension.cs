using Microsoft.Extensions.DependencyInjection;

namespace Fast.Cache.Extensions;

/// <summary>
/// <see cref="IServiceCollection"/> 拓展类
/// </summary>
public static class CacheIServiceCollectionExtension
{
    /// <summary>
    /// 添加缓存
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCache(this IServiceCollection services)
    {
        // 单例注入
        services.AddSingleton<ICache, Realize.Cache>();

        return services;
    }
}