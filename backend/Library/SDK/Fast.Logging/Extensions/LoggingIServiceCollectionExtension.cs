using System;
using System.Collections.Generic;
using Fast.Core;
using Fast.Logging.Implantation.Console;
using Fast.Logging.Implantation.File;
using Furion.Templates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fast.Logging.Extensions;

/// <summary>
/// 日志服务拓展类
/// </summary>
public static class LoggingIServiceCollectionExtension
{
    /// <summary>
    /// 添加控制台默认格式化器
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configure">添加更多配置</param>
    /// <returns></returns>
    public static IServiceCollection AddConsoleFormatter(this IServiceCollection services,
        Action<ConsoleFormatterExtendOptions> configure = default)
    {
        return services.AddLogging(builder => builder.AddConsoleFormatter(configure));
    }

    /// <summary>
    /// 添加文件日志服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="fileName">日志文件完整路径或文件名，推荐 .log 作为拓展名</param>
    /// <param name="append">追加到已存在日志文件或覆盖它们</param>
    /// <returns></returns>
    public static IServiceCollection AddFileLogging(this IServiceCollection services, string fileName, bool append = true)
    {
        return services.AddLogging(builder => builder.AddFile(fileName, append));
    }

    /// <summary>
    /// 添加文件日志服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="fileName">日志文件完整路径或文件名，推荐 .log 作为拓展名</param>
    /// <param name="configure">文件日志记录器配置选项委托</param>
    /// <returns></returns>
    public static IServiceCollection AddFileLogging(this IServiceCollection services, string fileName,
        Action<FileLoggerOptions> configure)
    {
        return services.AddLogging(builder => builder.AddFile(fileName, configure));
    }

    /// <summary>
    /// 添加文件日志服务（从配置文件中读取配置）
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configure">文件日志记录器配置选项委托</param>
    /// <returns></returns>
    public static IServiceCollection AddFileLogging(this IServiceCollection services,
        Action<FileLoggerOptions> configure = default)
    {
        return services.AddLogging(builder => builder.AddFile(configure));
    }

    /// <summary>
    /// 添加文件日志服务（从配置文件中读取配置）
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuraionKey">获取配置文件对应的 Key</param>
    /// <param name="configure">文件日志记录器配置选项委托</param>
    /// <returns></returns>
    public static IServiceCollection AddFileLogging(this IServiceCollection services, Func<string> configuraionKey,
        Action<FileLoggerOptions> configure = default)
    {
        return services.AddLogging(builder => builder.AddFile(configuraionKey, configure));
    }

    /// <summary>
    /// 添加日志服务
    /// 197001/01/24.log
    /// </summary>
    /// <param name="services"></param>
    /// <param name="fileSizeLimitBytes">日志文件大小 控制每一个日志文件最大存储大小，默认无限制，单位是 B，也就是 1024 才等于 1KB</param>
    public static void AddLogging(this IServiceCollection services, long fileSizeLimitBytes = 10 * 1024 * 1024)
    {
        const string monthFormat = "{0:yyyy}{0:MM}";

        const string dayFormat = "{0:dd}";

        const string hourFormat = "{0:HH}";

        const string logFileFormat = $"{monthFormat}/{dayFormat}/{hourFormat}";

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddFile($"logs/error/{logFileFormat}.log",
                options => { SetLogOptions(options, LogLevel.Error, fileSizeLimitBytes); });
            // Environments other than the development environment are not logged.
            if (!App.HostEnvironment.IsDevelopment())
                return;
            loggingBuilder.AddFile($"logs/info/{logFileFormat}.log",
                options => { SetLogOptions(options, LogLevel.Information, fileSizeLimitBytes); });
            loggingBuilder.AddFile($"logs/warn/{logFileFormat}.log",
                options => { SetLogOptions(options, LogLevel.Warning, fileSizeLimitBytes); });
        });
    }

    /// <summary>
    /// 配置日志
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logLevel"></param>
    /// <param name="fileSizeLimitBytes">日志文件大小 控制每一个日志文件最大存储大小，默认无限制，单位是 B，也就是 1024 才等于 1KB</param>
    private static void SetLogOptions(FileLoggerOptions options, LogLevel logLevel, long fileSizeLimitBytes)
    {
        options.WithTraceId = true;
        options.WithStackFrame = true;
        options.FileNameRule = fileName => string.Format(fileName, DateTime.Now, logLevel.ToString());
        options.WriteFilter = logMsg => logMsg.LogLevel == logLevel;
        options.FileSizeLimitBytes = fileSizeLimitBytes;
        options.MessageFormat = logMsg =>
        {
            var msg = new List<string>
            {
                $"{logMsg.LogName}",
                $"##日志时间## {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                $"##日志等级## {logLevel}",
                $"##日志内容## {logMsg.Message}",
            };
            if (!string.IsNullOrEmpty(logMsg.Exception?.ToString()))
                msg.Add($"##异常信息## {logMsg.Exception}");

            // Generating template strings.
            var template = TP.Wrapper("WMS.Admin", "", msg.ToArray());
            return template;
        };
    }
}