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

using CSRedis;

// ReSharper disable once CheckNamespace
namespace Fast.Cache;

/// <summary>
/// <see cref="ICache"/> 默认缓存服务接口
/// </summary>
public interface ICache : ICache<DefaultCacheContextLocator>
{
}

/// <summary>
/// <see cref="ICache{CacheContextLocator}"/> 缓存服务接口
/// </summary>
public interface ICache<out CacheContextLocator> where CacheContextLocator : ICacheContextLocator, new()
{
    /// <summary>
    /// 前缀
    /// </summary>
    public string Prefix { get; }

    /// <summary>
    /// CSRedis 缓存客户端
    /// </summary>
    CSRedisClient Client { get; }

    /// <summary>
    /// 缓存上下文定位器
    /// </summary>
    CacheContextLocator ContextLocator { get; }

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    long Del(params string[] key);

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<long> DelAsync(params string[] key);

    /// <summary>
    /// 根据前缀删除缓存
    /// 慎用
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    long DelByPattern(string pattern);

    /// <summary>
    /// 根据前缀删除缓存
    /// 慎用
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    Task<long> DelByPatternAsync(string pattern);

    /// <summary>
    /// 判断是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Exists(string key);

    /// <summary>
    /// 判断是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    string Get(string key);

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    T Get<T>(string key);

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<string> GetAsync(string key);

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<T> GetAsync<T>(string key);

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Set(string key, object value);

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expireSeconds">单位秒</param>
    /// <returns></returns>
    bool Set(string key, object value, int expireSeconds);

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expireTimeSpan"></param>
    /// <returns></returns>
    bool Set(string key, object value, TimeSpan expireTimeSpan);

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Task<bool> SetAsync(string key, object value);

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expireSeconds">单位秒</param>
    /// <returns></returns>
    Task<bool> SetAsync(string key, object value, int expireSeconds);

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expireTimeSpan"></param>
    /// <returns></returns>
    Task<bool> SetAsync(string key, object value, TimeSpan expireTimeSpan);

    /// <summary>
    /// 获取所有缓存Key
    /// 慎用
    /// </summary>
    /// <returns></returns>
    List<string> GetAllKeys();

    /// <summary>
    /// 获取所有缓存Key
    /// 慎用
    /// </summary>
    /// <returns></returns>
    Task<List<string>> GetAllKeysAsync();

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    string GetAndSet(string key, Func<string> func);

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    T GetAndSet<T>(string key, Func<T> func);

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expireSeconds">单位秒</param>
    /// <param name="func"></param>
    /// <returns></returns>
    string GetAndSet(string key, int expireSeconds, Func<string> func);

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="expireSeconds">单位秒</param>
    /// <param name="func"></param>
    /// <returns></returns>
    T GetAndSet<T>(string key, int expireSeconds, Func<T> func);

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expireTimeSpan"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    string GetAndSet(string key, TimeSpan expireTimeSpan, Func<string> func);

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="expireTimeSpan"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    T GetAndSet<T>(string key, TimeSpan expireTimeSpan, Func<T> func);

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    Task<string> GetAndSetAsync(string key, Func<Task<string>> func);

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    Task<T> GetAndSetAsync<T>(string key, Func<Task<T>> func);

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expireSeconds">单位秒</param>
    /// <param name="func"></param>
    /// <returns></returns>
    Task<string> GetAndSetAsync(string key, int expireSeconds, Func<Task<string>> func);

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="expireSeconds">单位秒</param>
    /// <param name="func"></param>
    /// <returns></returns>
    Task<T> GetAndSetAsync<T>(string key, int expireSeconds, Func<Task<T>> func);

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expireTimeSpan"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    Task<string> GetAndSetAsync(string key, TimeSpan expireTimeSpan, Func<Task<string>> func);

    /// <summary>
    /// 获取并且设置缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="expireTimeSpan"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    Task<T> GetAndSetAsync<T>(string key, TimeSpan expireTimeSpan, Func<Task<T>> func);
}