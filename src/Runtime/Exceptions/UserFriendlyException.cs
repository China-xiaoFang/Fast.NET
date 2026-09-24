// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using Microsoft.AspNetCore.Http;


// ReSharper disable once CheckNamespace
namespace System;

/// <summary>
/// 用户友好异常
/// </summary>
[SuppressSniffer]
public class UserFriendlyException : Exception
{
    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    public UserFriendlyException() : base("Bad Request")
    {
        ErrorCode = StatusCodes.Status400BadRequest;
    }

    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    public UserFriendlyException(string message) : base(message ?? "Bad Request")
    {
        ErrorMessage = message;
    }

    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    /// <param name="errorCode">用于标识错误类型的错误码</param>
    public UserFriendlyException(string message, object errorCode) : base(message ?? "Bad Request")
    {
        ErrorMessage = message;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    /// <param name="innerException">导致当前异常的内部异常</param>
    public UserFriendlyException(string message, Exception innerException) : base(message ?? "Bad Request", innerException)
    {
        ErrorMessage = message;
    }

    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    /// <param name="errorCode">用于标识错误类型的错误码</param>
    /// <param name="innerException">导致当前异常的内部异常</param>
    public UserFriendlyException(string message, object errorCode, Exception innerException) : base(message ?? "Bad Request",
        innerException)
    {
        ErrorMessage = message;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// 错误码
    /// </summary>
    public object ErrorCode { get; set; }

    /// <summary>
    /// 原始错误码（未被覆盖的 <see cref="ErrorCode"/>）
    /// </summary>
    public object OriginErrorCode { get; set; }

    /// <summary>
    /// 错误消息（支持 Object 对象）
    /// </summary>
    public object ErrorMessage { get; set; }

    /// <summary>
    /// 状态码
    /// </summary>
    public int StatusCode { get; set; } = StatusCodes.Status400BadRequest;

    /// <summary>
    /// 是否是数据验证异常
    /// </summary>
    public bool ValidationException { get; set; } = false;

    /// <summary>
    /// 额外数据
    /// </summary>
    public new object Data { get; set; }
}
