using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSRedis;
using Fast.Core.DependencyInjection.Dependencies;
using Microsoft.Extensions.Configuration;

namespace Fast.Cache;

/// <summary>
/// 缓存实现
/// </summary>
public class Cache : ICache, ISingletonDependency
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration">连接字符串</param>
    public Cache(IConfiguration configuration)
    {
        // 读取 Redis 缓存字符串
        var connectionString = configuration["RedisConnectionString"];
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), "Inject cache is connectionString empty.");
        }

        var csRedis = new CSRedisClient(connectionString);
        RedisHelper.Initialization(csRedis);
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public long Del(params string[] key)
    {
        return RedisHelper.Del(key);
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<long> DelAsync(params string[] key)
    {
        return await RedisHelper.DelAsync(key);
    }

    /// <summary>
    /// 根据前缀删除缓存
    /// 慎用
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public long DelByPattern(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return default;

        var keys = RedisHelper.Scan(0, $"{pattern}*");
        if (keys != null && keys.Items.Length > 0)
        {
            return RedisHelper.Del(keys.Items);
        }

        return default;
    }

    /// <summary>
    /// 根据前缀删除缓存
    /// 慎用
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public async Task<long> DelByPatternAsync(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return default;

        var keys = await RedisHelper.ScanAsync(0, $"{pattern}*");
        if (keys != null && keys.Items.Length > 0)
        {
            return await RedisHelper.DelAsync(keys.Items);
        }

        return default;
    }

    /// <summary>
    /// 判断是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Exists(string key)
    {
        return RedisHelper.Exists(key);
    }

    /// <summary>
    /// 判断是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<bool> ExistsAsync(string key)
    {
        return await RedisHelper.ExistsAsync(key);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string Get(string key)
    {
        return RedisHelper.Get(key);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<string> GetAsync(string key)
    {
        return await RedisHelper.GetAsync(key);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public T Get<T>(string key)
    {
        return RedisHelper.Get<T>(key);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<T> GetAsync<T>(string key)
    {
        return await RedisHelper.GetAsync<T>(key);
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Set(string key, object value)
    {
        return RedisHelper.Set(key, value);
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task<bool> SetAsync(string key, object value)
    {
        return await RedisHelper.SetAsync(key, value);
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expireSeconds">单位秒</param>
    /// <returns></returns>
    public bool Set(string key, object value, int expireSeconds)
    {
        return RedisHelper.Set(key, value, expireSeconds);
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expireSeconds">单位秒</param>
    /// <returns></returns>
    public async Task<bool> SetAsync(string key, object value, int expireSeconds)
    {
        return await RedisHelper.SetAsync(key, value, expireSeconds);
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expireTimeSpan"></param>
    /// <returns></returns>
    public bool Set(string key, object value, TimeSpan expireTimeSpan)
    {
        return RedisHelper.Set(key, value, expireTimeSpan);
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expireTimeSpan"></param>
    /// <returns></returns>
    public async Task<bool> SetAsync(string key, object value, TimeSpan expireTimeSpan)
    {
        return await RedisHelper.SetAsync(key, value, expireTimeSpan);
    }

    /// <summary>
    /// 获取所有缓存Key
    /// 慎用
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllKeys()
    {
        var result = RedisHelper.Keys("*");
        return result.ToList();
    }

    /// <summary>
    /// 获取所有缓存Key
    /// 慎用
    /// </summary>
    /// <returns></returns>
    public async Task<List<string>> GetAllKeysAsync()
    {
        var result = await RedisHelper.KeysAsync("*");
        return result.ToList();
    }

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public string GetAndSet(string key, Func<string> func)
    {
        var result = RedisHelper.Get(key);

        if (IsEmpty(result))
        {
            result = func.Invoke();

            RedisHelper.Set(key, result);
        }

        return result;
    }

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public async Task<string> GetAndSetAsync(string key, Func<Task<string>> func)
    {
        var result = await RedisHelper.GetAsync(key);

        if (IsEmpty(result))
        {
            result = await func.Invoke();

            await RedisHelper.SetAsync(key, result);
        }

        return result;
    }

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public T GetAndSet<T>(string key, Func<T> func)
    {
        var result = RedisHelper.Get<T>(key);

        if (IsEmpty(result))
        {
            result = func.Invoke();

            RedisHelper.Set(key, result);
        }

        return result;
    }

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public async Task<T> GetAndSetAsync<T>(string key, Func<Task<T>> func)
    {
        var result = await RedisHelper.GetAsync<T>(key);

        if (IsEmpty(result))
        {
            result = await func.Invoke();

            await RedisHelper.SetAsync(key, result);
        }

        return result;
    }

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expireSeconds">单位秒</param>
    /// <param name="func"></param>
    /// <returns></returns>
    public string GetAndSet(string key, int expireSeconds, Func<string> func)
    {
        var result = RedisHelper.Get(key);

        if (IsEmpty(result))
        {
            result = func.Invoke();

            RedisHelper.Set(key, result, expireSeconds);
        }

        return result;
    }

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expireSeconds">单位秒</param>
    /// <param name="func"></param>
    /// <returns></returns>
    public async Task<string> GetAndSetAsync(string key, int expireSeconds, Func<Task<string>> func)
    {
        var result = await RedisHelper.GetAsync(key);

        if (IsEmpty(result))
        {
            result = await func.Invoke();

            await RedisHelper.SetAsync(key, result, expireSeconds);
        }

        return result;
    }

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="expireSeconds">单位秒</param>
    /// <param name="func"></param>
    /// <returns></returns>
    public T GetAndSet<T>(string key, int expireSeconds, Func<T> func)
    {
        var result = RedisHelper.Get<T>(key);

        if (IsEmpty(result))
        {
            result = func.Invoke();

            RedisHelper.Set(key, result, expireSeconds);
        }

        return result;
    }

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="expireSeconds">单位秒</param>
    /// <param name="func"></param>
    /// <returns></returns>
    public async Task<T> GetAndSetAsync<T>(string key, int expireSeconds, Func<Task<T>> func)
    {
        var result = await RedisHelper.GetAsync<T>(key);

        if (IsEmpty(result))
        {
            result = await func.Invoke();

            await RedisHelper.SetAsync(key, result, expireSeconds);
        }

        return result;
    }

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expireTimeSpan"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public string GetAndSet(string key, TimeSpan expireTimeSpan, Func<string> func)
    {
        var result = RedisHelper.Get(key);

        if (IsEmpty(result))
        {
            result = func.Invoke();

            RedisHelper.Set(key, result, expireTimeSpan);
        }

        return result;
    }

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expireTimeSpan"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public async Task<string> GetAndSetAsync(string key, TimeSpan expireTimeSpan, Func<Task<string>> func)
    {
        var result = await RedisHelper.GetAsync(key);

        if (IsEmpty(result))
        {
            result = await func.Invoke();

            await RedisHelper.SetAsync(key, result, expireTimeSpan);
        }

        return result;
    }

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="expireTimeSpan"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public T GetAndSet<T>(string key, TimeSpan expireTimeSpan, Func<T> func)
    {
        var result = RedisHelper.Get<T>(key);

        if (IsEmpty(result))
        {
            result = func.Invoke();

            RedisHelper.Set(key, result, expireTimeSpan);
        }

        return result;
    }

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="expireTimeSpan"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public async Task<T> GetAndSetAsync<T>(string key, TimeSpan expireTimeSpan, Func<Task<T>> func)
    {
        var result = await RedisHelper.GetAsync<T>(key);

        if (IsEmpty(result))
        {
            result = await func.Invoke();

            await RedisHelper.SetAsync(key, result, expireTimeSpan);
        }

        return result;
    }

    /// <summary>
    /// 将object转换为string，若转换失败，则返回""。不抛出异常。  
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    string ParseToString(object obj)
    {
        try
        {
            return obj == null ? string.Empty : obj.ToString();
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 检查 Object 或者 集合 是否为 NULL 或者 空集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    bool IsEmpty<T>(T value)
    {
        if (value == null)
        {
            return true;
        }

        if (string.IsNullOrEmpty(ParseToString(value)))
        {
            return true;
        }

        var type = typeof(T);

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            if (!(value is IList<object> list) || list.Count == 0)
            {
                return true;
            }

            return false;
        }

        if (value is IEnumerable<T> collection && !collection.Any())
        {
            return true;
        }

        return false;
    }
}