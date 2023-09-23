using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Fast.IaaS.Extensions;

/// <summary>
/// <see cref="string"/> 拓展类
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// 字符串首字母大写
    /// </summary>
    /// <param name="str"><see cref="string"/></param>
    /// <returns><see cref="string"/></returns>
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
    /// <param name="str"><see cref="string"/></param>
    /// <returns><see cref="string"/></returns>
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
    /// <param name="str"><see cref="string"/>字符串</param>
    /// <param name="pos">0：前后缀，1：后缀，-1：前缀</param>
    /// <param name="affixes">前后缀集合</param>
    /// <returns><see cref="string"/></returns>
    public static string ClearStringAffixes(this string str, int pos = 0, params string[] affixes)
    {
        // 空字符串直接返回
        if (string.IsNullOrWhiteSpace(str))
            return str;

        // 空前后缀集合直接返回
        if (affixes == null || affixes.Length == 0)
            return str;

        var startCleared = false;
        var endCleared = false;

        string tempStr = null;
        foreach (var affix in affixes)
        {
            if (string.IsNullOrWhiteSpace(affix))
                continue;

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

            if (startCleared && endCleared)
                break;
        }

        return !string.IsNullOrWhiteSpace(tempStr) ? tempStr : str;
    }

    /// <summary>
    /// 切割骆驼命名式字符串
    /// </summary>
    /// <param name="str"><see cref="string"/></param>
    /// <returns><see cref="string"/></returns>
    public static string[] SplitCamelCase(this string str)
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
    /// 格式化字符串
    /// </summary>
    /// <param name="str"><see cref="string"/></param>
    /// <param name="args"></param>
    /// <returns><see cref="string"/></returns>
    public static string Format(this string str, params object[] args)
    {
        return args == null || args.Length == 0 ? str : string.Format(str, args);
    }

    /// <summary>
    /// Unicode编码
    /// </summary>
    /// <param name="str"><see cref="string"/></param>
    /// <returns><see cref="string"/></returns>
    public static string EnUnicode(this string str)
    {
        var strResult = new StringBuilder();
        if (string.IsNullOrEmpty(str))
            return strResult.ToString();
        foreach (var c in str)
        {
            strResult.Append("\\u");
            strResult.Append(((int) c).ToString("x"));
        }

        return strResult.ToString();
    }

    /// <summary>
    /// Unicode解码
    /// </summary>
    /// <param name="str"><see cref="string"/></param>
    /// <returns><see cref="string"/></returns>
    public static string DeUnicode(this string str)
    {
        //最直接的方法Regex.Unescape(str);
        var reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
        return reg.Replace(str, m => ((char) Convert.ToInt32(m.Groups[1].Value, 16)).ToString());
    }

    /// <summary>
    /// 将字符串转化为固定长度左对齐，右补空格
    /// </summary>
    /// <param name="strTemp"><see cref="string"/></param>
    /// <param name="length"></param>
    /// <returns><see cref="string"/></returns>
    public static string PadStringLeftAlign(this string strTemp, int length)
    {
        var byteStr = Encoding.Default.GetBytes(strTemp.Trim());
        var iLength = byteStr.Length;
        var iNeed = length - iLength;
        var spaceLen = Encoding.Default.GetBytes(" "); //一个空格的长度
        iNeed = iNeed / spaceLen.Length;
        var spaceString = GenerateSpaceString(iNeed);
        return strTemp + spaceString;
    }

    /// <summary>
    /// 生成固定长度的空格字符串
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    static string GenerateSpaceString(int length)
    {
        var strReturn = string.Empty;

        if (length <= 0)
            return strReturn;
        for (var i = 0; i < length; i++)
        {
            strReturn += " ";
        }

        return strReturn;
    }

    /// <summary>
    /// 截取指定长度的字符串
    /// </summary>
    /// <param name="value"><see cref="string"/></param>
    /// <param name="length"></param>
    /// <param name="ellipsis"></param>
    /// <returns><see cref="string"/></returns>
    public static string GetSubStringWithEllipsis(this string value, int length, bool ellipsis = false)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        if (value.Length <= length)
            return value;
        value = value[..length];
        if (ellipsis)
        {
            value += "...";
        }

        return value;
    }

    /// <summary>
    /// 将一个字符串 URL 编码
    /// </summary>
    /// <param name="str"><see cref="string"/></param>
    /// <returns><see cref="string"/></returns>
    public static string UrlEncode(this string str)
    {
        return string.IsNullOrEmpty(str) ? "" : HttpUtility.UrlEncode(str, Encoding.UTF8);
    }

    /// <summary>
    /// 将一个Url 编码 转为字符串
    /// </summary>
    /// <param name="str"><see cref="string"/></param>
    /// <returns><see cref="string"/></returns>
    public static string UrlDecode(this string str)
    {
        return string.IsNullOrEmpty(str) ? "" : HttpUtility.UrlDecode(str, Encoding.UTF8);
    }
}