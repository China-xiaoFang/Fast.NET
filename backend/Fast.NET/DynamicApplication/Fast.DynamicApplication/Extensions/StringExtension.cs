using System.Text;
using System.Text.RegularExpressions;

namespace Fast.DynamicApplication.Extensions;

/// <summary>
/// <see cref="string"/> 拓展类
/// </summary>
internal static class StringExtension
{
    /// <summary>
    /// 切割骆驼命名式字符串
    /// </summary>
    /// <param name="str"><see cref="string"/></param>
    /// <returns><see cref="string"/></returns>
    internal static string[] SplitCamelCase(this string str)
    {
        if (str == null)
            return Array.Empty<string>();

        if (string.IsNullOrWhiteSpace(str))
            return new[] {str};
        if (str.Length == 1)
            return new[] {str};

        return Regex.Split(str, @"(?=\p{Lu}\p{Ll})|(?<=\p{Ll})(?=\p{Lu})").Where(u => u.Length > 0).ToArray();
    }

    /// <summary>
    /// 字符串首字母小写
    /// </summary>
    /// <param name="str"><see cref="string"/></param>
    /// <returns><see cref="string"/></returns>
    internal static string FirstCharToLower(this string str)
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