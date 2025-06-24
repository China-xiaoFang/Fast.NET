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

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="Base64Extension"/> Base64 拓展类
/// </summary>
public static class Base64Extension
{
    /// <summary>
    /// 随机字符长度
    /// </summary>
    public const int RandomPrefixStrLength = 6;

    private static readonly Encoding encoding = Encoding.UTF8;

    /// <summary>
    /// 普通 字符串 转换为 Base64 字符串
    /// </summary>
    /// <param name="str"><see cref="string"/> 字符串</param>
    /// <param name="randomPrefixStrLength"><see cref="int"/> 随机字符长度，默认6位</param>
    /// <returns><see cref="string"/> 转换后的 Base64 字符串</returns>
    public static string ToBase64(this string str, int randomPrefixStrLength = RandomPrefixStrLength)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return "";
        }

        try
        {
            var randomPrefixStr = VerificationUtil.GenStrVerCode(randomPrefixStrLength);
            var buffer = encoding.GetBytes(str);
            var base64Str = Convert.ToBase64String(buffer);

            base64Str = randomPrefixStrLength == 0 ? base64Str : InsertRandomStrToBase64Str(base64Str);

            return $"{randomPrefixStr}{base64Str}";
        }
        catch (Exception ex)
        {
            var logSb = new StringBuilder();
            logSb.Append("\u001b[41m\u001b[30m");
            logSb.Append("fail");
            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            logSb.Append(": ");
            logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
            logSb.Append(Environment.NewLine);
            logSb.Append("\u001b[41m\u001b[30m");
            logSb.Append("      ");
            logSb.Append($"Base64Util.ToBase64: {ex}");
            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            Console.WriteLine(logSb.ToString());
        }

        return string.Empty;
    }

    /// <summary>
    /// Base64 字符串 转换为 普通 字符串
    /// </summary>
    /// <param name="base64Str"><see cref="string"/> Base64 字符串</param>
    /// <param name="randomPrefixStrLength"><see cref="int"/> 随机字符长度，默认6位</param>
    /// <returns><see cref="string"/> 转换后的 字符串</returns>
    public static string Base64ToString(this string base64Str, int randomPrefixStrLength = RandomPrefixStrLength)
    {
        var result = base64Str.Trim();
        try
        {
            if (string.IsNullOrWhiteSpace(base64Str.Trim()))
            {
                return "";
            }

            base64Str = base64Str.Trim();
            var input = base64Str[randomPrefixStrLength..];

            input = randomPrefixStrLength == 0 ? input : RemoveBase64StrRandomStr(input);
            var buffer = Convert.FromBase64String(input);
            result = encoding.GetString(buffer);
        }
        catch (Exception ex)
        {
            var logSb = new StringBuilder();
            logSb.Append("\u001b[41m\u001b[30m");
            logSb.Append("fail");
            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            logSb.Append(": ");
            logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
            logSb.Append(Environment.NewLine);
            logSb.Append("\u001b[41m\u001b[30m");
            logSb.Append("      ");
            logSb.Append($"Base64Util.Base64ToString: {ex}");
            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            Console.WriteLine(logSb.ToString());
        }

        return result;
    }

    /// <summary>
    /// 添加随机字符串到 Base64 字符串
    /// </summary>
    /// <param name="base64Str"><see cref="string"/> Base64 字符串</param>
    /// <returns></returns>
    private static string InsertRandomStrToBase64Str(string base64Str)
    {
        var strResult = $"{base64Str}";

        dic.Item.ForEach(item =>
        {
            if (item.Index < base64Str.Length)
            {
                var randomChar = base64Str[item.RandomIndex];
                strResult = strResult.Insert(item.Index, $"{randomChar}");
            }
        });

        return strResult;
    }

    /// <summary>
    /// 删除 Base64 字符串中的随机数
    /// </summary>
    /// <param name="input"><see cref="string"/> Base64 字符串</param>
    /// <returns></returns>
    private static string RemoveBase64StrRandomStr(string input)
    {
        var items = dic.Item.OrderBy(x => x.Index)
            .ToList();

        var strResult = $"{input}";

        items.ForEach(item =>
        {
            if (item.Index < strResult.Length)
            {
                //var randomChar = input[item.RandomIndex];
                strResult = strResult.Remove(item.Index, 1);
            }
        });

        return strResult;
    }

    private struct PwdDic
    {
        public List<PwdDicItem> Item { get; }

        public PwdDic(List<PwdDicItem> item)
        {
            Item = item;
        }
    }

    private readonly struct PwdDicItem
    {
        public int Index { get; }

        public int RandomIndex { get; }

        public PwdDicItem(int index, int randomIndex)
        {
            Index = index;
            RandomIndex = randomIndex;
        }
    }

    private static readonly PwdDic dic = new(new List<PwdDicItem>
    {
        new(950, 188),
        new(900, 201),
        new(800, 225),
        new(700, 255),
        new(600, 268),
        new(500, 277),
        new(400, 288),
        new(330, 327),
        new(300, 180),
        new(200, 178),
        new(100, 124),
        // 100 以内字典
        new(98, 95),
        new(92, 90),
        new(91, 87),
        new(88, 84),
        new(82, 79),
        new(78, 71),
        new(72, 69),
        new(68, 66),
        new(59, 55),
        new(48, 43),
        new(42, 37),
        new(36, 30),
        new(33, 27),
        new(24, 20),
        new(23, 18),
        new(21, 16),
        new(17, 14),
        new(13, 9),
        new(7, 4),
        new(5, 3),
        new(2, 1)
    });
}