using System.Linq.Expressions;
using Fast.Admin.Service.Cache.Dto;
using Fast.Core.AdminFactory.ModelFactory.Sys;
using Fast.Core.AdminFactory.ModelFactory.Tenant;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Service.Cache;

/// <summary>
/// 系统缓存服务
/// </summary>
public class SysCacheService : ISysCacheService, ITransient
{
    public ICache _cache { get; }
    private readonly ISqlSugarClient _tenant;

    public SysCacheService(ICache cache, ISqlSugarClient tenant)
    {
        _cache = cache;
        _tenant = tenant;
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