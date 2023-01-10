using Fast.Core.AdminFactory.ServiceFactory.Cache.Dto;
using Furion.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;

namespace Fast.Core.AdminFactory.ServiceFactory.Cache;

/// <summary>
/// 缓存服务
/// </summary>
public class CacheService : ICacheService, ITransient
{
    private readonly ICache _cache;
    private readonly IMemoryCache _memoryCache;

    public CacheService(ICache cache, IMemoryCache memoryCache)
    {
        _cache = cache;
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// 获取缓存类型
    /// </summary>
    /// <returns></returns>
    public string GetCacheType()
    {
        return GlobalContext.CacheOptions.CacheType.GetDescription();
    }

    /// <summary>
    /// 获取所有缓存Key
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllCacheKey()
    {
        return _cache.GetAllKeys();
    }

    /// <summary>
    /// 获取缓存值
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<string> GetCacheValue(string key)
    {
        return await _cache.GetAsync(key);
    }

    /// <summary>
    /// 设置缓存值
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task EditCacheValue(EditCacheValueInput input)
    {
        // 判断Key是否存在
        if (await _cache.ExistsAsync(input.Key))
        {
            await _cache.SetAsync(input.Key, input.Value);
        }
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task DeleteCacheValue(DeleteCacheValueInput input)
    {
        await _cache.DelAsync(input.Key);
    }
}