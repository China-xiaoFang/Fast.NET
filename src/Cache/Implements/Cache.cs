// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using System.Collections;
using CSRedis;
using Microsoft.Extensions.Options;

namespace Fast.Cache;

/// <inheritdoc cref="ICache" />
internal sealed class Cache : Cache<DefaultCacheContextLocator>, ICache
{
    public Cache(IOptionsMonitor<RedisSettingsOptions> redisSettings) : base(redisSettings)
    {
    }
}

/// <inheritdoc cref="ICache{CacheContextLocator}" />
internal class Cache<CacheContextLocator> : ICache<CacheContextLocator>, IDisposable
    where CacheContextLocator : ICacheContextLocator, new()
{
    internal readonly IDisposable _optionsReloadToken;

    /// <summary>
    /// 空值。
    /// </summary>
    internal const string _nullValue = "×Null×";

    /// <inheritdoc />
    public string Prefix { get; private set; }

    /// <inheritdoc />
    public CSRedisClient Client { get; private set; }

    /// <inheritdoc />
    public CacheContextLocator ContextLocator { get; }

    public Cache(IOptionsMonitor<RedisSettingsOptions> redisSettings)
    {
        ContextLocator = new CacheContextLocator();

        // 初始化服务
        Create(redisSettings.CurrentValue);

        // 监听配置更改
        _optionsReloadToken = redisSettings.OnChange(Create);
    }

    /// <summary>
    /// 创建/初始化服务。
    /// </summary>
    /// <param name="redisSettings">redis Settings 配置。</param>
    private void Create(RedisSettingsOptions redisSettings)
    {
        Debugging.Info($"Creating cache, Service = {ContextLocator.ServiceName}......");

        string connectionStr;

        if (ContextLocator.ServiceName == "Default")
        {
            connectionStr =
                $"{redisSettings.ServiceIp}:{redisSettings.Port ?? 6379},password={redisSettings.DbPwd},defaultDatabase={redisSettings.DbName},prefix={redisSettings.Prefix},poolsize={redisSettings.Poolsize},ssl={(redisSettings.SSL == true ? "true" : "false")}";

            Prefix = redisSettings.Prefix;
        }
        else
        {
            // 根据缓存上下文定位器，获取到服务名称
            var redisServiceSettings = redisSettings.Services.FirstOrDefault(f => f.ServiceName == ContextLocator.ServiceName);

            if (redisServiceSettings == null)
            {
                throw new InvalidOperationException($"服务名称“{ContextLocator.ServiceName}”不存在于“RedisSetting”配置节点中！");
            }

            connectionStr =
                $"{redisServiceSettings.ServiceIp ?? redisSettings.ServiceIp}:{redisServiceSettings.Port ?? redisSettings.Port ?? 6379},password={redisServiceSettings.DbPwd ?? redisSettings.DbPwd},defaultDatabase={redisServiceSettings.DbName ?? redisSettings.DbName},prefix={redisServiceSettings.Prefix ?? redisSettings.Prefix},poolsize={redisServiceSettings.Poolsize ?? redisSettings.Poolsize},ssl={((redisServiceSettings.SSL ?? redisSettings.SSL) == true ? "true" : "false")}";

            Prefix = redisServiceSettings.Prefix ?? redisSettings.Prefix;
        }

        Client?.Dispose();
        Client = new CSRedisClient(connectionStr);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
        Client?.Dispose();
    }

    /// <inheritdoc />
    public long Del(params string[] key)
    {
        return Client.Del(key);
    }

    /// <inheritdoc />
    public async Task<long> DelAsync(params string[] key)
    {
        return await Client.DelAsync(key);
    }

    /// <inheritdoc />
    public long DelByPattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            return 0;

        // 判断是否已 * 结尾
        if (!pattern.EndsWith('*'))
        {
            pattern += "*";
        }

        // 处理前缀，这里 Scan 扫描不会默认带前缀
        if (!string.IsNullOrWhiteSpace(Prefix))
        {
            pattern = Prefix + pattern;
        }

        var cursor = 0L;
        var totalDeleted = 0L;

        do
        {
            // 每次返回最多 100 个
            var keys = Client.Scan(cursor, pattern, 100);
            if (keys == null)
            {
                cursor = 0;
                continue;
            }

            cursor = keys.Cursor;
            if (keys.Items.Length > 0)
            {
                var keyItems = keys.Items;
                // 处理前缀，这里 Del 删除又会默认带前缀
                if (!string.IsNullOrWhiteSpace(Prefix))
                {
                    keyItems = keys.Items.Select(sl => sl[Prefix.Length..])
                        .ToArray();
                }

                totalDeleted += Client.Del(keyItems);
            }
        } while (cursor != 0);

        return totalDeleted;
    }

    /// <inheritdoc />
    public async Task<long> DelByPatternAsync(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            return 0;

        // 判断是否已 * 结尾
        if (!pattern.EndsWith("*"))
        {
            pattern += "*";
        }

        // 处理前缀，这里 ScanAsync 扫描不会默认带前缀
        if (!string.IsNullOrWhiteSpace(Prefix))
        {
            pattern = Prefix + pattern;
        }

        var cursor = 0L;
        var totalDeleted = 0L;

        do
        {
            // 每次返回最多 100 个
            var keys = await Client.ScanAsync(cursor, pattern, 100);
            if (keys == null)
            {
                cursor = 0;
                continue;
            }

            cursor = keys.Cursor;
            if (keys.Items.Length > 0)
            {
                var keyItems = keys.Items;
                // 处理前缀，这里 DelAsync 删除又会默认带前缀
                if (!string.IsNullOrWhiteSpace(Prefix))
                {
                    keyItems = keys.Items.Select(sl => sl[Prefix.Length..])
                        .ToArray();
                }

                totalDeleted += await Client.DelAsync(keyItems);
            }
        } while (cursor != 0);

        return totalDeleted;
    }

    /// <inheritdoc />
    public bool Exists(string key)
    {
        return Client.Exists(key);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string key)
    {
        return await Client.ExistsAsync(key);
    }

    /// <inheritdoc />
    public string Get(string key)
    {
        return Client.Get(key);
    }

    /// <inheritdoc />
    public async Task<string> GetAsync(string key)
    {
        return await Client.GetAsync(key);
    }

    /// <inheritdoc />
    public T Get<T>(string key)
    {
        return Client.Get<T>(key);
    }

    /// <inheritdoc />
    public async Task<T> GetAsync<T>(string key)
    {
        return await Client.GetAsync<T>(key);
    }

    /// <inheritdoc />
    public bool Set(string key, object value)
    {
        return Client.Set(key, value);
    }

    /// <inheritdoc />
    public async Task<bool> SetAsync(string key, object value)
    {
        return await Client.SetAsync(key, value);
    }

    /// <inheritdoc />
    public bool Set(string key, object value, int expireSeconds)
    {
        return Client.Set(key, value, expireSeconds);
    }

    /// <inheritdoc />
    public async Task<bool> SetAsync(string key, object value, int expireSeconds)
    {
        return await Client.SetAsync(key, value, expireSeconds);
    }

    /// <inheritdoc />
    public bool Set(string key, object value, TimeSpan expireTimeSpan)
    {
        return Client.Set(key, value, expireTimeSpan);
    }

    /// <inheritdoc />
    public async Task<bool> SetAsync(string key, object value, TimeSpan expireTimeSpan)
    {
        return await Client.SetAsync(key, value, expireTimeSpan);
    }

    /// <inheritdoc />
    public List<string> GetAllKeys()
    {
        var result = Client.Keys("*");
        return result.ToList();
    }

    /// <inheritdoc />
    public async Task<List<string>> GetAllKeysAsync()
    {
        var result = await Client.KeysAsync("*");
        return result.ToList();
    }

    /// <inheritdoc />
    public string GetAndSet(string key, Func<string> func)
    {
        var result = Client.Get(key);

        if (result is _nullValue)
        {
            return null;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.Lock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    // 等待锁期间其他实例可能已经回填缓存，进入临界区后必须再次检查。
                    if (TryGetCachedValue(key, out result))
                        return result;

                    result = func.Invoke();

                    // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                    if (IsEmpty(result))
                    {
                        Client.Set(key, _nullValue, TimeSpan.FromHours(2));
                    }
                    else
                    {
                        Client.Set(key, result);
                    }
                }
                finally
                {
                    acquired.Unlock();
                }
            }
            else
            {
                result = func.Invoke();

                // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                if (IsEmpty(result))
                {
                    Client.Set(key, _nullValue, TimeSpan.FromHours(2));
                }
                else
                {
                    Client.Set(key, result);
                }
            }
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<string> GetAndSetAsync(string key, Func<Task<string>> func)
    {
        var result = await Client.GetAsync(key);

        if (result is _nullValue)
        {
            return null;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.Lock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    // 等待锁期间其他实例可能已经回填缓存，进入临界区后必须再次检查。
                    var cachedValue = await TryGetCachedValueAsync(key, result)
                        .ConfigureAwait(false);
                    if (cachedValue.Found)
                        return cachedValue.Value;

                    result = await func.Invoke();

                    // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                    if (IsEmpty(result))
                    {
                        await Client.SetAsync(key, _nullValue, TimeSpan.FromHours(2));
                    }
                    else
                    {
                        await Client.SetAsync(key, result);
                    }
                }
                finally
                {
                    acquired.Unlock();
                }
            }
            else
            {
                result = await func.Invoke();

                // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                if (IsEmpty(result))
                {
                    await Client.SetAsync(key, _nullValue, TimeSpan.FromHours(2));
                }
                else
                {
                    await Client.SetAsync(key, result);
                }
            }
        }

        return result;
    }

    /// <inheritdoc />
    public T GetAndSet<T>(string key, Func<T> func)
    {
        var value = Client.Get(key);

        if (value is _nullValue)
        {
            return default;
        }

        var result = Client.Get<T>(key);

        if (IsEmpty(result))
        {
            var acquired = Client.Lock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    // 等待锁期间其他实例可能已经回填缓存，进入临界区后必须再次检查。
                    if (TryGetCachedValue(key, out result))
                        return result;

                    result = func.Invoke();

                    // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                    if (IsEmpty(result))
                    {
                        Client.Set(key, _nullValue, TimeSpan.FromHours(2));
                    }
                    else
                    {
                        Client.Set(key, result);
                    }
                }
                finally
                {
                    acquired.Unlock();
                }
            }
            else
            {
                result = func.Invoke();

                // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                if (IsEmpty(result))
                {
                    Client.Set(key, _nullValue, TimeSpan.FromHours(2));
                }
                else
                {
                    Client.Set(key, result);
                }
            }
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<T> GetAndSetAsync<T>(string key, Func<Task<T>> func)
    {
        var value = await Client.GetAsync(key);

        if (value is _nullValue)
        {
            return default;
        }

        var result = await Client.GetAsync<T>(key);

        if (result is _nullValue)
        {
            return default;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.Lock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    // 等待锁期间其他实例可能已经回填缓存，进入临界区后必须再次检查。
                    var cachedValue = await TryGetCachedValueAsync(key, result)
                        .ConfigureAwait(false);
                    if (cachedValue.Found)
                        return cachedValue.Value;

                    result = await func.Invoke();

                    // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                    if (IsEmpty(result))
                    {
                        await Client.SetAsync(key, _nullValue, TimeSpan.FromHours(2));
                    }
                    else
                    {
                        await Client.SetAsync(key, result);
                    }
                }
                finally
                {
                    acquired.Unlock();
                }
            }
            else
            {
                result = await func.Invoke();

                // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                if (IsEmpty(result))
                {
                    await Client.SetAsync(key, _nullValue, TimeSpan.FromHours(2));
                }
                else
                {
                    await Client.SetAsync(key, result);
                }
            }
        }

        return result;
    }

    /// <inheritdoc />
    public string GetAndSet(string key, int expireSeconds, Func<string> func)
    {
        var result = Client.Get(key);

        if (result is _nullValue)
        {
            return null;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.Lock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    // 等待锁期间其他实例可能已经回填缓存，进入临界区后必须再次检查。
                    if (TryGetCachedValue(key, out result))
                        return result;

                    result = func.Invoke();

                    // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                    if (IsEmpty(result))
                    {
                        Client.Set(key, _nullValue, TimeSpan.FromHours(2));
                    }
                    else
                    {
                        Client.Set(key, result, expireSeconds);
                    }
                }
                finally
                {
                    acquired.Unlock();
                }
            }
            else
            {
                result = func.Invoke();

                // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                if (IsEmpty(result))
                {
                    Client.Set(key, _nullValue, TimeSpan.FromHours(2));
                }
                else
                {
                    Client.Set(key, result, expireSeconds);
                }
            }
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<string> GetAndSetAsync(string key, int expireSeconds, Func<Task<string>> func)
    {
        var result = await Client.GetAsync(key);

        if (result is _nullValue)
        {
            return null;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.Lock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    // 等待锁期间其他实例可能已经回填缓存，进入临界区后必须再次检查。
                    var cachedValue = await TryGetCachedValueAsync(key, result)
                        .ConfigureAwait(false);
                    if (cachedValue.Found)
                        return cachedValue.Value;

                    result = await func.Invoke();

                    // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                    if (IsEmpty(result))
                    {
                        await Client.SetAsync(key, _nullValue, TimeSpan.FromHours(2));
                    }
                    else
                    {
                        await Client.SetAsync(key, result, expireSeconds);
                    }
                }
                finally
                {
                    acquired.Unlock();
                }
            }
            else
            {
                result = await func.Invoke();

                // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                if (IsEmpty(result))
                {
                    await Client.SetAsync(key, _nullValue, TimeSpan.FromHours(2));
                }
                else
                {
                    await Client.SetAsync(key, result, expireSeconds);
                }
            }
        }

        return result;
    }

    /// <inheritdoc />
    public T GetAndSet<T>(string key, int expireSeconds, Func<T> func)
    {
        var value = Client.Get(key);

        if (value is _nullValue)
        {
            return default;
        }

        var result = Client.Get<T>(key);

        if (IsEmpty(result))
        {
            var acquired = Client.Lock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    // 等待锁期间其他实例可能已经回填缓存，进入临界区后必须再次检查。
                    if (TryGetCachedValue(key, out result))
                        return result;

                    result = func.Invoke();

                    // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                    if (IsEmpty(result))
                    {
                        Client.Set(key, _nullValue, TimeSpan.FromHours(2));
                    }
                    else
                    {
                        Client.Set(key, result, expireSeconds);
                    }
                }
                finally
                {
                    acquired.Unlock();
                }
            }
            else
            {
                result = func.Invoke();

                // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                if (IsEmpty(result))
                {
                    Client.Set(key, _nullValue, TimeSpan.FromHours(2));
                }
                else
                {
                    Client.Set(key, result, expireSeconds);
                }
            }
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<T> GetAndSetAsync<T>(string key, int expireSeconds, Func<Task<T>> func)
    {
        var value = await Client.GetAsync(key);

        if (value is _nullValue)
        {
            return default;
        }

        var result = await Client.GetAsync<T>(key);

        if (IsEmpty(result))
        {
            var acquired = Client.Lock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    // 等待锁期间其他实例可能已经回填缓存，进入临界区后必须再次检查。
                    var cachedValue = await TryGetCachedValueAsync(key, result)
                        .ConfigureAwait(false);
                    if (cachedValue.Found)
                        return cachedValue.Value;

                    result = await func.Invoke();

                    // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                    if (IsEmpty(result))
                    {
                        await Client.SetAsync(key, _nullValue, TimeSpan.FromHours(2));
                    }
                    else
                    {
                        await Client.SetAsync(key, result, expireSeconds);
                    }
                }
                finally
                {
                    acquired.Unlock();
                }
            }
            else
            {
                result = await func.Invoke();

                // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                if (IsEmpty(result))
                {
                    await Client.SetAsync(key, _nullValue, TimeSpan.FromHours(2));
                }
                else
                {
                    await Client.SetAsync(key, result, expireSeconds);
                }
            }
        }

        return result;
    }

    /// <inheritdoc />
    public string GetAndSet(string key, TimeSpan expireTimeSpan, Func<string> func)
    {
        var result = Client.Get(key);

        if (result is _nullValue)
        {
            return null;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.Lock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    // 等待锁期间其他实例可能已经回填缓存，进入临界区后必须再次检查。
                    if (TryGetCachedValue(key, out result))
                        return result;

                    result = func.Invoke();

                    // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                    if (IsEmpty(result))
                    {
                        Client.Set(key, _nullValue, TimeSpan.FromHours(2));
                    }
                    else
                    {
                        Client.Set(key, result, expireTimeSpan);
                    }
                }
                finally
                {
                    acquired.Unlock();
                }
            }
            else
            {
                result = func.Invoke();

                // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                if (IsEmpty(result))
                {
                    Client.Set(key, _nullValue, TimeSpan.FromHours(2));
                }
                else
                {
                    Client.Set(key, result, expireTimeSpan);
                }
            }
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<string> GetAndSetAsync(string key, TimeSpan expireTimeSpan, Func<Task<string>> func)
    {
        var result = await Client.GetAsync(key);

        if (result is _nullValue)
        {
            return null;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.Lock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    // 等待锁期间其他实例可能已经回填缓存，进入临界区后必须再次检查。
                    var cachedValue = await TryGetCachedValueAsync(key, result)
                        .ConfigureAwait(false);
                    if (cachedValue.Found)
                        return cachedValue.Value;

                    result = await func.Invoke();

                    // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                    if (IsEmpty(result))
                    {
                        await Client.SetAsync(key, _nullValue, TimeSpan.FromHours(2));
                    }
                    else
                    {
                        await Client.SetAsync(key, result, expireTimeSpan);
                    }
                }
                finally
                {
                    acquired.Unlock();
                }
            }
            else
            {
                result = await func.Invoke();

                // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                if (IsEmpty(result))
                {
                    await Client.SetAsync(key, _nullValue, TimeSpan.FromHours(2));
                }
                else
                {
                    await Client.SetAsync(key, result, expireTimeSpan);
                }
            }
        }

        return result;
    }

    /// <inheritdoc />
    public T GetAndSet<T>(string key, TimeSpan expireTimeSpan, Func<T> func)
    {
        var value = Client.Get(key);

        if (value is _nullValue)
        {
            return default;
        }

        var result = Client.Get<T>(key);

        if (IsEmpty(result))
        {
            var acquired = Client.Lock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    // 等待锁期间其他实例可能已经回填缓存，进入临界区后必须再次检查。
                    if (TryGetCachedValue(key, out result))
                        return result;

                    result = func.Invoke();

                    // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                    if (IsEmpty(result))
                    {
                        Client.Set(key, _nullValue, TimeSpan.FromHours(2));
                    }
                    else
                    {
                        Client.Set(key, result, expireTimeSpan);
                    }
                }
                finally
                {
                    acquired.Unlock();
                }
            }
            else
            {
                result = func.Invoke();

                // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                if (IsEmpty(result))
                {
                    Client.Set(key, _nullValue, TimeSpan.FromHours(2));
                }
                else
                {
                    Client.Set(key, result, expireTimeSpan);
                }
            }
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<T> GetAndSetAsync<T>(string key, TimeSpan expireTimeSpan, Func<Task<T>> func)
    {
        var value = await Client.GetAsync(key);

        if (value is _nullValue)
        {
            return default;
        }

        var result = await Client.GetAsync<T>(key);

        if (IsEmpty(result))
        {
            var acquired = Client.Lock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    // 等待锁期间其他实例可能已经回填缓存，进入临界区后必须再次检查。
                    var cachedValue = await TryGetCachedValueAsync(key, result)
                        .ConfigureAwait(false);
                    if (cachedValue.Found)
                        return cachedValue.Value;

                    result = await func.Invoke();

                    // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                    if (IsEmpty(result))
                    {
                        await Client.SetAsync(key, _nullValue, TimeSpan.FromHours(2));
                    }
                    else
                    {
                        await Client.SetAsync(key, result, expireTimeSpan);
                    }
                }
                finally
                {
                    acquired.Unlock();
                }
            }
            else
            {
                result = await func.Invoke();

                // 缓存未命中时写入 _nullValue 空值哨兵并保留 2 小时，避免缓存穿透。
                if (IsEmpty(result))
                {
                    await Client.SetAsync(key, _nullValue, TimeSpan.FromHours(2));
                }
                else
                {
                    await Client.SetAsync(key, result, expireTimeSpan);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 尝试读取缓存，并区分“缓存未命中”和“已缓存空值”。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="result">缓存值读取结果。</param>
    /// <typeparam name="T">缓存值的类型。</typeparam>
    /// <returns>成功取得目标值时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
    private bool TryGetCachedValue<T>(string key, out T result)
    {
        var rawValue = Client.Get(key);
        if (string.Equals(rawValue, _nullValue, StringComparison.Ordinal))
        {
            result = default;
            return true;
        }

        result = typeof(T) == typeof(string) ? (T) (object) rawValue : Client.Get<T>(key);
        return !IsEmpty(result);
    }

    /// <summary>
    /// 异步尝试读取缓存，并区分“缓存未命中”和“已缓存空值”。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="_">扩展方法接收者；该值不参与输出。</param>
    /// <typeparam name="T">缓存值的类型。</typeparam>
    /// <returns>表示异步尝试读取缓存，并区分“缓存未命中”和“已缓存空值”的任务，任务结果为尝试读取缓存，并区分“缓存未命中”和“已缓存空值”。</returns>
    private async Task<(bool Found, T Value)> TryGetCachedValueAsync<T>(string key, T _)
    {
        var rawValue = await Client.GetAsync(key)
            .ConfigureAwait(false);
        if (string.Equals(rawValue, _nullValue, StringComparison.Ordinal))
            return (true, default);

        var result = typeof(T) == typeof(string)
            ? (T) (object) rawValue
            : await Client.GetAsync<T>(key)
                .ConfigureAwait(false);
        return (!IsEmpty(result), result);
    }

    /// <summary>
    /// 检查对象或集合是否为 <see langword="null"/>、空字符串或空集合。
    /// </summary>
    /// <param name="value">要检查的值。</param>
    /// <typeparam name="T">要检查是否为空的值类型。</typeparam>
    /// <returns>满足条件时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
    private static bool IsEmpty<T>(T value)
    {
        if (value == null)
            return true;

        if (value is string text)
            return string.IsNullOrWhiteSpace(text);

        if (value is ICollection collection)
            return collection.Count == 0;

        if (value is IEnumerable enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            try
            {
                return !enumerator.MoveNext();
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }
        }

        return false;
    }
}
