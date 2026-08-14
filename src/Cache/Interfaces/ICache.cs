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

namespace Fast.Cache;

/// <summary>
/// 默认缓存服务接口。
/// </summary>
public interface ICache : ICache<DefaultCacheContextLocator>
{
}

/// <summary>
/// 缓存服务接口。
/// </summary>
/// <typeparam name="CacheContextLocator">缓存上下文定位器类型，用于隔离不同缓存配置。</typeparam>
public interface ICache<out CacheContextLocator> where CacheContextLocator : ICacheContextLocator, new()
{
    /// <summary>
    /// 前缀。
    /// </summary>
    public string Prefix { get; }

    /// <summary>
    /// CSRedis 缓存客户端。
    /// </summary>
    CSRedisClient Client { get; }

    /// <summary>
    /// 缓存上下文定位器。
    /// </summary>
    CacheContextLocator ContextLocator { get; }

    /// <summary>
    /// 删除指定键的缓存项。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <returns>实际删除的项数。</returns>
    long Del(params string[] key);

    /// <summary>
    /// 异步删除指定键的缓存项。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <returns>表示异步删除缓存的任务，任务结果为实际删除的项数。</returns>
    Task<long> DelAsync(params string[] key);

    /// <summary>
    /// 根据匹配模式批量删除缓存。
    /// </summary>
    /// <remarks>该操作会扫描并删除所有匹配键，键数量较多时可能造成 Redis 阻塞，请避免在高流量路径中调用。</remarks>
    /// <param name="pattern">用于匹配目标项的模式。</param>
    /// <returns>实际删除的项数。</returns>
    long DelByPattern(string pattern);

    /// <summary>
    /// 异步根据匹配模式批量删除缓存。
    /// </summary>
    /// <remarks>该操作会扫描并删除所有匹配键，键数量较多时可能造成 Redis 阻塞，请避免在高流量路径中调用。</remarks>
    /// <param name="pattern">用于匹配目标项的模式。</param>
    /// <returns>表示异步根据匹配模式批量删除缓存的任务，任务结果为实际删除的项数。</returns>
    Task<long> DelByPatternAsync(string pattern);

    /// <summary>
    /// 判断指定缓存键是否存在。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <returns>满足条件时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
    bool Exists(string key);

    /// <summary>
    /// 异步判断指定缓存键是否存在。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <returns>满足条件时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// 获取指定键的缓存值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <returns>获取到的缓存。</returns>
    string Get(string key);

    /// <summary>
    /// 获取指定键的缓存值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <typeparam name="T">缓存值的类型。</typeparam>
    /// <returns>获取到的缓存。</returns>
    T Get<T>(string key);

    /// <summary>
    /// 异步获取指定键的缓存值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <returns>表示异步获取缓存的任务，任务结果为获取到的缓存。</returns>
    Task<string> GetAsync(string key);

    /// <summary>
    /// 异步获取指定键的缓存值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <typeparam name="T">缓存值的类型。</typeparam>
    /// <returns>表示异步获取缓存的任务，任务结果为获取到的缓存。</returns>
    Task<T> GetAsync<T>(string key);

    /// <summary>
    /// 写入指定键的缓存值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="value">要写入缓存的值。</param>
    /// <returns>缓存写入成功时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
    bool Set(string key, object value);

    /// <summary>
    /// 写入指定键的缓存值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="value">要写入缓存的值。</param>
    /// <param name="expireSeconds">有效时长，单位为秒。</param>
    /// <returns>缓存写入成功时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
    bool Set(string key, object value, int expireSeconds);

    /// <summary>
    /// 写入指定键的缓存值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="value">要写入缓存的值。</param>
    /// <param name="expireTimeSpan">有效时长。</param>
    /// <returns>缓存写入成功时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
    bool Set(string key, object value, TimeSpan expireTimeSpan);

    /// <summary>
    /// 异步写入指定键的缓存值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="value">要写入缓存的值。</param>
    /// <returns>缓存写入成功时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
    Task<bool> SetAsync(string key, object value);

    /// <summary>
    /// 异步写入指定键的缓存值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="value">要写入缓存的值。</param>
    /// <param name="expireSeconds">有效时长，单位为秒。</param>
    /// <returns>缓存写入成功时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
    Task<bool> SetAsync(string key, object value, int expireSeconds);

    /// <summary>
    /// 异步写入指定键的缓存值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="value">要写入缓存的值。</param>
    /// <param name="expireTimeSpan">有效时长。</param>
    /// <returns>缓存写入成功时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
    Task<bool> SetAsync(string key, object value, TimeSpan expireTimeSpan);

    /// <summary>
    /// 获取当前 Redis 数据库中的所有缓存键。
    /// </summary>
    /// <remarks>该操作需要扫描数据库中的键，键数量较多时开销较大，不应在高频请求中调用。</remarks>
    /// <returns>获取到的当前 Redis 数据库中的所有缓存键集合。</returns>
    List<string> GetAllKeys();

    /// <summary>
    /// 异步获取当前 Redis 数据库中的所有缓存键。
    /// </summary>
    /// <remarks>该操作需要扫描数据库中的键，键数量较多时开销较大，不应在高频请求中调用。</remarks>
    /// <returns>表示异步获取当前 Redis 数据库中的所有缓存键的任务，任务结果为获取到的当前 Redis 数据库中的所有缓存键集合。</returns>
    Task<List<string>> GetAllKeysAsync();

