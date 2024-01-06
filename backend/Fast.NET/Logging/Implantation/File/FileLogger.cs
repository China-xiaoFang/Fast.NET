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

using Fast.IaaS;
using Fast.Logging.Commons;
using Fast.Logging.Internal;
using Microsoft.Extensions.Logging;

namespace Fast.Logging.Implantation.File;

/// <summary>
/// 文件日志记录器
/// </summary>
/// <remarks>https://docs.microsoft.com/zh-cn/dotnet/core/extensions/custom-logging-provider</remarks>
internal class FileLogger : ILogger
{
    /// <summary>
    /// 记录器类别名称
    /// </summary>
    private readonly string _logName;

    /// <summary>
    /// 文件日志记录器提供器
    /// </summary>
    private readonly FileLoggerProvider _fileLoggerProvider;

    /// <summary>
    /// 日志配置选项
    /// </summary>
    private readonly FileLoggerOptions _options;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logName">记录器类别名称</param>
    /// <param name="fileLoggerProvider">文件日志记录器提供器</param>
    public FileLogger(string logName, FileLoggerProvider fileLoggerProvider)
    {
        _logName = logName;
        _fileLoggerProvider = fileLoggerProvider;
        _options = fileLoggerProvider.LoggerOptions;
    }

    /// <summary>
    /// 开始逻辑操作范围
    /// </summary>
    /// <typeparam name="TState">标识符类型参数</typeparam>
    /// <param name="state">要写入的项/对象</param>
    /// <returns><see cref="IDisposable"/></returns>
    public IDisposable BeginScope<TState>(TState state)
    {
        return _fileLoggerProvider.ScopeProvider.Push(state);
    }

    /// <summary>
    /// 检查是否已启用给定日志级别
    /// </summary>
    /// <param name="logLevel">日志级别</param>
    /// <returns><see cref="bool"/></returns>
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= _options.MinimumLevel;
    }

    /// <summary>
    /// 写入日志项
    /// </summary>
    /// <typeparam name="TState">标识符类型参数</typeparam>
    /// <param name="logLevel">日志级别</param>
    /// <param name="eventId">事件 Id</param>
    /// <param name="state">要写入的项/对象</param>
    /// <param name="exception">异常对象</param>
    /// <param name="formatter">日志格式化器</param>
    /// <exception cref="ArgumentNullException"></exception>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
        Func<TState, Exception, string> formatter)
    {
        // 判断日志级别是否有效
        if (!IsEnabled(logLevel))
            return;

        // 检查日志格式化器
        if (formatter == null)
            throw new ArgumentNullException(nameof(formatter));

        // 获取格式化后的消息
        var message = formatter(state, exception);

        var logDateTime = _options.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now;
        var logMsg = new LogMessage(_logName, logLevel, eventId, message, exception, null, state, logDateTime,
            Environment.CurrentManagedThreadId, _options.UseUtcTimestamp,
            IaaSContext.GetTraceId(Penetrates.RootServices, Penetrates.HttpContext));

        // 设置日志上下文
        logMsg = Penetrates.SetLogContext(_fileLoggerProvider.ScopeProvider, logMsg, _options.IncludeScopes);

        // 判断是否自定义了日志筛选器，如果是则检查是否符合条件
        if (_options.WriteFilter?.Invoke(logMsg) == false)
            return;

        // 设置日志消息模板
        logMsg.Message = _options.MessageFormat != null
            ? _options.MessageFormat(logMsg)
            : Penetrates.OutputStandardMessage(logMsg, _options.DateFormat, withTraceId: _options.WithTraceId,
                withStackFrame: _options.WithStackFrame);

        // 空检查
        if (logMsg.Message is null)
            return;

        // 写入日志队列
        _fileLoggerProvider.WriteToQueue(logMsg);
    }
}