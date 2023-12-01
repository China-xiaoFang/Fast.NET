// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
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

using System.Text;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="Base64Extension"/> Base64 拓展类
/// </summary>
[SuppressSniffer]
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
            if (string.IsNullOrWhiteSpace(base64Str?.Trim()))
            {
                return "";
            }

            base64Str = base64Str?.Trim();
            var input = base64Str?[randomPrefixStrLength..];

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
        public string Version { get; set; }

        public List<PwdDicItem> Item { get; init; }
    }

    private readonly struct PwdDicItem
    {
        public int Index { get; init; }

        public int RandomIndex { get; init; }
    }

    private static readonly PwdDic dic = new()
    {
        Version = "3",
        Item = new List<PwdDicItem>
        {
            new() {Index = 950, RandomIndex = 188},
            new() {Index = 900, RandomIndex = 201},
            new() {Index = 800, RandomIndex = 225},
            new() {Index = 700, RandomIndex = 255},
            new() {Index = 600, RandomIndex = 268},
            new() {Index = 500, RandomIndex = 277},
            new() {Index = 400, RandomIndex = 288},
            new() {Index = 330, RandomIndex = 327},
            new() {Index = 300, RandomIndex = 180},
            new() {Index = 200, RandomIndex = 178},
            new() {Index = 100, RandomIndex = 124},
            // 100 以内字典
            new() {Index = 98, RandomIndex = 95},
            new() {Index = 92, RandomIndex = 90},
            new() {Index = 91, RandomIndex = 87},
            new() {Index = 88, RandomIndex = 84},
            new() {Index = 82, RandomIndex = 79},
            new() {Index = 78, RandomIndex = 71},
            new() {Index = 72, RandomIndex = 69},
            new() {Index = 68, RandomIndex = 66},
            new() {Index = 59, RandomIndex = 55},
            new() {Index = 48, RandomIndex = 43},
            new() {Index = 42, RandomIndex = 37},
            new() {Index = 36, RandomIndex = 30},
            new() {Index = 33, RandomIndex = 27},
            new() {Index = 24, RandomIndex = 20},
            new() {Index = 23, RandomIndex = 18},
            new() {Index = 21, RandomIndex = 16},
            new() {Index = 17, RandomIndex = 14},
            new() {Index = 13, RandomIndex = 9},
            new() {Index = 7, RandomIndex = 4},
            new() {Index = 5, RandomIndex = 3},
            new() {Index = 2, RandomIndex = 1},
        }
    };
}