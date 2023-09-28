using Microsoft.AspNetCore.Http;

// ReSharper disable once CheckNamespace
namespace Fast.Exception;

/// <summary>
/// <see cref="UserFriendlyException"/> 用户友好异常
/// </summary>
public class UserFriendlyException : System.Exception
{
    /// <summary>
    /// 异常编码
    /// </summary>
    public object Code { get; set; }

    /// <summary>
    /// <inheritdoc cref="UserFriendlyException" />
    /// </summary>
    public UserFriendlyException() : base("Bad Request")
    {
        Code = StatusCodes.Status400BadRequest;
    }

    /// <summary>
    /// <inheritdoc cref="UserFriendlyException" />
    /// </summary>
    /// <param name="message">异常信息</param>
    public UserFriendlyException(string message) : base(message ?? "Bad Request")
    {
    }

    /// <summary>
    /// <inheritdoc cref="UserFriendlyException" />
    /// </summary>
    /// <param name="message">异常信息</param>
    /// <param name="code">异常编码</param>
    public UserFriendlyException(string message, object code) : base(message ?? "Bad Request")
    {
        Code = code;
    }

    /// <summary>
    /// <inheritdoc cref="UserFriendlyException" />
    /// </summary>
    /// <param name="message">异常信息</param>
    /// <param name="innerException">内部异常</param>
    public UserFriendlyException(string message, System.Exception innerException) : base(message ?? "Bad Request", innerException)
    {
    }

    /// <summary>
    /// <inheritdoc cref="UserFriendlyException" />
    /// </summary>
    /// <param name="message">异常信息</param>
    /// <param name="innerException">内部异常</param>
    /// <param name="code">异常编码</param>
    public UserFriendlyException(string message, object code, System.Exception innerException) : base(message ?? "Bad Request",
        innerException)
    {
        Code = code;
    }
}