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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Fast.Logging;

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
                         (_formatterOptions.ColorBehavior == LoggerColorBehavior.Default && Console.IsOutputRedirected);
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
        var message = logEntry.Formatter!.Invoke(logEntry.State, logEntry.Exception);

        // 创建日志消息
        var logDateTime = _formatterOptions.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now;
        var logMsg = new LogMessage(logEntry.Category, logEntry.LogLevel, logEntry.EventId, message, logEntry.Exception, null,
            logEntry.State, logDateTime, Environment.CurrentManagedThreadId, _formatterOptions.UseUtcTimestamp,
            MAppContext.GetTraceId(Penetrates.RootServices, Penetrates.HttpContext));

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
            standardMessage = LoggingContext.OutputStandardMessage(logMsg, _formatterOptions.DateFormat, true, _disableColors,
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
                         (options.ColorBehavior == LoggerColorBehavior.Default && Console.IsOutputRedirected);
    }
}