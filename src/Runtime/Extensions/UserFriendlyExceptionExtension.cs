// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using Microsoft.AspNetCore.Http;

namespace Fast.Runtime;

/// <summary>
/// 为 <see cref="UserFriendlyException"/> 提供扩展方法
/// </summary>
[SuppressSniffer]
public static class UserFriendlyExceptionExtension
{
    /// <summary>
    /// 设置异常状态码
    /// </summary>
    /// <param name="exception">要处理的异常</param>
    /// <param name="statusCode">HTTP 状态码</param>
    /// <returns>设置异常状态码</returns>
    public static UserFriendlyException StatusCode(this UserFriendlyException exception,
        int statusCode = StatusCodes.Status400BadRequest)
    {
        exception.StatusCode = statusCode;
        return exception;
    }

    /// <summary>
    /// 设置额外数据
    /// </summary>
    /// <param name="exception">要处理的异常</param>
    /// <param name="data">要处理或传输的数据</param>
    /// <returns>设置额外数据</returns>
    public static UserFriendlyException WithData(this UserFriendlyException exception, object data)
    {
        exception.Data = data;
        return exception;
    }
}
