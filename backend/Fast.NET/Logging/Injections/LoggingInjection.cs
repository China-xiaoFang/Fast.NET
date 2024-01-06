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
using Fast.Logging.Extensions;
using Fast.Logging.Implantation.File;
using Fast.Logging.Internal;
using Fast.Logging.Templates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fast.Logging.Injections;

/// <summary>
/// <see cref="LoggingInjection"/> 日志服务注册
/// </summary>
public class LoggingInjection : IHostingStartup
{
    /// <summary>
    /// 排序
    /// </summary>
#pragma warning disable CA1822
    public int Order => 69999;
#pragma warning restore CA1822

    /// <summary>
    /// 配置
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((hostContext, services) =>
        {
            Debugging.Info("Registering logging......");

            // 存储服务提供器
            Penetrates.InternalServices = services;

            // 获取默认日志级别
            var defaultLogLevelStr = hostContext.Configuration["Logging:LogLevel:Default"];

            if (string.IsNullOrEmpty(defaultLogLevelStr))
            {
                Penetrates.DefaultLogLevel = LogLevel.Error;
            }
            else
            {
                // 转换为 LogLevel 枚举
                Penetrates.DefaultLogLevel = Enum.TryParse(defaultLogLevelStr, out LogLevel defaultLogLevel)
                    ? defaultLogLevel
                    : LogLevel.Error;
            }

            var fileSizeLimitBytes = 0L;

            // 获取日志文件大小
            var fileSizeLimitBytesStr = hostContext.Configuration["Logging:FileSizeLimitBytes"];

            if (string.IsNullOrEmpty(fileSizeLimitBytesStr))
            {
                // 默认设置为 10MB / 10485760B
                fileSizeLimitBytes = 1024 * 1024 * 10;
            }
            else
            {
                fileSizeLimitBytes = long.TryParse(fileSizeLimitBytesStr, out fileSizeLimitBytes)
                    ? fileSizeLimitBytes
                    : 1024 * 1024 * 10;
            }

            // 默认根据年月日时分类
            const string logFileFormat = "{0:yyyy}/{0:MM}/{0:dd}/{0:HH}";

            services.AddLogging(loggingBuilder =>
            {
                // 添加控制台默认格式化器
                loggingBuilder.AddConsoleFormatter();

                // 根据默认日志级别创建对应的文件日志
                if (LogLevel.Trace >= Penetrates.DefaultLogLevel)
                {
                    loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>(_ =>
                        new FileLoggerProvider($"logs/trace/{logFileFormat}.log",
                            GetLogOptions(LogLevel.Trace, fileSizeLimitBytes))));
                }

                if (LogLevel.Debug >= Penetrates.DefaultLogLevel)
                {
                    loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>(_ =>
                        new FileLoggerProvider($"logs/debug/{logFileFormat}.log",
                            GetLogOptions(LogLevel.Debug, fileSizeLimitBytes))));
                }

                if (LogLevel.Information >= Penetrates.DefaultLogLevel)
                {
                    loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>(_ =>
                        new FileLoggerProvider($"logs/information/{logFileFormat}.log",
                            GetLogOptions(LogLevel.Information, fileSizeLimitBytes))));
                }

                if (LogLevel.Warning >= Penetrates.DefaultLogLevel)
                {
                    loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>(_ =>
                        new FileLoggerProvider($"logs/warning/{logFileFormat}.log",
                            GetLogOptions(LogLevel.Warning, fileSizeLimitBytes))));
                }

                if (LogLevel.Error >= Penetrates.DefaultLogLevel)
                {
                    loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>(_ =>
                        new FileLoggerProvider($"logs/error/{logFileFormat}.log",
                            GetLogOptions(LogLevel.Error, fileSizeLimitBytes))));
                }

                if (LogLevel.Critical >= Penetrates.DefaultLogLevel)
                {
                    loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>(_ =>
                        new FileLoggerProvider($"logs/critical/{logFileFormat}.log",
                            GetLogOptions(LogLevel.Critical, fileSizeLimitBytes))));
                }
            });
        });
    }

    /// <summary>
    /// 配置日志
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="fileSizeLimitBytes">日志文件大小 控制每一个日志文件最大存储大小，默认无限制，单位是 B，也就是 1024 才等于 1KB</param>
    private static FileLoggerOptions GetLogOptions(LogLevel logLevel, long fileSizeLimitBytes)
    {
        return new FileLoggerOptions
        {
            IncludeScopes = true,
            WithTraceId = true,
            WithStackFrame = true,
            FileNameRule = fileName => string.Format(fileName, DateTime.Now, logLevel.ToString()),
            WriteFilter = logMsg => logMsg.LogLevel == logLevel,
            FileSizeLimitBytes = fileSizeLimitBytes,
            MessageFormat = logMsg =>
            {
                var msg = new List<string>
                {
                    $"{logMsg.LogName}",
                    $"##日志时间## {DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}",
                    $"##日志等级## {logLevel}",
                    $"##日志内容## {logMsg.Message}",
                };
                if (!string.IsNullOrEmpty(logMsg.Exception?.ToString()))
                    msg.Add($"##异常信息## {logMsg.Exception}");

                // Generating template strings.
                var template = TP.Wrapper("Fast.NET", "", msg.ToArray());
                return template;
            },
            HandleWriteError = err =>
            {
                err.UseRollbackFileName(Path.GetFileNameWithoutExtension(err.CurrentFileName) + "_alt" +
                                        Path.GetExtension(err.CurrentFileName));
            },
        };
    }
}