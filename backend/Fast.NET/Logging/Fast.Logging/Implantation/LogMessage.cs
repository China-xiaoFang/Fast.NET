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
using Fast.Logging.Internal;
using Microsoft.Extensions.Logging;

namespace Fast.Logging.Implantation;

/// <summary>
/// 日志结构化消息
/// </summary>
[SuppressSniffer]
public struct LogMessage
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logName">记录器类别名称</param>
    /// <param name="logLevel">日志级别</param>
    /// <param name="eventId">事件 Id</param>
    /// <param name="message">日志消息</param>
    /// <param name="exception">异常对象</param>
    /// <param name="context">日志上下文</param>
    /// <param name="state">当前状态值</param>
    /// <param name="logDateTime">日志记录时间</param>
    /// <param name="threadId">线程 Id</param>
    /// <param name="useUtcTimestamp">是否使用 UTC 时间戳</param>
    /// <param name="traceId">请求/跟踪 Id</param>
    internal LogMessage(string logName, LogLevel logLevel, EventId eventId, string message, Exception exception,
        LogContext context, object state, DateTime logDateTime, int threadId, bool useUtcTimestamp, string traceId)
    {
        LogName = logName;
        Message = message;
        LogLevel = logLevel;
        EventId = eventId;
        Exception = exception;
        Context = context;
        State = state;
        LogDateTime = logDateTime;
        ThreadId = threadId;
        UseUtcTimestamp = useUtcTimestamp;
        TraceId = traceId;
    }

    /// <summary>
    /// 记录器类别名称
    /// </summary>
    public string LogName { get; }

    /// <summary>
    /// 日志级别
    /// </summary>
    public LogLevel LogLevel { get; }

    /// <summary>
    /// 事件 Id
    /// </summary>
    public EventId EventId { get; }

    /// <summary>
    /// 日志消息
    /// </summary>
    public string Message { get; internal set; }

    /// <summary>
    /// 异常对象
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// 当前状态值
    /// </summary>
    /// <remarks>可以是任意类型</remarks>
    public object State { get; }

    /// <summary>
    /// 日志记录时间
    /// </summary>
    public DateTime LogDateTime { get; }

    /// <summary>
    /// 线程 Id
    /// </summary>
    public int ThreadId { get; }

    /// <summary>
    /// 是否使用 UTC 时间戳
    /// </summary>
    public bool UseUtcTimestamp { get; }

    /// <summary>
    /// 请求/跟踪 Id
    /// </summary>
    public string TraceId { get; }

    /// <summary>
    /// 日志上下文
    /// </summary>
    public LogContext Context { get; set; }

    /// <summary>
    /// 重写默认输出
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public readonly override string ToString()
    {
        return Penetrates.OutputStandardMessage(this);
    }
}