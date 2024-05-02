// Apache开源许可证
//
// 版权所有 © 2018-Now 小方
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
using Fast.Cache.Options;
using Fast.IaaS;
using Microsoft.Extensions.Options;

namespace Fast.Cache.Implements;

/// <summary>
/// <see cref="Cache"/> 默认缓存实现
/// </summary>
internal class Cache : Cache<DefaultCacheContextLocator>, ICache
{
    public Cache(IOptionsMonitor<List<RedisSettingsOptions>> redisSettings) : base(redisSettings)
    {
    }
}

/// <summary>
/// <see cref="Cache{CacheContextLocator}"/> 缓存实现
/// </summary>
[SuppressSniffer]
internal class Cache<CacheContextLocator> : ICache<CacheContextLocator>, IDisposable
    where CacheContextLocator : ICacheContextLocator, new()
{
    private readonly IDisposable _optionsReloadToken;

    /// <summary>
    /// CSRedis 缓存客户端
    /// </summary>
    private CSRedisClient _client;

    private readonly CacheContextLocator _cacheContextLocator = new();

    public Cache(IOptionsMonitor<List<RedisSettingsOptions>> redisSettings)
    {
        // 初始化服务
        Create(redisSettings.CurrentValue);

        // 监听配置更改
        _optionsReloadToken = redisSettings.OnChange(Create);
    }

    /// <summary>
    /// 创建/初始化服务
    /// </summary>
    private void Create(List<RedisSettingsOptions> redisSettings)
    {
        Debugging.Info("Creating cache......");

        // 根据缓存上下文定位器，获取到服务名称
        var redisSetting = redisSettings.FirstOrDefault(f => f.ServiceName == _cacheContextLocator.ServiceName);

        if (redisSetting != null)
        {
            // 组装连接字符串
            var connectionStr =
                $"{redisSetting.ServiceIp}:{redisSetting.Port ?? 6379},password={redisSetting.DbPwd},defaultDatabase={redisSetting.DbName ?? 0},prefix={redisSetting.Prefix},poolsize=50,ssl=false";

            _client?.Dispose();
            _client = new CSRedisClient(connectionStr);
        }
        else
        {
            throw new InvalidOperationException($"服务名称“{_cacheContextLocator.ServiceName}”不存在于“RedisSetting”配置节点中！");
        }
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public long Del(params string[] key)
    {
        return _client.Del(key);
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<long> DelAsync(params string[] key)
    {
        return await _client.DelAsync(key);
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

        var keys = _client.Scan(0, $"{pattern}*");
        if (keys != null && keys.Items.Length > 0)
        {
            return _client.Del(keys.Items);
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

        var keys = await _client.ScanAsync(0, $"{pattern}*");
        if (keys != null && keys.Items.Length > 0)
        {
            return await _client.DelAsync(keys.Items);
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
        return _client.Exists(key);
    }

    /// <summary>
    /// 判断是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<bool> ExistsAsync(string key)
    {
        return await _client.ExistsAsync(key);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string Get(string key)
    {
        return _client.Get(key);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<string> GetAsync(string key)
    {
        return await _client.GetAsync(key);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public T Get<T>(string key)
    {
        return _client.Get<T>(key);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<T> GetAsync<T>(string key)
    {
        return await _client.GetAsync<T>(key);
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Set(string key, object value)
    {
        return _client.Set(key, value);
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task<bool> SetAsync(string key, object value)
    {
        return await _client.SetAsync(key, value);
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
        return _client.Set(key, value, expireSeconds);
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
        return await _client.SetAsync(key, value, expireSeconds);
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
        return _client.Set(key, value, expireTimeSpan);
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
        return await _client.SetAsync(key, value, expireTimeSpan);
    }

    /// <summary>
    /// 获取所有缓存Key
    /// 慎用
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllKeys()
    {
        var result = _client.Keys("*");
        return result.ToList();
    }

    /// <summary>
    /// 获取所有缓存Key
    /// 慎用
    /// </summary>
    /// <returns></returns>
    public async Task<List<string>> GetAllKeysAsync()
    {
        var result = await _client.KeysAsync("*");
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
        var result = _client.Get(key);

        if (IsEmpty(result))
        {
            result = func.Invoke();

            _client.Set(key, result);
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
        var result = await _client.GetAsync(key);

        if (IsEmpty(result))
        {
            result = await func.Invoke();

            await _client.SetAsync(key, result);
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
        var result = _client.Get<T>(key);

        if (IsEmpty(result))
        {
            result = func.Invoke();

            _client.Set(key, result);
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
        var result = await _client.GetAsync<T>(key);

        if (IsEmpty(result))
        {
            result = await func.Invoke();

            await _client.SetAsync(key, result);
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
        var result = _client.Get(key);

        if (IsEmpty(result))
        {
            result = func.Invoke();

            _client.Set(key, result, expireSeconds);
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
        var result = await _client.GetAsync(key);

        if (result.IsEmpty())
        {
            result = await func.Invoke();

            await _client.SetAsync(key, result, expireSeconds);
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
        var result = _client.Get<T>(key);

        if (IsEmpty(result))
        {
            result = func.Invoke();

            _client.Set(key, result, expireSeconds);
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
        var result = await _client.GetAsync<T>(key);

        if (IsEmpty(result))
        {
            result = await func.Invoke();

            await _client.SetAsync(key, result, expireSeconds);
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
        var result = _client.Get(key);

        if (IsEmpty(result))
        {
            result = func.Invoke();

            _client.Set(key, result, expireTimeSpan);
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
        var result = await _client.GetAsync(key);

        if (IsEmpty(result))
        {
            result = await func.Invoke();

            await _client.SetAsync(key, result, expireTimeSpan);
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
        var result = _client.Get<T>(key);

        if (IsEmpty(result))
        {
            result = func.Invoke();

            _client.Set(key, result, expireTimeSpan);
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
        var result = await _client.GetAsync<T>(key);

        if (IsEmpty(result))
        {
            result = await func.Invoke();

            await _client.SetAsync(key, result, expireTimeSpan);
        }

        return result;
    }

    /// <summary>
    /// 检查 Object 或者 集合 是否为 NULL 或者 空集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    private static bool IsEmpty<T>(T value)
    {
        if (value == null)
        {
            return true;
        }

        try
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return true;
            }
        }
        catch
        {
            return true;
        }


        var type = typeof(T);

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            if (value is not IList<object> list || list.Count == 0)
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