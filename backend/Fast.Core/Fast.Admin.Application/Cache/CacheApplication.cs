using Fast.Core.AdminFactory.ServiceFactory.Cache;
using Fast.Core.AdminFactory.ServiceFactory.Cache.Dto;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Application.Cache;

/// <summary>
/// 缓存接口
/// </summary>
[ApiDescriptionSettings(Name = "Cache", Order = 100)]
public class CacheApplication : IDynamicApiController
{
    private readonly ICacheService _cacheService;

    public CacheApplication(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    /// <summary>
    /// 获取缓存类型
    /// </summary>
    /// <returns></returns>
    [HttpGet("cache/getCacheType", "获取缓存类型")]
    public string GetCacheType()
    {
        return _cacheService.GetCacheType();
    }

    /// <summary>
    /// 获取所有缓存Key
    /// </summary>
    /// <returns></returns>
    [HttpGet("cache/getAllCacheKey", "获取所有缓存Key")]
    public List<string> GetAllCacheKey()
    {
        return _cacheService.GetAllCacheKey();
    }

    /// <summary>
    /// 获取缓存值
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [HttpGet("cache/getCacheValue", "获取缓存值")]
    public async Task<string> GetCacheValue(string key)
    {
        return await _cacheService.GetCacheValue(key);
    }

    /// <summary>
    /// 设置缓存值
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPut("cache/editCacheValue", "设置缓存值")]
    public async Task EditCacheValue(EditCacheValueInput input)
    {
        await _cacheService.EditCacheValue(input);
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpDelete("cache/deleteCacheValue", "删除缓存")]
    public async Task DeleteCacheValue(DeleteCacheValueInput input)
    {
        await _cacheService.DeleteCacheValue(input);
    }
}