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
using System.Security.Cryptography;
using System.Text;

namespace Fast.IaaS;

/// <summary>
/// <see cref="VerificationUtil"/> 验证工具类
/// </summary>
public static class VerificationUtil
{
    /// <summary>
    /// 自定义进制所用的编码，大小写和数字(初始62位)，但去掉7位相似：O/o/0,I/i/1/l，去掉一个补位：A;最终只留(26+26+10)-(7+1)=54位
    /// </summary>
    private static readonly char[] BASE =
    {
        '8', 'S', '2', 'H', 'b', 'V', 'c', 'E', 'Z', 'g', 'X', 'h', '9', 'z', 'y', 'C', 'x', '7', 'P', 'p', '5', 'K', 'B',
        'G', 'Q', 'U', 'F', 'R', '4', 'u', 'W', 'n', 'Y', 'D', 'd', 'e', 'f', 'a', '3', 't', 'M', 'q', 'J', 'r', 's', 'L',
        'm', 'T', 'N', 'w', '6', 'v', 'j', 'k'
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
    /// 生成邀请码最小长度
    /// </summary>
    private const int CODE_LEN = 6;

    /// <summary>
    /// Id转换为邀请码
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string IdToCodeByLong(long id)
    {
        if (id < 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Id 不能为负数。");

        var buf = new char[BASE_LEN];
        var charPos = BASE_LEN;

        //当id除以数组长度结果大于0，则进行取模操作，并以取模的值作为数组的坐标获得对应的字符
        while (id / BASE_LEN > 0)
        {
            var index = (int) (id % BASE_LEN);
            buf[--charPos] = BASE[index];
            id /= BASE_LEN;
        }

        buf[--charPos] = BASE[(int) (id % BASE_LEN)];
        // 将字符数组转化为字符串
        var result = new string(buf, charPos, BASE_LEN - charPos);

        // 长度不足指定长度则随机补全
        var len = result.Length;
        if (len >= CODE_LEN)
            return result;
        var sb = new StringBuilder();
        sb.Append(SUFFIX_CHAR);
        // 去除SUFFIX_CHAR本身占位之后需要补齐的位数
        for (var i = 0; i < CODE_LEN - len - 1; i++)
        {
            sb.Append(BASE[RandomNumberGenerator.GetInt32(BASE_LEN)]);
        }

        result += sb.ToString();

        return result;
    }

    /// <summary>
    /// 邀请码解析出Id,基本操作思路恰好与idToCode反向操作。
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static long CodeToIdByLong(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("邀请码不能为空。", nameof(code));

        var charArray = code.ToCharArray();
        var result = 0L;
        for (var i = 0; i < charArray.Length; i++)
        {
            if (charArray[i] == SUFFIX_CHAR)
            {
                break;
            }

            var index = Array.IndexOf(BASE, charArray[i]);
            if (index < 0)
                throw new FormatException($"邀请码包含无效字符“{charArray[i]}”。");

            result = checked(result * BASE_LEN + index);
        }

        return result;
    }

    /// <summary>
    /// Id转换为邀请码
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string IdToCodeByInt(int id)
    {
        if (id < 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Id 不能为负数。");

        var buf = new char[BASE_LEN];
        var charPos = BASE_LEN;

        //当id除以数组长度结果大于0，则进行取模操作，并以取模的值作为数组的坐标获得对应的字符
        while (id / BASE_LEN > 0)
        {
            var index = id % BASE_LEN;
            buf[--charPos] = BASE[index];
            id /= BASE_LEN;
        }

        buf[--charPos] = BASE[id % BASE_LEN];
        // 将字符数组转化为字符串
        var result = new string(buf, charPos, BASE_LEN - charPos);

        // 长度不足指定长度则随机补全
        var len = result.Length;
        if (len >= CODE_LEN)
            return result;
        var sb = new StringBuilder();
        sb.Append(SUFFIX_CHAR);
        // 去除SUFFIX_CHAR本身占位之后需要补齐的位数
        for (var i = 0; i < CODE_LEN - len - 1; i++)
        {
            sb.Append(BASE[RandomNumberGenerator.GetInt32(BASE_LEN)]);
        }

        result += sb.ToString();

        return result;
    }

    /// <summary>
    /// 邀请码解析出Id,基本操作思路恰好与idToCode反向操作。
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static int CodeToIdByInt(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("邀请码不能为空。", nameof(code));

        var charArray = code.ToCharArray();
        var result = 0;
        for (var i = 0; i < charArray.Length; i++)
        {
            if (charArray[i] == SUFFIX_CHAR)
            {
                break;
            }

            var index = Array.IndexOf(BASE, charArray[i]);
            if (index < 0)
                throw new FormatException($"邀请码包含无效字符“{charArray[i]}”。");

            result = checked(result * BASE_LEN + index);
        }

        return result;
    }

    /// <summary>
    /// 显示用于进制编码的所有字符串
    /// </summary>
    /// <returns></returns>
    public static string PrintBase()
    {
        var upperCase = new StringBuilder();
        var lowerCase = new StringBuilder();
        var number = new StringBuilder();

        // 编码表的顺序参与 Id 编解码，绝不能为了展示而原地排序。
        var sortedBase = (char[]) BASE.Clone();
        Array.Sort(sortedBase);
        foreach (var item in sortedBase)
        {
            int ascii = item;
            if (ascii >= 48 && ascii <= 57)
                number.Append(item);
            else if (ascii >= 65 && ascii <= 90)
                upperCase.Append(item);
            else if (ascii >= 97 && ascii <= 122)
                lowerCase.Append(item);
        }

        var allStr = upperCase.Append(",")
            .Append(lowerCase)
            .Append(",")
            .Append(number)
            .ToString();
        return $"Count({allStr.Length - 2}):{allStr}";
    }

    /// <summary>
    /// 生成数字验证码
    /// </summary>
    /// <param name="len"><see cref="int"/> 验证码长度，默认6位</param>
    /// <returns><see cref="string"/></returns>
    public static string GenNumVerCode(int len = CODE_LEN)
    {
        if (len <= 0)
            throw new ArgumentOutOfRangeException(nameof(len), "验证码长度必须大于 0。");

        var result = new StringBuilder(len);
        result.Append(RandomNumberGenerator.GetInt32(1, 10));
        for (var i = 1; i < len; i++)
        {
            result.Append(RandomNumberGenerator.GetInt32(10));
        }

        return result.ToString();
    }

    /// <summary>
    /// 生成字符串验证码
    /// </summary>
    /// <param name="len"><see cref="int"/> 验证码长度，默认6位</param>
    /// <returns><see cref="string"/></returns>
    public static string GenStrVerCode(int len = CODE_LEN)
    {
        if (len <= 0)
            throw new ArgumentOutOfRangeException(nameof(len), "验证码长度必须大于 0。");

        var result = new StringBuilder(len);

        for (var i = 0; i < len; i++)
        {
            var randomInt = RandomNumberGenerator.GetInt32(BASE_LEN);
            var randomChar = BASE[randomInt];
            result.Append(randomChar);
        }

        return result.ToString();
    }

    /// <summary>
    /// 生成随机数
    /// </summary>
    /// <param name="minVal">最小值（包含）</param>
    /// <param name="maxVal">最大值（默认不包含）</param>
    /// <param name="isInclude">是否包含最大值</param>
    /// <returns></returns>
    public static int GenRandomNum(int minVal, int maxVal, bool isInclude = false)
    {
        if (isInclude ? maxVal < minVal : maxVal <= minVal)
            throw new ArgumentOutOfRangeException(nameof(maxVal), isInclude ? "最大值不能小于最小值。" : "最大值必须大于最小值。");

        return GetRandomInt32(minVal, isInclude ? maxVal : maxVal - 1);
    }

    /// <summary>
    /// 生成包含上下边界的安全随机整数。
    /// </summary>
    private static int GetRandomInt32(int minValue, int maxValue)
    {
        var range = (ulong) ((long) maxValue - minValue + 1);
        const ulong sampleSpace = 1UL << 32;
        var limit = sampleSpace - sampleSpace % range;

        // 舍弃不能被区间长度整除的尾部样本，避免取模造成某些数字概率偏高。
        Span<byte> bytes = stackalloc byte[sizeof(uint)];
        uint sample;
        do
        {
            RandomNumberGenerator.Fill(bytes);
            sample = BitConverter.ToUInt32(bytes);
        } while (sample >= limit);

        return (int) (minValue + (long) (sample % range));
    }
}