namespace Hello.NET.Iaas.Extension;

/// <summary>
/// 字符串扩展
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// 字符串首字母大写
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string FirstCharToUpper(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;

        return str.First().ToString().ToUpper() + str[1..];
    }

    /// <summary>
    /// 字符串首字母小写
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string FirstCharToLower(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;

        return str.First().ToString().ToLower() + str[1..];
    }
}