// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="StringEncodingExtension"/> 字符串编码拓展类
/// </summary>
[SuppressSniffer]
public static class StringEncodingExtension
{
    #region ASCII

    /// <summary>
    /// 将字符串转换为 ASCII 编码形式。
    /// </summary>
    /// <param name="str"><see cref="string"/> 要进行编码的字符串。</param>
    /// <returns><see cref="string"/> 转换后的 ASCII 编码形式字符串。</returns>
    public static string EnAscii(this string str)
    {
        // 使用默认编码将字符串转换为字节数组
        var textBuf = Encoding.Default.GetBytes(str);

        // 将每个字节转换为两位的十六进制数，并拼接起来
        return textBuf.Aggregate(string.Empty, (current, t) => current + t.ToString("X"));
    }

    /// <summary>
    /// 将 ASCII 编码形式的字符串转换为字符串。
    /// </summary>
    /// <param name="str"><see cref="string"/> 要进行解码的 ASCII 编码形式字符串。</param>
    /// <returns><see cref="string"/> 解码后的字符串。</returns>
    public static string DeAscii(this string str)
    {
        var k = 0;
        // 创建一个字节数组，长度为输入字符串长度的一半
        var buffer = new byte[str.Length / 2];
        for (var i = 0; i < str.Length / 2; i++)
        {
            // 从 ASCII 编码形式的字符串中提取两位十六进制数，将其转换为字节
            buffer[i] = byte.Parse(str.Substring(k, 2), NumberStyles.HexNumber);
            k += 2;
        }

        // 使用默认编码将字节数组转换为字符串
        return Encoding.Default.GetString(buffer);
    }

    #endregion

    #region Unicode

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

    #endregion

    #region Url

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

    #endregion
}