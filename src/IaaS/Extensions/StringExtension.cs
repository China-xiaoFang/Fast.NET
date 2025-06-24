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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

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

        return Regex.Split(str, @"(?=\p{Lu}\p{Ll})|(?<=\p{Ll})(?=\p{Lu})")
            .Where(u => u.Length > 0)
            .ToArray();
    }

    /// <summary>
    /// 字符串小驼峰命名
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        var arr = str.SplitCamelCase();

        if (arr.Length == 1)
        {
            return str.ToLower();
        }

        return $"{arr[0].ToLower()}{string.Join(string.Empty, arr.Skip(1))}";
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
    /// 获取字符长度
    /// </summary>
    /// <param name="str"><see cref="string"/></param>
    /// <returns><see cref="int"/></returns>
    public static int GetCharLength(this string str)
    {
        return Encoding.GetEncoding("GB18030")
            .GetByteCount(str);
    }

    /// <summary>
    /// 将字符串转化为固定长度左对齐，右补空格
    /// </summary>
    /// <param name="strTemp"><see cref="string"/></param>
    /// <param name="length"></param>
    /// <returns><see cref="string"/></returns>
    public static string PadStringLeftAlign(this string strTemp, int length)
    {
        strTemp ??= string.Empty;
        strTemp = strTemp.Trim();
        var iLength = strTemp.GetCharLength();
        var padCount = length - iLength;
        // 超长则不补
        if (padCount <= 0)
        {
            return strTemp;
        }

        return strTemp + new string(' ', padCount);
    }

    /// <summary>
    /// 将字符串转化为固定长度右对齐，左补空格
    /// </summary>
    /// <param name="strTemp"><see cref="string"/></param>
    /// <param name="length"></param>
    /// <returns><see cref="string"/></returns>
    public static string PadStringRightAlign(this string strTemp, int length)
    {
        strTemp ??= string.Empty;
        strTemp = strTemp.Trim();
        var iLength = strTemp.GetCharLength();
        var padCount = length - iLength;
        // 超长则不补
        if (padCount <= 0)
        {
            return strTemp;
        }

        return new string(' ', padCount) + strTemp;
    }

    /// <summary>
    /// 将字符串转换为固定长度的数组
    /// </summary>
    /// <param name="strTemp"><see cref="string"/></param>
    /// <param name="length"></param>
    /// <returns><see cref="string"/></returns>
    public static List<string> SplitString(this string strTemp, int length)
    {
        strTemp ??= string.Empty;
        strTemp = strTemp.Trim();
        var encoding = Encoding.GetEncoding("GB18030");
        var result = new List<string>();
        var sb = new StringBuilder();
        var curLen = 0;
        foreach (var c in strTemp)
        {
            var cLen = encoding.GetByteCount(new[] {c});
            if (curLen + cLen > length)
            {
                result.Add(sb.ToString());
                sb.Clear();
                curLen = 0;
            }

            sb.Append(c);
            curLen += cLen;
        }

        if (sb.Length > 0)
        {
            result.Add(sb.ToString());
        }

        return result;
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
    /// 获取 Sql Server NVarchar 最大字节长度
    /// </summary>
    /// <param name="str"><see cref="string"/></param>
    /// <param name="maxLen"><see cref="int"/>最大长度</param>
    /// <param name="ellipsis"><see cref="bool"/></param>
    /// <returns></returns>
    public static string GetNVarcharMaxLen(this string str, int maxLen, bool ellipsis = false)
    {
        // NVARCHAR 每个字符占用2个字节
        var maxByteLen = maxLen * 2;
        var byteLen = Encoding.Unicode.GetBytes(str)
            .Length;

        if (byteLen <= maxLen)
        {
            // 长度符合
            return str;
        }

        // 判断是否需要省略号
        int maxCharLen;
        if (ellipsis)
        {
            // 考虑省略号的字节长度为6
            maxCharLen = (maxByteLen - 6) / 2;
        }
        else
        {
            maxCharLen = maxByteLen;
        }

        return str.GetSubStringWithEllipsis(maxCharLen, ellipsis);
    }
}