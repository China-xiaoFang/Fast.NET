using Furion.Logging;
using Furion.Templates;
using Microsoft.Extensions.Logging;

namespace Fast.Core.ServiceCollection;

/// <summary>
/// 日志
/// </summary>
public static class Logging
{
    /// <summary>
    /// 添加日志服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="logFileFormat">日志文件格式</param>
    /// <param name="isRun"></param>
    public static void AddLogging(this IServiceCollection services, string logFileFormat = "{0:yyyy-MM-dd}", bool isRun = true)
    {
        if (isRun)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFile($"logs/error/{logFileFormat}_0.log", options => { SetLogOptions(options, LogLevel.Error); });
                // Environments other than the development environment are not logged.
                if (!HostEnvironment.IsDevelopment())
                    return;
                loggingBuilder.AddFile($"logs/info/{logFileFormat}_0.log",
                    options => { SetLogOptions(options, LogLevel.Information); });
                loggingBuilder.AddFile($"logs/warn/{logFileFormat}_0.log",
                    options => { SetLogOptions(options, LogLevel.Warning); });
            });
        }
    }

    /// <summary>
    /// 配置日志
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logLevel"></param>
    private static void SetLogOptions(FileLoggerOptions options, LogLevel logLevel)
    {
        options.WriteFilter = logMsg => logMsg.LogLevel == logLevel;
        options.FileNameRule = fileName => string.Format(fileName, DateTime.UtcNow);
        options.FileSizeLimitBytes = 10 * 1024 * 1024; // 10MB
        options.MessageFormat = logMsg =>
        {
            var msg = new List<string>
            {
                $"##日志时间## {DateTime.Now:yyyy-MM-dd HH:mm:ss}", $"##日志等级## {logLevel}", $"##日志内容## {logMsg.Message}",
            };
            if (!string.IsNullOrEmpty(logMsg.Exception?.ToString()))
                msg.Add($"##异常信息## {logMsg.Exception}");

            // Generating template strings.
            var template = TP.Wrapper($"{logMsg.LogName}", "", msg.ToArray());
            return template;
        };
    }
}