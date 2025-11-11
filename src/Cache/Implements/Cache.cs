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

// ReSharper disable once CheckNamespace
namespace Fast.Cache;

/// <summary>
/// <see cref="Cache"/> 默认缓存实现
/// </summary>
internal class Cache : Cache<DefaultCacheContextLocator>, ICache
{
    public Cache(IOptionsMonitor<RedisSettingsOptions> redisSettings) : base(redisSettings)
    {
    }
}

/// <summary>
/// <see cref="Cache{CacheContextLocator}"/> 缓存实现
/// </summary>
internal class Cache<CacheContextLocator> : ICache<CacheContextLocator>, IDisposable
    where CacheContextLocator : ICacheContextLocator, new()
{
    internal readonly IDisposable _optionsReloadToken;

    /// <summary>
    /// 前缀
    /// </summary>
    public string Prefix { get; private set; }

    /// <summary>
    /// CSRedis 缓存客户端
    /// </summary>
    public CSRedisClient Client { get; private set; }

    /// <summary>
    /// 缓存上下文定位器
    /// </summary>
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
    /// 创建/初始化服务
    /// </summary>
    private void Create(RedisSettingsOptions redisSettings)
    {
        Debugging.Info($"Creating cache, Service = {ContextLocator.ServiceName}......");

        // 连接字符串
        string connectionStr;

        if (ContextLocator.ServiceName == "Default")
        {
            // 组装连接字符串
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

            // 组装连接字符串
            connectionStr =
                $"{redisServiceSettings.ServiceIp ?? redisSettings.ServiceIp}:{redisServiceSettings.Port ?? redisSettings.Port ?? 6379},password={redisServiceSettings.DbPwd ?? redisSettings.DbPwd},defaultDatabase={redisServiceSettings.DbName ?? redisSettings.DbName},prefix={redisServiceSettings.Prefix ?? redisSettings.Prefix},poolsize={redisServiceSettings.Poolsize ?? redisSettings.Poolsize},ssl={((redisServiceSettings.SSL ?? redisSettings.SSL) == true ? "true" : "false")}";

            Prefix = redisServiceSettings.Prefix;
        }

        Client?.Dispose();
        Client = new CSRedisClient(connectionStr);
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
        return Client.Del(key);
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<long> DelAsync(params string[] key)
    {
        return await Client.DelAsync(key);
    }

    /// <summary>
    /// 根据前缀删除缓存
    /// 慎用
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public long DelByPattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            return 0;

        // 判断是否已 * 结尾
        if (!pattern.EndsWith("*"))
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

    /// <summary>
    /// 根据前缀删除缓存
    /// 慎用
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 判断是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Exists(string key)
    {
        return Client.Exists(key);
    }

    /// <summary>
    /// 判断是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<bool> ExistsAsync(string key)
    {
        return await Client.ExistsAsync(key);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string Get(string key)
    {
        return Client.Get(key);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<string> GetAsync(string key)
    {
        return await Client.GetAsync(key);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public T Get<T>(string key)
    {
        return Client.Get<T>(key);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<T> GetAsync<T>(string key)
    {
        return await Client.GetAsync<T>(key);
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Set(string key, object value)
    {
        return Client.Set(key, value);
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task<bool> SetAsync(string key, object value)
    {
        return await Client.SetAsync(key, value);
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
        return Client.Set(key, value, expireSeconds);
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
        return await Client.SetAsync(key, value, expireSeconds);
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
        return Client.Set(key, value, expireTimeSpan);
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
        return await Client.SetAsync(key, value, expireTimeSpan);
    }

    /// <summary>
    /// 获取所有缓存Key
    /// 慎用
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllKeys()
    {
        var result = Client.Keys("*");
        return result.ToList();
    }

    /// <summary>
    /// 获取所有缓存Key
    /// 慎用
    /// </summary>
    /// <returns></returns>
    public async Task<List<string>> GetAllKeysAsync()
    {
        var result = await Client.KeysAsync("*");
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
        var result = Client.Get(key);

        if (result is "×Null×")
        {
            return null;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.TryLock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    result = func.Invoke();

                    // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                    if (IsEmpty(result))
                    {
                        Client.Set(key, "×Null×", TimeSpan.FromHours(2));
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

                // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                if (IsEmpty(result))
                {
                    Client.Set(key, "×Null×", TimeSpan.FromHours(2));
                }
                else
                {
                    Client.Set(key, result);
                }
            }
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
        var result = await Client.GetAsync(key);

        if (result is "×Null×")
        {
            return null;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.TryLock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    result = await func.Invoke();

                    // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                    if (IsEmpty(result))
                    {
                        await Client.SetAsync(key, "×Null×", TimeSpan.FromHours(2));
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

                // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                if (IsEmpty(result))
                {
                    await Client.SetAsync(key, "×Null×", TimeSpan.FromHours(2));
                }
                else
                {
                    await Client.SetAsync(key, result);
                }
            }
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
        var result = Client.Get<T>(key);

        if (result is "×Null×")
        {
            return default;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.TryLock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    result = func.Invoke();

                    // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                    if (IsEmpty(result))
                    {
                        Client.Set(key, "×Null×", TimeSpan.FromHours(2));
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

                // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                if (IsEmpty(result))
                {
                    Client.Set(key, "×Null×", TimeSpan.FromHours(2));
                }
                else
                {
                    Client.Set(key, result);
                }
            }
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
        var result = await Client.GetAsync<T>(key);

        if (result is "×Null×")
        {
            return default;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.TryLock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    result = await func.Invoke();

                    // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                    if (IsEmpty(result))
                    {
                        await Client.SetAsync(key, "×Null×", TimeSpan.FromHours(2));
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

                // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                if (IsEmpty(result))
                {
                    await Client.SetAsync(key, "×Null×", TimeSpan.FromHours(2));
                }
                else
                {
                    await Client.SetAsync(key, result);
                }
            }
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
        var result = Client.Get(key);

        if (result is "×Null×")
        {
            return null;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.TryLock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    result = func.Invoke();

                    // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                    if (IsEmpty(result))
                    {
                        Client.Set(key, "×Null×", TimeSpan.FromHours(2));
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

                // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                if (IsEmpty(result))
                {
                    Client.Set(key, "×Null×", TimeSpan.FromHours(2));
                }
                else
                {
                    Client.Set(key, result, expireSeconds);
                }
            }
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
        var result = await Client.GetAsync(key);

        if (result is "×Null×")
        {
            return null;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.TryLock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    result = await func.Invoke();

                    // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                    if (IsEmpty(result))
                    {
                        await Client.SetAsync(key, "×Null×", TimeSpan.FromHours(2));
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

                // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                if (IsEmpty(result))
                {
                    await Client.SetAsync(key, "×Null×", TimeSpan.FromHours(2));
                }
                else
                {
                    await Client.SetAsync(key, result, expireSeconds);
                }
            }
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
        var result = Client.Get<T>(key);

        if (result is "×Null×")
        {
            return default;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.TryLock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    result = func.Invoke();

                    // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                    if (IsEmpty(result))
                    {
                        Client.Set(key, "×Null×", TimeSpan.FromHours(2));
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

                // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                if (IsEmpty(result))
                {
                    Client.Set(key, "×Null×", TimeSpan.FromHours(2));
                }
                else
                {
                    Client.Set(key, result, expireSeconds);
                }
            }
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
        var result = await Client.GetAsync<T>(key);

        if (result is "×Null×")
        {
            return default;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.TryLock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    result = await func.Invoke();

                    // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                    if (IsEmpty(result))
                    {
                        await Client.SetAsync(key, "×Null×", TimeSpan.FromHours(2));
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

                // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                if (IsEmpty(result))
                {
                    await Client.SetAsync(key, "×Null×", TimeSpan.FromHours(2));
                }
                else
                {
                    await Client.SetAsync(key, result, expireSeconds);
                }
            }
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
        var result = Client.Get(key);

        if (result is "×Null×")
        {
            return null;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.TryLock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    result = func.Invoke();

                    // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                    if (IsEmpty(result))
                    {
                        Client.Set(key, "×Null×", TimeSpan.FromHours(2));
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

                // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                if (IsEmpty(result))
                {
                    Client.Set(key, "×Null×", TimeSpan.FromHours(2));
                }
                else
                {
                    Client.Set(key, result, expireTimeSpan);
                }
            }
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
        var result = await Client.GetAsync(key);

        if (result is "×Null×")
        {
            return null;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.TryLock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    result = await func.Invoke();

                    // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                    if (IsEmpty(result))
                    {
                        await Client.SetAsync(key, "×Null×", TimeSpan.FromHours(2));
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

                // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                if (IsEmpty(result))
                {
                    await Client.SetAsync(key, "×Null×", TimeSpan.FromHours(2));
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
    /// 获取并且设置缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="expireTimeSpan"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public T GetAndSet<T>(string key, TimeSpan expireTimeSpan, Func<T> func)
    {
        var result = Client.Get<T>(key);

        if (result is "×Null×")
        {
            return default;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.TryLock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    result = func.Invoke();

                    // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                    if (IsEmpty(result))
                    {
                        Client.Set(key, "×Null×", TimeSpan.FromHours(2));
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

                // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                if (IsEmpty(result))
                {
                    Client.Set(key, "×Null×", TimeSpan.FromHours(2));
                }
                else
                {
                    Client.Set(key, result, expireTimeSpan);
                }
            }
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
        var result = await Client.GetAsync<T>(key);

        if (result is "×Null×")
        {
            return default;
        }

        if (IsEmpty(result))
        {
            var acquired = Client.TryLock($"{key}_lock", 5);
            if (acquired != null)
            {
                try
                {
                    result = await func.Invoke();

                    // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                    if (IsEmpty(result))
                    {
                        await Client.SetAsync(key, "×Null×", TimeSpan.FromHours(2));
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

                // 如果返回空，则默认写入"×Null×"，缓存2小时，防止缓存击穿
                if (IsEmpty(result))
                {
                    await Client.SetAsync(key, "×Null×", TimeSpan.FromHours(2));
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
    /// 检查 Object 或者 集合 是否为 NULL 或者 空集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    private bool IsEmpty<T>(T value)
    {
        if (value == null)
        {
            return true;
        }

        try
        {
            if (string.IsNullOrWhiteSpace(value.ToString()))
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
            if (value is not IList list || list.Count == 0)
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