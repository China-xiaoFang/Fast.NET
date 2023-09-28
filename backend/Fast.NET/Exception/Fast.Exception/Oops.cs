namespace Fast.Exception;

/// <summary>
/// 抛异常静态类
/// </summary>
public static class Oops
{
    /// <summary>
    /// 抛出业务异常信息
    /// </summary>
    /// <param name="errorMessage"><see cref="string"/>异常消息</param>
    /// <param name="args">String.Format 参数</param>
    /// <returns><see cref="UserFriendlyException"/>异常实例</returns>
    [Obsolete("This method is deprecated, use throw new UserFriendlyException() instead.")]
    public static UserFriendlyException Bah(string errorMessage, params object[] args)
    {
        return new UserFriendlyException(string.Format(errorMessage, args));
    }

    /// <summary>
    /// 抛出字符串异常
    /// </summary>
    /// <param name="errorMessage"><see cref="string"/>异常消息</param>
    /// <param name="args">String.Format 参数</param>
    /// <returns><see cref="AppException"/>异常实例</returns>
    [Obsolete("This method is deprecated, use throw new AppException() instead.")]
    public static AppException Oh(string errorMessage, params object[] args)
    {
        return new AppException(string.Format(errorMessage, args));
    }

    /// <summary>
    /// 抛出字符串异常
    /// </summary>
    /// <param name="errorMessage"><see cref="string"/>异常消息</param>
    /// <param name="exceptionType">具体异常类型</param>
    /// <param name="args">String.Format 参数</param>
    /// <returns><see cref="AppException"/>异常实例</returns>
    [Obsolete("This method is deprecated, use throw new AppException() instead.")]
    public static AppException Oh(string errorMessage, Type exceptionType, params object[] args)
    {
        var exceptionMessage = string.Format(errorMessage, args);
        return new AppException(exceptionMessage,
            innerException: Activator.CreateInstance(exceptionType, exceptionMessage) as System.Exception);
    }

    /// <summary>
    /// 抛出字符串异常
    /// </summary>
    /// <typeparam name="TException">具体异常类型</typeparam>
    /// <param name="errorMessage"><see cref="string"/>异常消息</param>
    /// <param name="args">String.Format 参数</param>
    /// <returns><see cref="AppException"/>异常实例</returns>
    [Obsolete("This method is deprecated, use throw new AppException() instead.")]
    public static AppException Oh<TException>(string errorMessage, params object[] args) where TException : class
    {
        return Oh(errorMessage, typeof(TException), args);
    }
}