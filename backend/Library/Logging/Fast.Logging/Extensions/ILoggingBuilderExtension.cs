using System;
using Fast.Logging.Implantation.Console;
using Fast.Logging.Implantation.File;
using Fast.Logging.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fast.Logging.Extensions;

/// <summary>
/// 日志构建器拓展类
/// </summary>
public static class ILoggingBuilderExtension
{
    /// <summary>
    /// 添加控制台默认格式化器
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddConsoleFormatter(this ILoggingBuilder builder,
        Action<ConsoleFormatterExtendOptions> configure = default)
    {
        configure ??= options => { };

        return builder.AddConsole(options => options.FormatterName = "console-format")
            .AddConsoleFormatter<ConsoleFormatterExtend, ConsoleFormatterExtendOptions>(configure);
    }

    /// <summary>
    /// 添加文件日志记录器
    /// </summary>
    /// <param name="builder">日志构建器</param>
    /// <param name="fileName">日志文件完整路径或文件名，推荐 .log 作为拓展名</param>
    /// <param name="append">追加到已存在日志文件或覆盖它们</param>
    /// <returns><see cref="ILoggingBuilder"/></returns>
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string fileName, bool append = true)
    {
        // 注册文件日志记录器提供器
        builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>(serviceProvider =>
        {
            return new FileLoggerProvider(fileName ?? "application.log", append);
        }));

        return builder;
    }

    /// <summary>
    /// 添加文件日志记录器
    /// </summary>
    /// <param name="builder">日志构建器</param>
    /// <param name="fileName">日志文件完整路径或文件名，推荐 .log 作为拓展名</param>
    /// <param name="configure">文件日志记录器配置选项委托</param>
    /// <returns><see cref="ILoggingBuilder"/></returns>
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string fileName, Action<FileLoggerOptions> configure)
    {
        // 注册文件日志记录器提供器
        builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>(serviceProvider =>
        {
            var options = new FileLoggerOptions();
            configure?.Invoke(options);

            return new FileLoggerProvider(fileName ?? "application.log", options);
        }));

        return builder;
    }

    /// <summary>
    /// 添加文件日志记录器（从配置文件中）默认 Key 为："Logging:File"
    /// </summary>
    /// <param name="builder">日志构建器</param>
    /// <param name="configure">文件日志记录器配置选项委托</param>
    /// <returns><see cref="ILoggingBuilder"/></returns>
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, Action<FileLoggerOptions> configure = default)
    {
        return builder.AddFile(() => "Logging:File", configure);
    }

    /// <summary>
    /// 添加文件日志记录器（从配置文件中）
    /// </summary>
    /// <param name="builder">日志构建器</param>
    /// <param name="configuraionKey">获取配置文件对应的 Key</param>
    /// <param name="configure">文件日志记录器配置选项委托</param>
    /// <returns><see cref="ILoggingBuilder"/></returns>
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, Func<string> configuraionKey,
        Action<FileLoggerOptions> configure = default)
    {
        // 注册文件日志记录器提供器
        builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>(serviceProvider =>
        {
            return Penetrates.CreateFromConfiguration(configuraionKey, configure);
        }));

        return builder;
    }
}