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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fast.Logging;

/// <summary>
/// 全局日志静态类。
/// </summary>
[SuppressSniffer]
public static class Log
{
    /// <summary>
    /// 手动构建方式。
    /// </summary>
    /// <returns>手动构建方式。</returns>
    public static StringLoggingPart Default()
    {
        return StringLoggingPart.Default();
    }

    /// <summary>
    /// 创建日志记录器。
    /// </summary>
    /// <typeparam name="T">日志记录器使用的分类类型。</typeparam>
    /// <returns>创建的日志记录器。</returns>
    public static ILogger CreateLogger<T>()
    {
        return MAppContext.GetServiceProvider(typeof(ILogger<T>), Penetrates.RootServices, Penetrates.InternalServices,
                Penetrates.HttpContext)
            .GetRequiredService<ILogger<T>>();
    }

    /// <summary>
    /// 创建日志工厂。
    /// </summary>
    /// <remarks><see cref="ILoggerFactory"/> 实现了 <see cref="IDisposable"/> 接口，注意使用 `using` 控制。</remarks>
    /// <param name="configure">日志构建配置操作。</param>
    /// <returns>创建的日志工厂。</returns>
    public static ILoggerFactory CreateLoggerFactory(Action<ILoggingBuilder> configure = null)
    {
        return LoggerFactory.Create(builder =>
        {
            // 添加默认控制台输出
            builder.AddConsoleFormatter();

            configure?.Invoke(builder);
        });
    }

    /// <summary>
    /// 创建包含指定属性的日志作用域。
    /// </summary>
    /// <param name="properties">要加入日志作用域的属性；并发修改时应使用 <see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey,TValue}"/>。</param>
    /// <returns>日志记录器以及用于结束作用域的释放句柄。</returns>
    public static (ILogger logger, IDisposable scope) ScopeContext(IDictionary<object, object> properties)
    {
        return GetLogger(StringLoggingPart.Default()
            .ScopeContext(properties));
    }

    /// <summary>
    /// 创建包含指定属性的日志作用域。
    /// </summary>
    /// <param name="configure">日志上下文配置操作。</param>
    /// <returns>日志记录器以及用于结束作用域的释放句柄。</returns>
    public static (ILogger logger, IDisposable scope) ScopeContext(Action<LogContext> configure)
    {
        return GetLogger(StringLoggingPart.Default()
            .ScopeContext(configure));
    }

