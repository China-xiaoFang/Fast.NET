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

using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="NumberUtil"/> 序号工具类
/// </summary>
public static class NumberUtil
{
    /// <summary>
    /// 自定义进制所用的编码，大写和数字(初始36位)，但去掉3位相似：O,0,I，去掉一个补位：A;最终只留(26+10)-(3+1)=32位
    /// </summary>
    private static readonly char[] BASE =
    {
        '8', 'R', 'T', 'G', 'V', '3', 'S', 'Y', '9', 'C', 'X', 'U', 'J', 'L', '7', 'Q', '2', 'H', 'M', '4', 'Z', '5', 'D',
        'P', 'B', '6', 'N', 'W', '1', 'F', 'K'
    };

    /// <summary>
    /// A补位字符，不能与自定义重复
    /// </summary>
    private static readonly char SUFFIX_CHAR = 'A';

    /// <summary>
    /// 进制长度
    /// </summary>
    private static readonly int BASE_LEN = BASE.Length;

    /// <summary>
    /// 最小长度
    /// </summary>
    private const int MIN_LEN = 6;

    /// <summary>
    /// Id转为 Base32 Crockford 字符串
    /// </summary>
    /// <param name="id"></param>
    /// <param name="maxLength"><see cref="int"/> 最长长度，默认12</param>
    /// <returns></returns>
    public static string IdToCodeByLong(long id, int maxLength = 12)
    {
        var chars = new List<char>();
        var value = id;

        // 进行 Base32 Crockford 转换
        do
        {
            var index = (int) (value % BASE_LEN);
            chars.Add(BASE[index]);
            value /= BASE_LEN;
        } while (value > 0);

        // 倒序排列，得到最终编码
        chars.Reverse();
        var result = new string(chars.ToArray()).ToUpperInvariant();

        // 补齐最小长度
        if (result.Length < MIN_LEN)
        {
            result = new string(SUFFIX_CHAR, MIN_LEN - result.Length) + result;
        }

        // 超过最大长度直接截取
        if (result.Length > maxLength)
        {
            result = result[..maxLength];
        }

        return result;
    }
}