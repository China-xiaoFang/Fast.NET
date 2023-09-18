using System;
using Fast.Logging.Implantation.File;
using Fast.Logging.Internal;
using Microsoft.Extensions.Logging;

namespace Fast.Logging.Extensions;

/// <summary>
/// <see cref="ILoggerFactory"/> 拓展
/// </summary>
public static class ILoggerFactoryExtensions
{
    /// <summary>
    /// 添加文件日志记录器
    /// </summary>
    /// <param name="factory">日志工厂</param>
    /// <param name="fileName">日志文件完整路径或文件名，推荐 .log 作为拓展名</param>
    /// <param name="append">追加到已存在日志文件或覆盖它们</param>
    /// <returns><see cref="ILoggerFactory"/></returns>
    public static ILoggerFactory AddFile(this ILoggerFactory factory, string fileName, bool append = true)
    {
        // 添加文件日志记录器提供程序
        factory.AddProvider(new FileLoggerProvider(fileName ?? "application.log", append));

        return factory;
    }

    /// <summary>
    /// 添加文件日志记录器
    /// </summary>
    /// <param name="factory">日志工厂</param>
    /// <param name="fileName">日志文件完整路径或文件名，推荐 .log 作为拓展名</param>
    /// <param name="configure"></param>
    /// <returns><see cref="ILoggerFactory"/></returns>
    public static ILoggerFactory AddFile(this ILoggerFactory factory, string fileName, Action<FileLoggerOptions> configure)
    {
        var options = new FileLoggerOptions();
        configure?.Invoke(options);

        // 添加文件日志记录器提供程序
        factory.AddProvider(new FileLoggerProvider(fileName ?? "application.log", options));

        return factory;
    }

    /// <summary>
    /// 添加文件日志记录器
    /// </summary>
    /// <param name="factory">日志工厂</param>
    /// <param name="configure">文件日志记录器配置选项委托</param>
    /// <returns><see cref="ILoggerFactory"/></returns>
    public static ILoggerFactory AddFile(this ILoggerFactory factory, Action<FileLoggerOptions> configure = default)
    {
        return factory.AddFile(() => "Logging:File", configure);
    }

    /// <summary>
    /// 添加文件日志记录器
    /// </summary>
    /// <param name="factory">日志工厂</param>
    /// <param name="configuraionKey">获取配置文件对应的 Key</param>
    /// <param name="configure">文件日志记录器配置选项委托</param>
    /// <returns><see cref="ILoggerFactory"/></returns>
    public static ILoggerFactory AddFile(this ILoggerFactory factory, Func<string> configuraionKey,
        Action<FileLoggerOptions> configure = default)
    {
        // 添加文件日志记录器提供程序
        factory.AddProvider(Penetrates.CreateFromConfiguration(configuraionKey, configure));

        return factory;
    }
}