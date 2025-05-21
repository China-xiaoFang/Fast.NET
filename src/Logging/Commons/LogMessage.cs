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

using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Fast.Logging;

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
    public LogMessage(string logName, LogLevel logLevel, EventId? eventId, string message, Exception exception,
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
    public EventId? EventId { get; }

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
        return LoggingContext.OutputStandardMessage(this);
    }
}