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
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace Fast.Logging.Console;

/// <summary>
/// 控制台默认格式化程序拓展
/// </summary>
internal sealed class ConsoleFormatterExtend : ConsoleFormatter, IDisposable
{
    /// <summary>
    /// 日志格式化选项刷新 Token
    /// </summary>
    private readonly IDisposable _formatOptionsReloadToken;

    /// <summary>
    /// 日志格式化配置选项
    /// </summary>
    private ConsoleFormatterExtendOptions _formatterOptions;

    /// <summary>
    /// 是否启用控制台颜色
    /// </summary>
    private bool _disableColors;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="formatterOptions"></param>
    public ConsoleFormatterExtend(IOptionsMonitor<ConsoleFormatterExtendOptions> formatterOptions) : base(Penetrates
        .ConsoleFormatterName)
    {
        (_formatOptionsReloadToken, _formatterOptions) =
            (formatterOptions.OnChange(ReloadFormatterOptions), formatterOptions.CurrentValue);
        _disableColors = _formatterOptions.ColorBehavior == LoggerColorBehavior.Disabled ||
                         (_formatterOptions.ColorBehavior == LoggerColorBehavior.Default && System.Console.IsOutputRedirected);
    }

    /// <summary>
    /// 写入日志
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="logEntry"></param>
    /// <param name="scopeProvider"></param>
    /// <param name="textWriter"></param>
    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
    {
        // 获取格式化后的消息
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);

        // 创建日志消息
        var logDateTime = _formatterOptions.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now;
        var logMsg = new LogMessage(logEntry.Category, logEntry.LogLevel, logEntry.EventId, message, logEntry.Exception, null,
            logEntry.State, logDateTime, Environment.CurrentManagedThreadId, _formatterOptions.UseUtcTimestamp,
            IaaSContext.GetTraceId(Penetrates.RootServices, Penetrates.HttpContext));

        string standardMessage;

        // 是否自定义了自定义日志格式化程序，如果是则使用
        if (_formatterOptions.MessageFormat != null)
        {
            // 设置日志上下文
            logMsg = Penetrates.SetLogContext(scopeProvider, logMsg, _formatterOptions.IncludeScopes);

            // 设置日志消息模板
            standardMessage = _formatterOptions.MessageFormat(logMsg);
        }
        else
        {
            // 获取标准化日志消息
            standardMessage = Penetrates.OutputStandardMessage(logMsg, _formatterOptions.DateFormat, true, _disableColors,
                _formatterOptions.WithTraceId, _formatterOptions.WithStackFrame);
        }

        // 空检查
        if (message is null)
            return;

        // 判断是否自定义了日志格式化程序
        if (_formatterOptions.WriteHandler != null)
        {
            _formatterOptions.WriteHandler?.Invoke(logMsg, scopeProvider, textWriter, standardMessage, _formatterOptions);
        }
        else
        {
            // 写入控制台
            textWriter.WriteLine(standardMessage);
        }
    }

    /// <summary>
    /// 释放非托管资源
    /// </summary>
    public void Dispose()
    {
        _formatOptionsReloadToken?.Dispose();
    }

    /// <summary>
    /// 刷新日志格式化选项
    /// </summary>
    /// <param name="options"></param>
    private void ReloadFormatterOptions(ConsoleFormatterExtendOptions options)
    {
        _formatterOptions = options;
        _disableColors = options.ColorBehavior == LoggerColorBehavior.Disabled ||
                         (options.ColorBehavior == LoggerColorBehavior.Default && System.Console.IsOutputRedirected);
    }
}