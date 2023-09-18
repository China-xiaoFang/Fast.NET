using System;

namespace Fast.Logging.Implantations.Database;

/// <summary>
/// 数据库写入错误信息上下文
/// </summary>
public sealed class DatabaseWriteError
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="exception">异常对象</param>
    internal DatabaseWriteError(Exception exception)
    {
        Exception = exception;
    }

    /// <summary>
    /// 引起数据库写入异常信息
    /// </summary>
    public Exception Exception { get; private set; }
}