    /// <summary>
    /// 获取缓存值；缓存未命中时生成并写入新值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="func">缓存未命中时用于生成并写入值的委托。</param>
    /// <returns>缓存中已有的值，或缓存未命中时由值工厂生成并写入的新值。</returns>
    string GetAndSet(string key, Func<string> func);

    /// <summary>
    /// 获取缓存值；缓存未命中时生成并写入新值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="func">缓存未命中时用于生成并写入值的委托。</param>
    /// <typeparam name="T">缓存值的类型。</typeparam>
    /// <returns>缓存中已有的值，或缓存未命中时由值工厂生成并写入的新值。</returns>
    T GetAndSet<T>(string key, Func<T> func);

    /// <summary>
    /// 获取缓存值；缓存未命中时生成并写入新值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="expireSeconds">有效时长，单位为秒。</param>
    /// <param name="func">缓存未命中时用于生成并写入值的委托。</param>
    /// <returns>缓存中已有的值，或缓存未命中时由值工厂生成并写入的新值。</returns>
    string GetAndSet(string key, int expireSeconds, Func<string> func);

    /// <summary>
    /// 获取缓存值；缓存未命中时生成并写入新值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="expireSeconds">有效时长，单位为秒。</param>
    /// <param name="func">缓存未命中时用于生成并写入值的委托。</param>
    /// <typeparam name="T">缓存值的类型。</typeparam>
    /// <returns>缓存中已有的值，或缓存未命中时由值工厂生成并写入的新值。</returns>
    T GetAndSet<T>(string key, int expireSeconds, Func<T> func);

    /// <summary>
    /// 获取缓存值；缓存未命中时生成并写入新值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="expireTimeSpan">有效时长。</param>
    /// <param name="func">缓存未命中时用于生成并写入值的委托。</param>
    /// <returns>缓存中已有的值，或缓存未命中时由值工厂生成并写入的新值。</returns>
    string GetAndSet(string key, TimeSpan expireTimeSpan, Func<string> func);

    /// <summary>
    /// 获取缓存值；缓存未命中时生成并写入新值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="expireTimeSpan">有效时长。</param>
    /// <param name="func">缓存未命中时用于生成并写入值的委托。</param>
    /// <typeparam name="T">缓存值的类型。</typeparam>
    /// <returns>缓存中已有的值，或缓存未命中时由值工厂生成并写入的新值。</returns>
    T GetAndSet<T>(string key, TimeSpan expireTimeSpan, Func<T> func);

    /// <summary>
    /// 异步获取缓存值；缓存未命中时生成并写入新值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="func">缓存未命中时用于生成并写入值的委托。</param>
    /// <returns>表示异步获取并且设置缓存的任务，任务结果为缓存中已有的值，或缓存未命中时由值工厂生成并写入的新值。</returns>
    Task<string> GetAndSetAsync(string key, Func<Task<string>> func);

    /// <summary>
    /// 异步获取缓存值；缓存未命中时生成并写入新值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="func">缓存未命中时用于生成并写入值的委托。</param>
    /// <typeparam name="T">缓存值的类型。</typeparam>
    /// <returns>表示异步获取并且设置缓存的任务，任务结果为缓存中已有的值，或缓存未命中时由值工厂生成并写入的新值。</returns>
    Task<T> GetAndSetAsync<T>(string key, Func<Task<T>> func);

    /// <summary>
    /// 异步获取缓存值；缓存未命中时生成并写入新值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="expireSeconds">有效时长，单位为秒。</param>
    /// <param name="func">缓存不存在时用于异步生成值的委托。</param>
    /// <returns>表示异步获取并且设置缓存的任务，任务结果为缓存中已有的值，或缓存未命中时由值工厂生成并写入的新值。</returns>
    Task<string> GetAndSetAsync(string key, int expireSeconds, Func<Task<string>> func);

    /// <summary>
    /// 异步获取缓存值；缓存未命中时生成并写入新值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="expireSeconds">有效时长，单位为秒。</param>
    /// <param name="func">缓存不存在时用于异步生成值的委托。</param>
    /// <typeparam name="T">缓存值的类型。</typeparam>
    /// <returns>表示异步获取并且设置缓存的任务，任务结果为缓存中已有的值，或缓存未命中时由值工厂生成并写入的新值。</returns>
    Task<T> GetAndSetAsync<T>(string key, int expireSeconds, Func<Task<T>> func);

    /// <summary>
    /// 异步获取缓存值；缓存未命中时生成并写入新值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="expireTimeSpan">有效时长。</param>
    /// <param name="func">缓存未命中时用于生成并写入值的委托。</param>
    /// <returns>表示异步获取并且设置缓存的任务，任务结果为缓存中已有的值，或缓存未命中时由值工厂生成并写入的新值。</returns>
    Task<string> GetAndSetAsync(string key, TimeSpan expireTimeSpan, Func<Task<string>> func);

    /// <summary>
    /// 异步获取缓存值；缓存未命中时生成并写入新值。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="expireTimeSpan">有效时长。</param>
    /// <param name="func">缓存未命中时用于生成并写入值的委托。</param>
    /// <typeparam name="T">缓存值的类型。</typeparam>
    /// <returns>表示异步获取并且设置缓存的任务，任务结果为缓存中已有的值，或缓存未命中时由值工厂生成并写入的新值。</returns>
    Task<T> GetAndSetAsync<T>(string key, TimeSpan expireTimeSpan, Func<Task<T>> func);
}
