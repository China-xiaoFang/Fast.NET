using Fast.Core.Cache.Enum;
using Fast.Core.Cache.Internal;
using Fast.Core.Cache.Realize;
using Furion.DependencyInjection;

namespace Fast.Core.Cache;

/// <summary>
/// 缓存实现
/// </summary>
public class CacheRealize : ICache, ISingleton
{
    private readonly ICacheInternal _cacheInternal;

    public CacheRealize(MemoryCache memoryCache, RedisCache redisCache)
    {
        _cacheInternal = GlobalContext.CacheOptions.CacheType switch
        {
            CacheTypeEnum.MemoryCache => memoryCache,
            CacheTypeEnum.RedisCache => redisCache,
            _ => throw new Exception("JSON file node [Cache:CacheType] is incorrectly configured.")
        };
    }

    /// <summary>
    /// 用于在 key 存在时删除 key
    /// </summary>
    /// <param name="key">键</param>
    public long Del(params string[] key)
    {
        return _cacheInternal.Del(key);
    }

    /// <summary>
    /// 用于在 key 存在时删除 key
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public async Task<long> DelAsync(params string[] key)
    {
        return await _cacheInternal.DelAsync(key);
    }

    /// <summary>
    /// 用于在 key 模板存在时删除
    /// </summary>
    /// <param name="pattern">key模板</param>
    /// <returns></returns>
    public async Task<long> DelByPatternAsync(string pattern)
    {
        return await _cacheInternal.DelByPatternAsync(pattern);
    }

    /// <summary>
    /// 检查给定 key 是否存在
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public bool Exists(string key)
    {
        return _cacheInternal.Exists(key);
    }

    /// <summary>
    /// 检查给定 key 是否存在
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public async Task<bool> ExistsAsync(string key)
    {
        return await _cacheInternal.ExistsAsync(key);
    }

    /// <summary>
    /// 获取指定 key 的值
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public string Get(string key)
    {
        return _cacheInternal.Get(key);
    }

    /// <summary>
    /// 获取指定 key 的值
    /// </summary>
    /// <typeparam name="T">byte[] 或其他类型</typeparam>
    /// <param name="key">键</param>
    /// <returns></returns>
    public T Get<T>(string key)
    {
        return _cacheInternal.Get<T>(key);
    }

    /// <summary>
    /// 获取指定 key 的值
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public async Task<string> GetAsync(string key)
    {
        return await _cacheInternal.GetAsync(key);
    }

    /// <summary>
    /// 获取指定 key 的值
    /// </summary>
    /// <typeparam name="T">byte[] 或其他类型</typeparam>
    /// <param name="key">键</param>
    /// <returns></returns>
    public async Task<T> GetAsync<T>(string key)
    {
        return await _cacheInternal.GetAsync<T>(key);
    }

    /// <summary>
    /// 设置指定 key 的值，所有写入参数object都支持string | byte[] | 数值 | 对象
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public bool Set(string key, object value)
    {
        return _cacheInternal.Set(key, value);
    }

    /// <summary>
    /// 设置指定 key 的值，所有写入参数object都支持string | byte[] | 数值 | 对象
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="expire">有效期</param>
    public bool Set(string key, object value, TimeSpan expire)
    {
        return _cacheInternal.Set(key, value, expire);
    }

    /// <summary>
    /// 设置指定 key 的值，所有写入参数object都支持string | byte[] | 数值 | 对象
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns></returns>
    public async Task<bool> SetAsync(string key, object value)
    {
        return await _cacheInternal.SetAsync(key, value);
    }

    /// <summary>
    /// 设置指定 key 的值，所有写入参数object都支持string | byte[] | 数值 | 对象
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="expire">有效期</param>
    /// <returns></returns>
    public async Task<bool> SetAsync(string key, object value, TimeSpan expire)
    {
        return await _cacheInternal.SetAsync(key, value, expire);
    }

    /// <summary>
    /// 获取所有缓存
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllKeys()
    {
        return _cacheInternal.GetAllKeys();
    }
}