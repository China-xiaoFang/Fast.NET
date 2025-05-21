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
namespace Fast.IaaS
{
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
                Console.WriteLine($"Base64Util.ToBase64: {ex}");
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
                Console.WriteLine($"Base64Util.Base64ToString: {ex}");
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
            var items = dic.Item.OrderBy(x => x.Index).ToList();

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

        private static readonly PwdDic dic = new PwdDic(new List<PwdDicItem>
        {
            new PwdDicItem(950, 188),
            new PwdDicItem(900, 201),
            new PwdDicItem(800, 225),
            new PwdDicItem(700, 255),
            new PwdDicItem(600, 268),
            new PwdDicItem(500, 277),
            new PwdDicItem(400, 288),
            new PwdDicItem(330, 327),
            new PwdDicItem(300, 180),
            new PwdDicItem(200, 178),
            new PwdDicItem(100, 124),
            // 100 以内字典
            new PwdDicItem(98, 95),
            new PwdDicItem(92, 90),
            new PwdDicItem(91, 87),
            new PwdDicItem(88, 84),
            new PwdDicItem(82, 79),
            new PwdDicItem(78, 71),
            new PwdDicItem(72, 69),
            new PwdDicItem(68, 66),
            new PwdDicItem(59, 55),
            new PwdDicItem(48, 43),
            new PwdDicItem(42, 37),
            new PwdDicItem(36, 30),
            new PwdDicItem(33, 27),
            new PwdDicItem(24, 20),
            new PwdDicItem(23, 18),
            new PwdDicItem(21, 16),
            new PwdDicItem(17, 14),
            new PwdDicItem(13, 9),
            new PwdDicItem(7, 4),
            new PwdDicItem(5, 3),
            new PwdDicItem(2, 1)
        });
    }
}