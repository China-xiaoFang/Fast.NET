// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using Microsoft.Extensions.Logging;

namespace Fast.Logging;

/// <summary>
/// 构建字符串日志部分类
/// </summary>
[SuppressSniffer]
public sealed partial class StringLoggingPart
{
    /// <summary>
    /// 静态缺省日志部件
    /// </summary>
    /// <returns>静态缺省日志部件</returns>
    public static StringLoggingPart Default()
    {
        return new StringLoggingPart();
    }

    /// <summary>
    /// 日志内容
    /// </summary>
    public string Message { get; private set; }

    /// <summary>
    /// 日志级别
    /// </summary>
    public LogLevel Level { get; private set; } = LogLevel.Information;

    /// <summary>
    /// 消息格式化参数
    /// </summary>
    public object[] Args { get; private set; }

    /// <summary>
    /// 事件Id
    /// </summary>
    public EventId? EventId { get; private set; }

    /// <summary>
    /// 日志分类类型
    /// </summary>
    public Type CategoryType { get; private set; } = typeof(StringLogging);

    /// <summary>
    /// 异常对象
    /// </summary>
    public Exception Exception { get; private set; }

    /// <summary>
    /// 日志对象所在作用域
    /// </summary>
    public IServiceProvider LoggerScoped { get; private set; }

    /// <summary>
    /// 日志上下文
    /// </summary>
    public LogContext LogContext { get; private set; }
}
