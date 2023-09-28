using Microsoft.AspNetCore.Http;

// ReSharper disable once CheckNamespace
namespace Fast.Exception;

/// <summary>
/// <see cref="AppException"/> 应用程序异常类
/// </summary>
public class AppException : System.Exception
{
    /// <summary>
    /// 异常编码
    /// </summary>
    public object Code { get; set; }

    /// <summary>
    /// <inheritdoc cref="AppException" />
    /// </summary>
    public AppException() : base("Internal Server Error.")
    {
        Code = StatusCodes.Status500InternalServerError;
    }

    /// <summary>
    /// <inheritdoc cref="AppException" />
    /// </summary>
    /// <param name="message">异常信息</param>
    public AppException(string message) : base(message ?? "Internal Server Error.")
    {
        Code = StatusCodes.Status500InternalServerError;
    }

    /// <summary>
    /// <inheritdoc cref="AppException" />
    /// </summary>
    /// <param name="message">异常信息</param>
    /// <param name="code">异常编码</param>
    public AppException(string message, object code) : base(message ?? "Internal Server Error.")
    {
        Code = code;
    }

    /// <summary>
    /// <inheritdoc cref="AppException" />
    /// </summary>
    /// <param name="message">异常信息</param>
    /// <param name="innerException">内部异常</param>
    public AppException(string message, System.Exception innerException) : base(message ?? "Internal Server Error.",
        innerException)
    {
        Code = StatusCodes.Status500InternalServerError;
    }

    /// <summary>
    /// <inheritdoc cref="AppException" />
    /// </summary>
    /// <param name="message">异常信息</param>
    /// <param name="innerException">内部异常</param>
    /// <param name="code">异常编码</param>
    public AppException(string message, object code, System.Exception innerException) : base(message ?? "Internal Server Error.",
        innerException)
    {
        Code = code;
    }
}