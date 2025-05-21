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

using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Fast.Logging;

/// <summary>
/// <see cref="LoggingContext"/> 日志上下文
/// </summary>
[SuppressSniffer]
public static class LoggingContext
{
    /// <summary>
    /// 异常分隔符
    /// </summary>
    private const string EXCEPTION_SEPARATOR = "++++++++++++++++++++++++++++++++++++++++++++++++++++++++";

    /// <summary>
    /// 输出标准日志消息
    /// </summary>
    /// <param name="logMsg"></param>
    /// <param name="dateFormat"></param>
    /// <param name="disableColors"></param>
    /// <param name="isConsole"></param>
    /// <param name="withTraceId"></param>
    /// <param name="withStackFrame"></param>
    /// <returns></returns>
    public static string OutputStandardMessage(LogMessage logMsg, string dateFormat = "yyyy-MM-dd HH:mm:ss.fffffff zzz dddd",
        bool isConsole = false, bool disableColors = true, bool withTraceId = false, bool withStackFrame = false)
    {
        // 空检查
        if (logMsg.Message is null)
            return null;

        // 创建默认日志格式化模板
        var formatString = new StringBuilder();

        // 获取日志级别对应控制台的颜色
        var disableConsoleColor = !isConsole || disableColors;
        var logLevelColors = GetLogLevelConsoleColors(logMsg.LogLevel, disableConsoleColor);
        var logLevelMessageColors = GetLogLevelMessageConsoleColors(logMsg.LogLevel, disableConsoleColor);

        _ = AppendWithColor(formatString, GetLogLevelString(logMsg.LogLevel), logLevelColors);
        formatString.Append(": ");
        formatString.Append(logMsg.LogDateTime.ToString(dateFormat));
        formatString.Append(' ');
        formatString.Append(logMsg.UseUtcTimestamp ? "U" : "L");
        formatString.Append(' ');
        _ = AppendWithColor(formatString, logMsg.LogName,
            disableConsoleColor ? new ConsoleColors(null, null) : new ConsoleColors(ConsoleColor.Cyan, ConsoleColor.DarkCyan));

        if (logMsg.EventId != null)
        {
            formatString.Append('[');
            formatString.Append(logMsg.EventId?.Id);
            formatString.Append(']');
        }

        formatString.Append(' ');
        formatString.Append($"#{logMsg.ThreadId}");
        if (withTraceId && !string.IsNullOrWhiteSpace(logMsg.TraceId))
        {
            formatString.Append(' ');
            _ = AppendWithColor(formatString, $"'{logMsg.TraceId}'",
                disableConsoleColor ? new ConsoleColors(null, null) : new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black));
        }

        formatString.AppendLine();

        // 输出日志输出所在方法，类型，程序集
        if (withStackFrame)
        {
            var stackTrace = new StackTrace();
            var stackFrames = stackTrace.GetFrames();
            var pos = isConsole ? 6 : 5;
            if (stackFrames.Length > pos)
            {
                var targetMethod = stackFrames.Where((_, i) => i == pos).First().GetMethod();
                var declaringType = targetMethod?.DeclaringType;
                var targetAssembly = declaringType?.Assembly;

                formatString.Append(PadLeftAlign($"[{targetAssembly?.GetName().Name}.dll] {targetMethod}"));
                formatString.AppendLine();
            }
        }

        // 对日志内容进行缩进对齐处理
        //formatString.Append(PadLeftAlign(logMsg.Message));
        _ = AppendWithColor(formatString, PadLeftAlign(logMsg.Message),
            disableConsoleColor ? new ConsoleColors(null, null) : logLevelMessageColors);

        // 如果包含异常信息，则创建新一行写入
        if (logMsg.Exception != null)
        {
            var EXCEPTION_SEPARATOR_WITH_COLOR = AppendWithColor(null, EXCEPTION_SEPARATOR, logLevelMessageColors).ToString();
            //var exceptionMessage =
            //    $"{Environment.NewLine}{EXCEPTION_SEPARATOR_WITH_COLOR}{Environment.NewLine}{logMsg.Exception}{Environment.NewLine}{EXCEPTION_SEPARATOR_WITH_COLOR}";
            var exceptionMessage =
                $"{Environment.NewLine}{EXCEPTION_SEPARATOR_WITH_COLOR}{Environment.NewLine}{AppendWithColor(null, logMsg.Exception.ToString(), logLevelMessageColors)}{Environment.NewLine}{EXCEPTION_SEPARATOR_WITH_COLOR}";

            formatString.Append(PadLeftAlign(exceptionMessage));
        }

