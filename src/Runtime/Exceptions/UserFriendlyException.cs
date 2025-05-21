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

using Microsoft.AspNetCore.Http;

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>
/// <see cref="UserFriendlyException"/> 用户友好异常
/// </summary>
[SuppressSniffer]
public class UserFriendlyException : Exception
{
    /// <summary>
    /// <inheritdoc cref="UserFriendlyException" />
    /// </summary>
    public UserFriendlyException() : base("Bad Request")
    {
        ErrorCode = StatusCodes.Status400BadRequest;
    }

    /// <summary>
    /// <inheritdoc cref="UserFriendlyException" />
    /// </summary>
    /// <param name="message">异常信息</param>
    public UserFriendlyException(string message) : base(message ?? "Bad Request")
    {
        ErrorMessage = message;
    }

    /// <summary>
    /// <inheritdoc cref="UserFriendlyException" />
    /// </summary>
    /// <param name="message">异常信息</param>
    /// <param name="errorCode">错误编码</param>
    public UserFriendlyException(string message, object errorCode) : base(message ?? "Bad Request")
    {
        ErrorMessage = message;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// <inheritdoc cref="UserFriendlyException" />
    /// </summary>
    /// <param name="message">异常信息</param>
    /// <param name="innerException">内部异常</param>
    public UserFriendlyException(string message, Exception innerException) : base(message ?? "Bad Request", innerException)
    {
        ErrorMessage = message;
    }

    /// <summary>
    /// <inheritdoc cref="UserFriendlyException" />
    /// </summary>
    /// <param name="message">异常信息</param>
    /// <param name="errorCode">错误编码</param>
    /// <param name="innerException">内部异常</param>
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
    /// 错误码（没被复写过的 ErrorCode ）
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