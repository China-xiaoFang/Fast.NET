// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using CSRedis;
using Fast.Cache.Extensions;
using Fast.Cache.Helpers;
using Fast.IaaS;
using Microsoft.Extensions.Configuration;
using ValidateExtension = Fast.Cache.Extensions.ValidateExtension;

namespace Fast.Cache.Implements;

/// <summary>
/// 缓存实现
/// </summary>
[SuppressSniffer]
internal class Cache : ICache
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration">连接字符串</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Cache(IConfiguration configuration)
    {
        // 从配置文件中读取 Redis 缓存字符串
        var connectionString = configuration["RedisConnectionString"];

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), "Inject cache is “RedisConnectionString” empty.");
        }

        var csRedis = new CSRedisClient(connectionString);
        FastRedisHelper.Initialization(csRedis);
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public long Del(params string[] key)
    {
        return FastRedisHelper.Del(key);
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<long> DelAsync(params string[] key)
    {
        return await FastRedisHelper.DelAsync(key);
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

        var keys = FastRedisHelper.Scan(0, $"{pattern}*");
        if (keys != null && keys.Items.Length > 0)
        {
            return FastRedisHelper.Del(keys.Items);
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

        var keys = await FastRedisHelper.ScanAsync(0, $"{pattern}*");
        if (keys != null && keys.Items.Length > 0)
        {
            return await FastRedisHelper.DelAsync(keys.Items);
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
        return FastRedisHelper.Exists(key);
    }

    /// <summary>
    /// 判断是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<bool> ExistsAsync(string key)
    {
        return await FastRedisHelper.ExistsAsync(key);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string Get(string key)
    {
        return FastRedisHelper.Get(key);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<string> GetAsync(string key)
    {
        return await FastRedisHelper.GetAsync(key);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public T Get<T>(string key)
    {
        return FastRedisHelper.Get<T>(key);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<T> GetAsync<T>(string key)
    {
        return await FastRedisHelper.GetAsync<T>(key);
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Set(string key, object value)
    {
        return FastRedisHelper.Set(key, value);
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task<bool> SetAsync(string key, object value)
    {
        return await FastRedisHelper.SetAsync(key, value);
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
        return FastRedisHelper.Set(key, value, expireSeconds);
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
        return await FastRedisHelper.SetAsync(key, value, expireSeconds);
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
        return FastRedisHelper.Set(key, value, expireTimeSpan);
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
        return await FastRedisHelper.SetAsync(key, value, expireTimeSpan);
    }

    /// <summary>
    /// 获取所有缓存Key
    /// 慎用
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllKeys()
    {
        var result = FastRedisHelper.Keys("*");
        return result.ToList();
    }

    /// <summary>
    /// 获取所有缓存Key
    /// 慎用
    /// </summary>
    /// <returns></returns>
    public async Task<List<string>> GetAllKeysAsync()
    {
        var result = await FastRedisHelper.KeysAsync("*");
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
        var result = FastRedisHelper.Get(key);

        if (ValidateExtension.IsEmpty(result))
        {
            result = func.Invoke();

            FastRedisHelper.Set(key, result);
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
        var result = await FastRedisHelper.GetAsync(key);

        if (ValidateExtension.IsEmpty(result))
        {
            result = await func.Invoke();

            await FastRedisHelper.SetAsync(key, result);
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
        var result = FastRedisHelper.Get<T>(key);

        if (result.IsEmpty())
        {
            result = func.Invoke();

            FastRedisHelper.Set(key, result);
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
        var result = await FastRedisHelper.GetAsync<T>(key);

        if (result.IsEmpty())
        {
            result = await func.Invoke();

            await FastRedisHelper.SetAsync(key, result);
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
        var result = FastRedisHelper.Get(key);

        if (ValidateExtension.IsEmpty(result))
        {
            result = func.Invoke();

            FastRedisHelper.Set(key, result, expireSeconds);
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
        var result = await FastRedisHelper.GetAsync(key);

        if (ValidateExtension.IsEmpty(result))
        {
            result = await func.Invoke();

            await FastRedisHelper.SetAsync(key, result, expireSeconds);
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
        var result = FastRedisHelper.Get<T>(key);

        if (result.IsEmpty())
        {
            result = func.Invoke();

            FastRedisHelper.Set(key, result, expireSeconds);
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
        var result = await FastRedisHelper.GetAsync<T>(key);

        if (result.IsEmpty())
        {
            result = await func.Invoke();

            await FastRedisHelper.SetAsync(key, result, expireSeconds);
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
        var result = FastRedisHelper.Get(key);

        if (ValidateExtension.IsEmpty(result))
        {
            result = func.Invoke();

            FastRedisHelper.Set(key, result, expireTimeSpan);
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
        var result = await FastRedisHelper.GetAsync(key);

        if (ValidateExtension.IsEmpty(result))
        {
            result = await func.Invoke();

            await FastRedisHelper.SetAsync(key, result, expireTimeSpan);
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
        var result = FastRedisHelper.Get<T>(key);

        if (result.IsEmpty())
        {
            result = func.Invoke();

            FastRedisHelper.Set(key, result, expireTimeSpan);
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
        var result = await FastRedisHelper.GetAsync<T>(key);

        if (result.IsEmpty())
        {
            result = await func.Invoke();

            await FastRedisHelper.SetAsync(key, result, expireTimeSpan);
        }

        return result;
    }
}