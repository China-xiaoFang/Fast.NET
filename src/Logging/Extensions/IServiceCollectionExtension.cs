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

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Fast.Logging;

/// <summary>
/// <see cref="IServiceCollection"/> 动态Api 拓展类
/// </summary>
[SuppressSniffer]
public static class IServiceCollectionExtension
{
    /// <summary>
    /// 注册日志服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="hostEnvironment"><see cref="IHostEnvironment"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddLoggingService(this IServiceCollection services, IHostEnvironment hostEnvironment)
    {
        Debugging.Info("Registering logging......");

        // 存储服务提供器
        Penetrates.InternalServices = services;

        // 获取默认日志级别
        Penetrates.DefaultLogLevel = LogLevel.Error;

        if (hostEnvironment.IsDevelopment())
        {
            Penetrates.DefaultLogLevel = LogLevel.Information;
        }

        // 默认设置为 10MB / 10485760B
        var fileSizeLimitBytes = 1024 * 1024 * 10;

        // 默认根据年月日时分类
        const string logFileFormat = "{0:yyyy}/{0:MM}/{0:dd}/{0:HH}";

        services.AddLogging(loggingBuilder =>
        {
            // 添加控制台默认格式化器
            loggingBuilder.AddConsoleFormatter();

            if (hostEnvironment.IsDevelopment())
            {
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
            }
            else
            {
                loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>(_ =>
                    new FileLoggerProvider("logs/{0:yyyy}/{0:MM}/{0:dd}/error_{0:HH}.log",
                        GetLogOptions(LogLevel.Error, fileSizeLimitBytes))));
            }
        });

        // 注册 Logging Startup 过滤器
        services.AddTransient(typeof(IStartupFilter), typeof(LoggingStartupFilter));

        return services;
    }

    /// <summary>
    /// 注册日志服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="configure"><see cref="Action{T}"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddLoggingService(this IServiceCollection services, Action<ILoggingBuilder, int> configure)
    {
        Debugging.Info("Registering logging......");

        // 存储服务提供器
        Penetrates.InternalServices = services;

        // 默认设置为 10MB / 10485760B
        var fileSizeLimitBytes = 1024 * 1024 * 10;

        services.AddLogging(loggingBuilder =>
        {
            // 添加控制台默认格式化器
            loggingBuilder.AddConsoleFormatter();

            configure.Invoke(loggingBuilder, fileSizeLimitBytes);
        });

        // 注册 Logging Startup 过滤器
        services.AddTransient(typeof(IStartupFilter), typeof(LoggingStartupFilter));

        return services;
    }

    /// <summary>
    /// 注册错误日志服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddErrorLoggingService(this IServiceCollection services)
    {
        Debugging.Info("Registering logging......");

        // 存储服务提供器
        Penetrates.InternalServices = services;

        // 默认设置为 10MB / 10485760B
        var fileSizeLimitBytes = 1024 * 1024 * 10;

        services.AddLogging(loggingBuilder =>
        {
            // 添加控制台默认格式化器
            loggingBuilder.AddConsoleFormatter();

            loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>(_ =>
                new FileLoggerProvider("logs/error_{0:yyyy}-{0:MM}.log", GetLogOptions(LogLevel.Error, fileSizeLimitBytes))));
        });

        // 注册 Logging Startup 过滤器
        services.AddTransient(typeof(IStartupFilter), typeof(LoggingStartupFilter));

        return services;
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
                    $"##日志内容## {logMsg.Message}"
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
            }
        };
    }
}