        // 返回日志消息模板
        return formatString.ToString();
    }

    /// <summary>
    /// 将日志内容进行对齐
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private static string PadLeftAlign(string message)
    {
        var newMessage = string.Join(Environment.NewLine,
            message.Split(new[] {Environment.NewLine, "\n"}, StringSplitOptions.None)
                .Select(line => string.Empty.PadLeft(6, ' ') + line));

        return newMessage;
    }

    /// <summary>
    /// 获取日志级别短名称
    /// </summary>
    /// <param name="logLevel">日志级别</param>
    /// <returns></returns>
    public static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }

    /// <summary>
    /// 拓展 StringBuilder 增加带颜色写入
    /// </summary>
    /// <param name="message"></param>
    /// <param name="colors"></param>
    /// <param name="formatString"></param>
    /// <returns></returns>
    private static StringBuilder AppendWithColor(StringBuilder formatString, string message, ConsoleColors colors)
    {
        formatString ??= new StringBuilder();

        // 输出控制台前景色和背景色
        if (colors.Background.HasValue)
            formatString.Append(GetBackgroundColorEscapeCode(colors.Background.Value));
        if (colors.Foreground.HasValue)
            formatString.Append(GetForegroundColorEscapeCode(colors.Foreground.Value));

        formatString.Append(message);

        // 输出控制台前景色和背景色
        if (colors.Background.HasValue)
            formatString.Append("\u001b[39m\u001b[22m");
        if (colors.Foreground.HasValue)
            formatString.Append("\u001b[49m");

        return formatString;
    }

    /// <summary>
    /// 输出控制台字体颜色 UniCode 码
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    private static string GetForegroundColorEscapeCode(ConsoleColor color)
    {
        return color switch
        {
            ConsoleColor.Black => "\u001b[30m",
            ConsoleColor.DarkRed => "\u001b[31m",
            ConsoleColor.DarkGreen => "\u001b[32m",
            ConsoleColor.DarkYellow => "\u001b[33m",
            ConsoleColor.DarkBlue => "\u001b[34m",
            ConsoleColor.DarkMagenta => "\u001b[35m",
            ConsoleColor.DarkCyan => "\u001b[36m",
            ConsoleColor.Gray => "\u001b[37m",
            ConsoleColor.Red => "\u001b[1m\u001b[31m",
            ConsoleColor.Green => "\u001b[1m\u001b[32m",
            ConsoleColor.Yellow => "\u001b[1m\u001b[33m",
            ConsoleColor.Blue => "\u001b[1m\u001b[34m",
            ConsoleColor.Magenta => "\u001b[1m\u001b[35m",
            ConsoleColor.Cyan => "\u001b[1m\u001b[36m",
            ConsoleColor.White => "\u001b[1m\u001b[37m",
            ConsoleColor.DarkGray => "\u001b[90m",
            _ => "\u001b[39m\u001b[22m"
        };
    }

    /// <summary>
    /// 输出控制台背景颜色 UniCode 码
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    private static string GetBackgroundColorEscapeCode(ConsoleColor color)
    {
        return color switch
        {
            ConsoleColor.Black => "\u001b[40m",
            ConsoleColor.Red => "\u001b[41m",
            ConsoleColor.Green => "\u001b[42m",
            ConsoleColor.Yellow => "\u001b[43m",
            ConsoleColor.Blue => "\u001b[44m",
            ConsoleColor.Magenta => "\u001b[45m",
            ConsoleColor.Cyan => "\u001b[46m",
            ConsoleColor.White => "\u001b[47m",
            _ => "\u001b[49m"
        };
    }

    /// <summary>
    /// 获取控制台日志级别对应的颜色
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="disableColors"></param>
    /// <returns></returns>
    private static ConsoleColors GetLogLevelConsoleColors(LogLevel logLevel, bool disableColors = false)
    {
        if (disableColors)
        {
            return new ConsoleColors(null, null);
        }

        return logLevel switch
        {
            LogLevel.Critical => new ConsoleColors(ConsoleColor.White, ConsoleColor.Red),
            LogLevel.Error => new ConsoleColors(ConsoleColor.Black, ConsoleColor.Red),
            LogLevel.Warning => new ConsoleColors(ConsoleColor.Yellow, ConsoleColor.Black),
            LogLevel.Information => new ConsoleColors(ConsoleColor.DarkGreen, ConsoleColor.Black),
            LogLevel.Debug => new ConsoleColors(ConsoleColor.DarkGray, ConsoleColor.Black),
            LogLevel.Trace => new ConsoleColors(ConsoleColor.DarkGray, ConsoleColor.Black),
            _ => new ConsoleColors(null, background: null)
        };
    }

    /// <summary>
    /// 获取控制台日志级别消息对应的颜色
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="disableColors"></param>
    /// <returns></returns>
    private static ConsoleColors GetLogLevelMessageConsoleColors(LogLevel logLevel, bool disableColors = false)
    {
        if (disableColors)
        {
            return new ConsoleColors(null, null);
        }

        return logLevel switch
        {
            LogLevel.Critical => new ConsoleColors(ConsoleColor.White, ConsoleColor.Red),
            LogLevel.Error => new ConsoleColors(ConsoleColor.Black, ConsoleColor.Red),
            LogLevel.Warning => new ConsoleColors(ConsoleColor.Yellow, ConsoleColor.Black),
            LogLevel.Information => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black),
            LogLevel.Debug => new ConsoleColors(ConsoleColor.DarkGray, ConsoleColor.Black),
            LogLevel.Trace => new ConsoleColors(ConsoleColor.DarkGray, ConsoleColor.Black),
            _ => new ConsoleColors(null, background: null)
        };
    }
}