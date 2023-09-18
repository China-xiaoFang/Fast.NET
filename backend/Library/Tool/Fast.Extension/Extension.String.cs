using System.Text;

namespace Fast.Extension;

/// <summary>
/// 位扩展
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
        // 空检查
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        // 初始化字符串构建器
        var stringBuilder = new StringBuilder(str);

        // 设置字符串构建器首个字符为小写
        stringBuilder[0] = char.ToUpper(stringBuilder[0]);

        return stringBuilder.ToString();
    }

    /// <summary>
    /// 字符串首字母小写
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string FirstCharToLower(this string str)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        // 初始化字符串构建器
        var stringBuilder = new StringBuilder(str);

        // 设置字符串构建器首个字符为小写
        stringBuilder[0] = char.ToLower(stringBuilder[0]);

        return stringBuilder.ToString();
    }
}