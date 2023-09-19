using System;
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

    /// <summary>
    /// 清除字符串前后缀
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="pos">0：前后缀，1：后缀，-1：前缀</param>
    /// <param name="affixes">前后缀集合</param>
    /// <returns></returns>
    public static string ClearStringAffixes(this string str, int pos = 0, params string[] affixes)
    {
        // 空字符串直接返回
        if (string.IsNullOrWhiteSpace(str)) return str;

        // 空前后缀集合直接返回
        if (affixes == null || affixes.Length == 0) return str;

        var startCleared = false;
        var endCleared = false;

        string tempStr = null;
        foreach (var affix in affixes)
        {
            if (string.IsNullOrWhiteSpace(affix)) continue;

            if (pos != 1 && !startCleared && str.StartsWith(affix, StringComparison.OrdinalIgnoreCase))
            {
                tempStr = str[affix.Length..];
                startCleared = true;
            }
            if (pos != -1 && !endCleared && str.EndsWith(affix, StringComparison.OrdinalIgnoreCase))
            {
                var _tempStr = !string.IsNullOrWhiteSpace(tempStr) ? tempStr : str;
                tempStr = _tempStr[..^affix.Length];
                endCleared = true;
            }
            if (startCleared && endCleared) break;
        }

        return !string.IsNullOrWhiteSpace(tempStr) ? tempStr : str;
    }
}