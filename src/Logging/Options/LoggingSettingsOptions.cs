// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using Fast.Runtime;
using Microsoft.Extensions.Logging;

namespace Fast.Logging;

/// <summary>
/// 日志配置选项
/// </summary>
[SuppressSniffer]
public class LoggingSettingsOptions : IPostConfigure
{
    /// <summary>
    /// 文件格式字符串
    /// </summary>
    public string FileFormat { get; set; }

    /// <summary>
    /// 文件大小限制（字节）
    /// </summary>
    public int? FileSizeLimit { get; set; }

    /// <summary>
    /// 最小日志级别
    /// </summary>
    public LogLevel? MiniLogLevel { get; set; }

    /// <summary>
    /// 启用 <see cref="LogLevel.Critical"/> 级别日志
    /// </summary>
    public bool? EnableCritical { get; set; }

    /// <inheritdoc />
    public void PostConfigure()
    {
        FileFormat ??= "/{0:yyyy}/{0:MM}/{0:dd}/{0:HH}";
        FileSizeLimit ??= 1024 * 1024 * 10;
        MiniLogLevel ??= LogLevel.Information;
        EnableCritical ??= false;
    }
}
