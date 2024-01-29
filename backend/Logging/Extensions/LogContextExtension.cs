// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
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

using Fast.Logging.Commons;

namespace Fast.Logging.Extensions;

/// <summary>
/// LogContext 拓展
/// </summary>
public static class LogContextExtension
{
    /// <summary>
    /// 设置上下文数据
    /// </summary>
    /// <param name="logContext"></param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns></returns>
    public static LogContext Set(this LogContext logContext, object key, object value)
    {
        if (logContext == null || key == null)
            return logContext;

        logContext.Properties ??= new Dictionary<object, object>();

        if (logContext.Properties.ContainsKey(key))
            logContext.Properties.Remove(key);
        logContext.Properties.Add(key, value);
        return logContext;
    }

    /// <summary>
    /// 批量设置上下文数据
    /// </summary>
    /// <param name="logContext"></param>
    /// <param name="properties"></param>
    /// <returns></returns>
    public static LogContext SetRange(this LogContext logContext, IDictionary<object, object> properties)
    {
        if (logContext == null || properties == null || properties.Count == 0)
            return logContext;

        foreach (var (key, value) in properties)
        {
            logContext.Set(key, value);
        }

        return logContext;
    }

    /// <summary>
    /// 获取上下文数据
    /// </summary>
    /// <param name="logContext"></param>
    /// <param name="key">键</param>
    /// <returns></returns>
    public static object Get(this LogContext logContext, object key)
    {
        if (logContext == null || key == null || logContext.Properties == null || logContext.Properties.Count == 0)
            return default;

        var isExists = logContext.Properties.TryGetValue(key, out var value);
        return isExists ? value : null;
    }

    /// <summary>
    /// 获取上下文数据
    /// </summary>
    /// <param name="logContext"></param>
    /// <param name="key">键</param>
    /// <returns></returns>
    public static object Get<T>(this LogContext logContext, object key)
    {
        var value = logContext.Get(key);
        return (T) value;
    }
}