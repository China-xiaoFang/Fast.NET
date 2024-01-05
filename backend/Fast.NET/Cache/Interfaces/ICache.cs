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

// ReSharper disable once CheckNamespace

namespace Fast.Cache;

/// <summary>
/// 缓存服务接口
/// </summary>
public interface ICache
{
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