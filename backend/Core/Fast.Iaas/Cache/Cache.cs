using CSRedis;
using Furion;
using Furion.DependencyInjection;

namespace Fast.Iaas.Cache;

/// <summary>
/// 缓存实现
/// </summary>
public class Cache : ICache, ISingleton
{
    public Cache()
    {
        var csRedis = new CSRedisClient(App.Configuration["Cache:RedisConnectionString"]);
        RedisHelper.Initialization(csRedis);
    }

    /// <summary>
    /// 用于在 key 存在时删除 key
    /// </summary>
    /// <param name="key">键</param>
    public long Del(params string[] key)
    {
        return RedisHelper.Del(key);
    }

    /// <summary>
    /// 用于在 key 存在时删除 key
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public async Task<long> DelAsync(params string[] key)
    {
        return await RedisHelper.DelAsync(key);
    }

    /// <summary>
    /// 用于在 key 模板存在时删除
    /// </summary>
    /// <param name="pattern">key模板</param>
    /// <returns></returns>
    public long DelByPattern(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return default;

        //pattern = Regex.Replace(pattern, @"\{.*\}", "*");
        var keys = RedisHelper.Keys($"{pattern}*");
        if (keys is {Length: > 0})
        {
            return RedisHelper.Del(keys);
        }

        return default;
    }

    /// <summary>
    /// 用于在 key 模板存在时删除
    /// </summary>
    /// <param name="pattern">key模板</param>
    /// <returns></returns>
    public async Task<long> DelByPatternAsync(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return default;

        //pattern = Regex.Replace(pattern, @"\{.*\}", "*");
        var keys = await RedisHelper.KeysAsync($"{pattern}*");
        if (keys is {Length: > 0})
        {
            return await RedisHelper.DelAsync(keys);
        }

        return default;
    }

    /// <summary>
    /// 检查给定 key 是否存在
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public bool Exists(string key)
    {
        return RedisHelper.Exists(key);
    }

    /// <summary>
    /// 检查给定 key 是否存在
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public async Task<bool> ExistsAsync(string key)
    {
        return await RedisHelper.ExistsAsync(key);
    }

    /// <summary>
    /// 获取指定 key 的值
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public string Get(string key)
    {
        return RedisHelper.Get(key);
    }

    /// <summary>
    /// 获取指定 key 的值
    /// </summary>
    /// <typeparam name="T">byte[] 或其他类型</typeparam>
    /// <param name="key">键</param>
    /// <returns></returns>
    public T Get<T>(string key)
    {
        return RedisHelper.Get<T>(key);
    }

    /// <summary>
    /// 获取指定 key 的值
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public async Task<string> GetAsync(string key)
    {
        return await RedisHelper.GetAsync(key);
    }

    /// <summary>
    /// 获取指定 key 的值
    /// </summary>
    /// <typeparam name="T">byte[] 或其他类型</typeparam>
    /// <param name="key">键</param>
    /// <returns></returns>
    public async Task<T> GetAsync<T>(string key)
    {
        return await RedisHelper.GetAsync<T>(key);
    }

    /// <summary>
    /// 设置指定 key 的值，所有写入参数object都支持string | byte[] | 数值 | 对象
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public bool Set(string key, object value)
    {
        return RedisHelper.Set(key, value);
    }

    /// <summary>
    /// 设置指定 key 的值，所有写入参数object都支持string | byte[] | 数值 | 对象
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="expire">有效期</param>
    public bool Set(string key, object value, TimeSpan expire)
    {
        return RedisHelper.Set(key, value, expire);
    }

    /// <summary>
    /// 设置指定 key 的值，所有写入参数object都支持string | byte[] | 数值 | 对象
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns></returns>
    public async Task<bool> SetAsync(string key, object value)
    {
        return await RedisHelper.SetAsync(key, value);
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
        return await RedisHelper.SetAsync(key, value, expire);
    }

    /// <summary>
    /// 获取所有缓存
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllKeys()
    {
        var result = RedisHelper.Keys("*");

        return result.ToList();
    }

    /// <summary>
    /// 获取所有缓存
    /// </summary>
    /// <returns></returns>
    public async Task<List<string>> GetAllKeysAsync()
    {
        var result = await RedisHelper.KeysAsync("*");

        return result.ToList();
    }
}