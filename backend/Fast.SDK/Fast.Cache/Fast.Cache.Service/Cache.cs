using Fast.Cache.Service.Enum;
using Fast.Cache.Service.Realize;
using Furion.DependencyInjection;

namespace Fast.Cache.Service;

/// <summary>
/// 缓存实现
/// </summary>
public class Cache : ICache, ISingleton
{
    private readonly ICache _cache;

    public Cache()
    {
        _cache = CacheContext.CacheOptions.CacheType switch
        {
            CacheTypeEnum.MemoryCache => new MemoryCache(),
            CacheTypeEnum.RedisCache => new RedisCache(),
            _ => throw new Exception("JSON文件节点 [Cache:CacheType] 配置错误！")
        };
    }

    /// <summary>
    /// 用于在 key 存在时删除 key
    /// </summary>
    /// <param name="key">键</param>
    public long Del(params string[] key)
    {
        return _cache.Del(key);
    }

    /// <summary>
    /// 用于在 key 存在时删除 key
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public async Task<long> DelAsync(params string[] key)
    {
        return await _cache.DelAsync(key);
    }

    /// <summary>
    /// 用于在 key 模板存在时删除
    /// </summary>
    /// <param name="pattern">key模板</param>
    /// <returns></returns>
    public async Task<long> DelByPatternAsync(string pattern)
    {
        return await _cache.DelByPatternAsync(pattern);
    }

    /// <summary>
    /// 检查给定 key 是否存在
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public bool Exists(string key)
    {
        return _cache.Exists(key);
    }

    /// <summary>
    /// 检查给定 key 是否存在
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public async Task<bool> ExistsAsync(string key)
    {
        return await _cache.ExistsAsync(key);
    }

    /// <summary>
    /// 获取指定 key 的值
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public string Get(string key)
    {
        return _cache.Get(key);
    }

    /// <summary>
    /// 获取指定 key 的值
    /// </summary>
    /// <typeparam name="T">byte[] 或其他类型</typeparam>
    /// <param name="key">键</param>
    /// <returns></returns>
    public T Get<T>(string key)
    {
        return _cache.Get<T>(key);
    }

    /// <summary>
    /// 获取指定 key 的值
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public async Task<string> GetAsync(string key)
    {
        return await _cache.GetAsync(key);
    }

    /// <summary>
    /// 获取指定 key 的值
    /// </summary>
    /// <typeparam name="T">byte[] 或其他类型</typeparam>
    /// <param name="key">键</param>
    /// <returns></returns>
    public async Task<T> GetAsync<T>(string key)
    {
        return await _cache.GetAsync<T>(key);
    }

    /// <summary>
    /// 设置指定 key 的值，所有写入参数object都支持string | byte[] | 数值 | 对象
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public bool Set(string key, object value)
    {
        return _cache.Set(key, value);
    }

    /// <summary>
    /// 设置指定 key 的值，所有写入参数object都支持string | byte[] | 数值 | 对象
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="expire">有效期</param>
    public bool Set(string key, object value, TimeSpan expire)
    {
        return _cache.Set(key, value, expire);
    }

    /// <summary>
    /// 设置指定 key 的值，所有写入参数object都支持string | byte[] | 数值 | 对象
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns></returns>
    public async Task<bool> SetAsync(string key, object value)
    {
        return await _cache.SetAsync(key, value);
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
        return await _cache.SetAsync(key, value, expire);
    }

    /// <summary>
    /// 获取所有缓存
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllKeys()
    {
        return _cache.GetAllKeys();
    }
}