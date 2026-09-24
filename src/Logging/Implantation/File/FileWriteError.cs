// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

namespace Fast.Logging;

/// <summary>
/// 文件写入错误信息上下文
/// </summary>
internal sealed class FileWriteError
{
    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    /// <param name="currentFileName">当前日志文件名</param>
    /// <param name="exception">异常对象</param>
    internal FileWriteError(string currentFileName, Exception exception)
    {
        CurrentFileName = currentFileName;
        Exception = exception;
    }

    /// <summary>
    /// 当前日志文件名
    /// </summary>
    public string CurrentFileName { get; private set; }

    /// <summary>
    /// 引起文件写入异常信息
    /// </summary>
    public Exception Exception { get; private set; }

    /// <summary>
    /// 备用日志文件名
    /// </summary>
    internal string RollbackFileName { get; private set; }

    /// <summary>
    /// 配置日志文件写入错误后新的备用日志文件名
    /// </summary>
    /// <param name="rollbackFileName">备用日志文件名</param>
    public void UseRollbackFileName(string rollbackFileName)
    {
        RollbackFileName = rollbackFileName;
    }
}
