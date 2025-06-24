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

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="StringEncodingExtension"/> 字符串编码拓展类
/// </summary>
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
    /// <para>如果已经 URL 编码则不会继续编码</para>
    /// </summary>
    /// <param name="str"><see cref="string"/></param>
    /// <returns><see cref="string"/></returns>
    public static string UrlEncode(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return "";
        }

        var result = HttpUtility.UrlEncode(str, Encoding.UTF8);

        try
        {
            // 尝试解密，避免再次编码已经是 URL 编码的字符串
            var tryDecode = HttpUtility.UrlDecode(str, Encoding.UTF8);
            // 如果解码后不相同，则直接返回原来的
            return str.Equals(tryDecode, StringComparison.OrdinalIgnoreCase) ? result : str;
        }
        catch
        {
            // 报错了，不管直接编码返回
            return result;
        }
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