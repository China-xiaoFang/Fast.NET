using Furion;
using Furion.Logging;
using Furion.Templates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fast.ServiceCollection.ServiceCollection;

/// <summary>
/// 日志
/// </summary>
public static class Logging
{
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
            loggingBuilder.AddFile($"logs/error/{logFileFormat}_0.log",
                options => { SetLogOptions(options, LogLevel.Error, fileSizeLimitBytes); });
            // Environments other than the development environment are not logged.
            if (!App.HostEnvironment.IsDevelopment())
                return;
            loggingBuilder.AddFile($"logs/info/{logFileFormat}_0.log",
                options => { SetLogOptions(options, LogLevel.Information, fileSizeLimitBytes); });
            loggingBuilder.AddFile($"logs/warn/{logFileFormat}_0.log",
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
        //options.WithStackFrame = true;
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
            var template = TP.Wrapper("Fast.NET", "", msg.ToArray());
            return template;
        };
    }
}