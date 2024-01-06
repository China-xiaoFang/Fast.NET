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

using Fast.Logging.Commons;
using Microsoft.Extensions.Logging;

namespace Fast.Logging.Internal;

/// <summary>
/// 构建字符串日志部分类
/// </summary>
public sealed partial class StringLoggingPart
{
    /// <summary>
    /// 设置消息
    /// </summary>
    /// <param name="message"></param>
    public StringLoggingPart SetMessage(string message)
    {
        // 支持读取配置渲染
        //if (message != null) Message = message.Render();
        if (message != null)
            Message = message;
        return this;
    }

    /// <summary>
    /// 设置日志级别
    /// </summary>
    /// <param name="level"></param>
    public StringLoggingPart SetLevel(LogLevel level)
    {
        Level = level;
        return this;
    }

    /// <summary>
    /// 设置消息格式化参数
    /// </summary>
    /// <param name="args"></param>
    public StringLoggingPart SetArgs(params object[] args)
    {
        if (args != null && args.Length > 0)
            Args = args;
        return this;
    }

    /// <summary>
    /// 设置事件 Id
    /// </summary>
    /// <param name="eventId"></param>
    public StringLoggingPart SetEventId(EventId eventId)
    {
        EventId = eventId;
        return this;
    }

    /// <summary>
    /// 设置日志分类
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    public StringLoggingPart SetCategory<TClass>()
    {
        CategoryType = typeof(TClass);
        return this;
    }

    /// <summary>
    /// 设置异常对象
    /// </summary>
    public StringLoggingPart SetException(Exception exception)
    {
        if (exception != null)
            Exception = exception;
        return this;
    }

    /// <summary>
    /// 设置日志服务作用域
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public StringLoggingPart SetLoggerScoped(IServiceProvider serviceProvider)
    {
        if (serviceProvider != null)
            LoggerScoped = serviceProvider;
        return this;
    }

    /// <summary>
    /// 配置日志上下文
    /// </summary>
    /// <param name="properties">建议使用 ConcurrentDictionary 类型</param>
    /// <returns></returns>
    public StringLoggingPart ScopeContext(IDictionary<object, object> properties)
    {
        if (properties == null)
            return this;
        LogContext = new LogContext {Properties = properties};

        return this;
    }

    /// <summary>
    /// 配置日志上下文
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public StringLoggingPart ScopeContext(Action<LogContext> configure)
    {
        var logContext = new LogContext();
        configure?.Invoke(logContext);

        LogContext = logContext;

        return this;
    }

    /// <summary>
    /// 配置日志上下文
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public StringLoggingPart ScopeContext(LogContext context)
    {
        if (context == null)
            return this;
        LogContext = context;

        return this;
    }
}