using Fast.Core.Util.Logging.Component;
using Fast.Core.Util.Logging.Format;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Fast.Core.Util.Logging.Extension;

/// <summary>
/// 日志写入文件扩展
/// </summary>
public static class LoggingFileExtensions
{
    /// <summary>
    /// 添加WorkerService项目控制台日志格式化扩展
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostBuilder UseLoggingFile(this IHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole(options => { options.FormatterName = "custom_format"; })
                .AddConsoleFormatter<ConsoleFormat, ConsoleFormatterOptions>();
        });
        builder.ConfigureServices((hostContext, services) => { services.AddComponent<LoggingFileComponent>(); });
        return builder;
    }


    /// <summary>
    /// 添加api项目控制台日志格式化扩展
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder UseLoggingFile(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options => { options.FormatterName = "custom_format"; })
            .AddConsoleFormatter<ConsoleFormat, ConsoleFormatterOptions>();
        builder.Services.AddComponent<LoggingFileComponent>();
        return builder;
    }
}