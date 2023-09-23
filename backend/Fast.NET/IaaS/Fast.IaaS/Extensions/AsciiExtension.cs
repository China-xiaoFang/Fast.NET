using System.Globalization;
using System.Text;

namespace Fast.IaaS.Extensions;

/// <summary>
/// Ascii 拓展类
/// </summary>
public static class AsciiExtension
{
    /// <summary>
    /// 将字符串转换为 ASCII 编码形式。
    /// </summary>
    /// <param name="str">要进行编码的字符串。</param>
    /// <returns>转换后的 ASCII 编码形式字符串。</returns>
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
    /// <param name="str">要进行解码的 ASCII 编码形式字符串。</param>
    /// <returns>解码后的字符串。</returns>
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
}