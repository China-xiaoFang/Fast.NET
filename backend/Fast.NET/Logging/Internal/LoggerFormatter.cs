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

using System.Text.Json;
using Fast.Logging.Extensions;
using Fast.Logging.Implantation;

namespace Fast.Logging.Internal;

/// <summary>
/// 日志格式化静态类
/// </summary>
public static class LoggerFormatter
{
    /// <summary>
    /// Json 输出格式化
    /// </summary>
    public static readonly Func<LogMessage, string> Json = logMsg =>
    {
        return logMsg.Write(writer => WriteJson(logMsg, writer));
    };

    /// <summary>
    /// Json 输出格式化
    /// </summary>
    public static readonly Func<LogMessage, string> JsonIndented = logMsg =>
    {
        return logMsg.Write(writer => WriteJson(logMsg, writer), true);
    };

    /// <summary>
    /// 写入 JSON
    /// </summary>
    /// <param name="logMsg"></param>
    /// <param name="writer"></param>
    private static void WriteJson(LogMessage logMsg, Utf8JsonWriter writer)
    {
        writer.WriteStartObject();

        // 输出日志级别
        writer.WriteString("logLevel", logMsg.LogLevel.ToString());

        // 输出日志时间
        writer.WriteString("logDateTime", logMsg.LogDateTime.ToString("o"));

        // 输出日志类别
        writer.WriteString("logName", logMsg.LogName);

        // 输出日志事件 Id
        writer.WriteNumber("eventId", logMsg.EventId.Id);

        // 输出日志消息
        writer.WriteString("message", logMsg.Message);

        // 输出日志所在线程 Id
        writer.WriteNumber("threadId", logMsg.ThreadId);

        // 输出是否使用 UTC 时间戳
        writer.WriteBoolean("useUtcTimestamp", logMsg.UseUtcTimestamp);

        // 输出请求 TraceId
        writer.WriteString("traceId", logMsg.TraceId);

        // 输出异常信息
        writer.WritePropertyName("exception");
        if (logMsg.Exception == null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(logMsg.Exception.ToString());

        writer.WriteEndObject();
    }
}