    /// <summary>
    /// 创建包含指定属性的日志作用域。
    /// </summary>
    /// <param name="context">当前日志上下文。</param>
    /// <returns>日志记录器以及用于结束作用域的释放句柄。</returns>
    public static (ILogger logger, IDisposable scope) ScopeContext(LogContext context)
    {
        return GetLogger(StringLoggingPart.Default()
            .ScopeContext(context));
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Information"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Information(string message, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .LogInformation();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Information"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Information(string message, EventId eventId, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .LogInformation();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Information"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Information(string message, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .SetException(exception)
            .LogInformation();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Information"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Information(string message, EventId eventId, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .SetException(exception)
            .LogInformation();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Information"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Information<TClass>(string message, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .LogInformation();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Information"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Information<TClass>(string message, EventId eventId, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .LogInformation();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Information"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Information<TClass>(string message, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .SetException(exception)
            .LogInformation();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Information"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Information<TClass>(string message, EventId eventId, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .SetException(exception)
            .LogInformation();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Warning"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Warning(string message, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .LogWarning();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Warning"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Warning(string message, EventId eventId, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .LogWarning();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Warning"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Warning(string message, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .SetException(exception)
            .LogWarning();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Warning"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Warning(string message, EventId eventId, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .SetException(exception)
            .LogWarning();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Warning"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Warning<TClass>(string message, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .LogWarning();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Warning"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Warning<TClass>(string message, EventId eventId, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .LogWarning();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Warning"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Warning<TClass>(string message, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .SetException(exception)
            .LogWarning();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Warning"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Warning<TClass>(string message, EventId eventId, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .SetException(exception)
            .LogWarning();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Error"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Error(string message, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .LogError();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Error"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Error(string message, EventId eventId, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .LogError();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Error"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Error(string message, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .SetException(exception)
            .LogError();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Error"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Error(string message, EventId eventId, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .SetException(exception)
            .LogError();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Error"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Error<TClass>(string message, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .LogError();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Error"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Error<TClass>(string message, EventId eventId, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .LogError();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Error"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Error<TClass>(string message, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .SetException(exception)
            .LogError();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Error"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Error<TClass>(string message, EventId eventId, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .SetException(exception)
            .LogError();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Debug"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Debug(string message, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .LogDebug();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Debug"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Debug(string message, EventId eventId, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .LogDebug();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Debug"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Debug(string message, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .SetException(exception)
            .LogDebug();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Debug"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Debug(string message, EventId eventId, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .SetException(exception)
            .LogDebug();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Debug"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Debug<TClass>(string message, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .LogDebug();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Debug"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Debug<TClass>(string message, EventId eventId, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .LogDebug();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Debug"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Debug<TClass>(string message, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .SetException(exception)
            .LogDebug();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Debug"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Debug<TClass>(string message, EventId eventId, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .SetException(exception)
            .LogDebug();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Trace"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Trace(string message, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .LogTrace();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Trace"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Trace(string message, EventId eventId, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .LogTrace();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Trace"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Trace(string message, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .SetException(exception)
            .LogTrace();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Trace"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Trace(string message, EventId eventId, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .SetException(exception)
            .LogTrace();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Trace"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Trace<TClass>(string message, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .LogTrace();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Trace"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Trace<TClass>(string message, EventId eventId, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .LogTrace();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Trace"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Trace<TClass>(string message, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .SetException(exception)
            .LogTrace();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Trace"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Trace<TClass>(string message, EventId eventId, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .SetException(exception)
            .LogTrace();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Critical"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Critical(string message, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .LogCritical();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Critical"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Critical(string message, EventId eventId, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .LogCritical();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Critical"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Critical(string message, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .SetException(exception)
            .LogCritical();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Critical"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    public static void Critical(string message, EventId eventId, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .SetException(exception)
            .LogCritical();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Critical"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Critical<TClass>(string message, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .LogCritical();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Critical"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Critical<TClass>(string message, EventId eventId, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .LogCritical();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Critical"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Critical<TClass>(string message, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .SetException(exception)
            .LogCritical();
    }

    /// <summary>
    /// 写入 <see cref="LogLevel.Critical"/> 级别日志。
    /// </summary>
    /// <param name="message">要记录或返回的消息。</param>
    /// <param name="eventId">日志事件标识。</param>
    /// <param name="exception">要处理的异常。</param>
    /// <param name="args">格式化消息时使用的参数。</param>
    /// <typeparam name="TClass">要处理的对象类型。</typeparam>
    public static void Critical<TClass>(string message, EventId eventId, Exception exception, params object[] args)
    {
        StringLoggingPart.Default()
            .SetCategory<TClass>()
            .SetMessage(message)
            .SetArgs(args)
            .SetEventId(eventId)
            .SetException(exception)
            .LogCritical();
    }

    /// <summary>
    /// 获取日志实例。
    /// </summary>
    /// <param name="loggingPart">用于创建日志记录器的日志消息构建器。</param>
    /// <returns>获取到的日志实例。</returns>
    private static (ILogger, IDisposable) GetLogger(StringLoggingPart loggingPart)
    {
        var (logger, loggerFactory, hasException) = loggingPart.GetLogger();

        if (logger == null)
            throw new InvalidOperationException("Unable to create a logger instance.");

        var scope = logger.BeginScope(loggingPart.LogContext);
        if (hasException)
        {
            scope!.Dispose();
            loggerFactory?.Dispose();

            throw new InvalidOperationException("Unable to set log context data.");
        }

        return (logger, scope);
    }
}
