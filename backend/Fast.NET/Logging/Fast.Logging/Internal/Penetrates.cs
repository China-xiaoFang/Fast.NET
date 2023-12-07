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

using System.Diagnostics;
using System.Text;
using Fast.IaaS;
using Fast.Logging.Extensions;
using Fast.Logging.Implantation;
using Fast.Logging.Implantation.Console;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fast.Logging.Internal;

/// <summary>
/// 常量、公共方法配置类
/// </summary>
internal static class Penetrates
{
    /// <summary>
    /// 默认日志级别
    /// </summary>
    internal static LogLevel DefaultLogLevel { get; set; }

    /// <summary>
    /// 应用服务
    /// </summary>
    internal static IServiceCollection InternalServices;

    /// <summary>
    /// 根服务
    /// </summary>
    internal static IServiceProvider RootServices;

    /// <summary>
    /// 请求上下文
    /// </summary>
    internal static HttpContext HttpContext =>
        FastContext.CatchOrDefault(() => RootServices?.GetService<IHttpContextAccessor>()?.HttpContext);

    /// <summary>
    /// 获取当前请求 TraceId
    /// </summary>
    /// <returns></returns>
    internal static string GetTraceId()
    {
        return Activity.Current?.Id ?? (RootServices == null
            ? default
            : FastContext.CatchOrDefault(() => RootServices?.GetService<IHttpContextAccessor>()?.HttpContext)?.TraceIdentifier);
    }

    /// <summary>
    /// 获取请求生存周期的服务
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    internal static object GetRequiredService(Type serviceType)
    {
        // 第一选择，判断是否是单例注册且单例服务不为空，如果是直接返回根服务提供器
        if (RootServices != null && InternalServices
                .Where(u => u.ServiceType == (serviceType.IsGenericType ? serviceType.GetGenericTypeDefinition() : serviceType))
                .Any(u => u.Lifetime == ServiceLifetime.Singleton))
        {
            return RootServices.GetRequiredService(serviceType);
        }

        // 第二选择是获取 HttpContext 对象的 RequestServices
        if (HttpContext != null)
        {
            return HttpContext.RequestServices.GetRequiredService(serviceType);
        }

        // 第三选择，创建新的作用域并返回服务提供器
        if (RootServices != null)
        {
            var scoped = RootServices.CreateScope();

            var result = scoped.ServiceProvider.GetRequiredService(serviceType);

            scoped.Dispose();

            return result;
        }

        {
            // 第四选择，构建新的服务对象（性能最差）
            var serviceProvider = InternalServices.BuildServiceProvider();

            var result = serviceProvider.GetRequiredService(serviceType);

            serviceProvider.Dispose();

            return result;
        }
    }

    /// <summary>
    /// 获取请求生存周期的服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    internal static TService GetRequiredService<TService>() where TService : class
    {
        var serviceType = typeof(TService);

        // 第一选择，判断是否是单例注册且单例服务不为空，如果是直接返回根服务提供器
        if (RootServices != null && InternalServices
                .Where(u => u.ServiceType == (serviceType.IsGenericType ? serviceType.GetGenericTypeDefinition() : serviceType))
                .Any(u => u.Lifetime == ServiceLifetime.Singleton))
        {
            return RootServices.GetRequiredService<TService>();
        }
        // 第二选择是获取 HttpContext 对象的 RequestServices

        if (HttpContext != null)
        {
            return HttpContext.RequestServices.GetRequiredService<TService>();
        }
        // 第三选择，创建新的作用域并返回服务提供器

        if (RootServices != null)
        {
            var scoped = RootServices.CreateScope();

            var result = scoped.ServiceProvider.GetRequiredService<TService>();

            scoped.Dispose();

            return result;
        }

        {
            // 第四选择，构建新的服务对象（性能最差）
            var serviceProvider = InternalServices.BuildServiceProvider();

            var result = serviceProvider.GetRequiredService<TService>();

            serviceProvider.Dispose();

            return result;
        }
    }

    /// <summary>
    /// 控制台默认格式化程序名称
    /// </summary>
    internal const string ConsoleFormatterName = "console-format";

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
    internal static string OutputStandardMessage(LogMessage logMsg, string dateFormat = "yyyy-MM-dd HH:mm:ss.fffffff zzz dddd",
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

        _ = AppendWithColor(formatString, GetLogLevelString(logMsg.LogLevel), logLevelColors);
        formatString.Append(": ");
        formatString.Append(logMsg.LogDateTime.ToString(dateFormat));
        formatString.Append(' ');
        formatString.Append(logMsg.UseUtcTimestamp ? "U" : "L");
        formatString.Append(' ');
        _ = AppendWithColor(formatString, logMsg.LogName,
            disableConsoleColor ? new ConsoleColors(null, null) : new ConsoleColors(ConsoleColor.Cyan, ConsoleColor.DarkCyan));
        formatString.Append('[');
        formatString.Append(logMsg.EventId.Id);
        formatString.Append(']');
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
            if (stackFrames.Count() > pos)
            {
                var targetMethod = stackFrames.Where((u, i) => i == pos).First().GetMethod();
                var declaringType = targetMethod?.DeclaringType;
                var targetAssembly = declaringType?.Assembly;

                formatString.Append(PadLeftAlign($"[{targetAssembly?.GetName().Name}.dll] {targetMethod}"));
                formatString.AppendLine();
            }
        }

        // 对日志内容进行缩进对齐处理
        formatString.Append(PadLeftAlign(logMsg.Message));

        // 如果包含异常信息，则创建新一行写入
        if (logMsg.Exception != null)
        {
            var EXCEPTION_SEPARATOR_WITH_COLOR = AppendWithColor(default, EXCEPTION_SEPARATOR, logLevelColors).ToString();
            var exceptionMessage =
                $"{Environment.NewLine}{EXCEPTION_SEPARATOR_WITH_COLOR}{Environment.NewLine}{logMsg.Exception}{Environment.NewLine}{EXCEPTION_SEPARATOR_WITH_COLOR}";

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
    internal static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel)),
        };
    }

    /// <summary>
    /// 设置日志上下文
    /// </summary>
    /// <param name="scopeProvider"></param>
    /// <param name="logMsg"></param>
    /// <param name="includeScopes"></param>
    /// <returns></returns>
    internal static LogMessage SetLogContext(IExternalScopeProvider scopeProvider, LogMessage logMsg, bool includeScopes)
    {
        // 设置日志上下文
        if (includeScopes && scopeProvider != null)
        {
            // 解析日志上下文数据
            scopeProvider.ForEachScope<object>((scope, ctx) =>
            {
                if (scope != null && scope is LogContext context)
                {
                    if (logMsg.Context == null)
                        logMsg.Context = context;
                    else
                        logMsg.Context = logMsg.Context.SetRange(context.Properties);
                }
            }, null);
        }

        return logMsg;
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
        formatString ??= new();

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
            _ => "\u001b[39m\u001b[22m",
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
            _ => "\u001b[49m",
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
            LogLevel.Debug => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black),
            LogLevel.Trace => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black),
            _ => new ConsoleColors(null, background: null),
        };
    }
}