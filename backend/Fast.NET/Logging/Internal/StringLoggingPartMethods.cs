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

using Fast.Logging.Extensions;
using Microsoft.Extensions.Logging;

namespace Fast.Logging.Internal;

/// <summary>
/// 构建字符串日志部分类
/// </summary>
public sealed partial class StringLoggingPart
{
    /// <summary>
    /// Information
    /// </summary>
    public void LogInformation()
    {
        SetLevel(LogLevel.Information).Log();
    }

    /// <summary>
    /// Warning
    /// </summary>
    public void LogWarning()
    {
        SetLevel(LogLevel.Warning).Log();
    }

    /// <summary>
    /// Error
    /// </summary>
    public void LogError()
    {
        SetLevel(LogLevel.Error).Log();
    }

    /// <summary>
    /// Debug
    /// </summary>
    public void LogDebug()
    {
        SetLevel(LogLevel.Debug).Log();
    }

    /// <summary>
    /// Trace
    /// </summary>
    public void LogTrace()
    {
        SetLevel(LogLevel.Trace).Log();
    }

    /// <summary>
    /// Critical
    /// </summary>
    public void LogCritical()
    {
        SetLevel(LogLevel.Critical).Log();
    }

    /// <summary>
    /// 写入日志
    /// </summary>
    /// <returns></returns>
    public void Log()
    {
        if (Message == null)
            return;

        // 获取日志实例
        var (logger, loggerFactory, hasException) = GetLogger();
        using var scope = logger.ScopeContext(LogContext);

        // 如果没有异常且事件 Id 为空
        if (Exception == null && EventId == null)
        {
            logger.Log(Level, Message, Args);
        }
        // 如果存在异常且事件 Id 为空
        else if (Exception != null && EventId == null)
        {
            logger.Log(Level, Exception, Message, Args);
        }
        // 如果异常为空且事件 Id 不为空
        else if (Exception == null && EventId != null)
        {
            logger.Log(Level, EventId.Value, Message, Args);
        }
        // 如果存在异常且事件 Id 不为空
        else if (Exception != null && EventId != null)
        {
            logger.Log(Level, EventId.Value, Exception, Message, Args);
        }

        // 释放临时日志工厂
        if (hasException)
        {
            loggerFactory?.Dispose();
        }
    }

    /// <summary>
    /// 获取日志实例
    /// </summary>
    /// <returns></returns>
    internal (ILogger, ILoggerFactory, bool) GetLogger()
    {
        // 解析日志分类名
        var categoryType = CategoryType ?? typeof(StringLogging);

        ILoggerFactory loggerFactory = null;
        ILogger logger = null;
        var hasException = false;

        // 解决启动时打印日志问题
        if (Penetrates.RootServices == null)
        {
            hasException = true;
            loggerFactory = CreateDisposeLoggerFactory();
        }
        else
        {
            try
            {
                logger = Penetrates.GetRequiredService(typeof(ILogger<>).MakeGenericType(categoryType)) as ILogger;
            }
            catch
            {
                hasException = true;
                loggerFactory = CreateDisposeLoggerFactory();
            }
        }

        // 创建日志实例
        logger ??= loggerFactory?.CreateLogger(categoryType.FullName);

        return (logger, loggerFactory, hasException);
    }

    /// <summary>
    /// 创建待释放的日志工厂
    /// </summary>
    /// <returns></returns>
    private static ILoggerFactory CreateDisposeLoggerFactory()
    {
        return LoggerFactory.Create(builder => { builder.AddConsoleFormatter(); });
    }
}