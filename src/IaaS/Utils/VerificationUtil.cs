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
using System.Text;
using System.Threading;

// ReSharper disable once CheckNamespace
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
    private static readonly int BIN_LEN = BASE.Length;

    /// <summary>
    /// 生成邀请码最小长度
    /// </summary>
    private const int CODE_LEN = 6;

    /// <summary>
    /// ID转换为邀请码
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string IdToCodeByLong(long id)
    {
        var buf = new char[BIN_LEN];
        var charPos = BIN_LEN;

        //当id除以数组长度结果大于0，则进行取模操作，并以取模的值作为数组的坐标获得对应的字符
        while (id / BIN_LEN > 0)
        {
            var index = (int) (id % BIN_LEN);
            buf[--charPos] = BASE[index];
            id /= BIN_LEN;
        }

        buf[--charPos] = BASE[(int) (id % BIN_LEN)];
        // 将字符数组转化为字符串
        var result = new string(buf, charPos, BIN_LEN - charPos);

        // 长度不足指定长度则随机补全
        var len = result.Length;
        if (len >= CODE_LEN)
            return result;
        var sb = new StringBuilder();
        sb.Append(SUFFIX_CHAR);
        var random = new Random();
        // 去除SUFFIX_CHAR本身占位之后需要补齐的位数
        for (var i = 0; i < CODE_LEN - len - 1; i++)
        {
            sb.Append(BASE[random.Next(BIN_LEN)]);
        }

        result += sb.ToString();

        return result;
    }

    /// <summary>
    /// 邀请码解析出ID,基本操作思路恰好与idToCode反向操作。
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static long CodeToIdByLong(string code)
    {
        var charArray = code.ToCharArray();
        var result = 0L;
        for (var i = 0; i < charArray.Length; i++)
        {
            var index = 0;
            for (var j = 0; j < BIN_LEN; j++)
            {
                if (charArray[i] != BASE[j])
                    continue;
                index = j;
                break;
            }

            if (charArray[i] == SUFFIX_CHAR)
            {
                break;
            }

            if (i > 0)
            {
                result = result * BIN_LEN + index;
            }
            else
            {
                result = index;
            }
        }

        return result;
    }

    /// <summary>
    /// ID转换为邀请码
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string IdToCodeByInt(int id)
    {
        var buf = new char[BIN_LEN];
        var charPos = BIN_LEN;

        //当id除以数组长度结果大于0，则进行取模操作，并以取模的值作为数组的坐标获得对应的字符
        while (id / BIN_LEN > 0)
        {
            var index = id % BIN_LEN;
            buf[--charPos] = BASE[index];
            id /= BIN_LEN;
        }

        buf[--charPos] = BASE[id % BIN_LEN];
        // 将字符数组转化为字符串
        var result = new string(buf, charPos, BIN_LEN - charPos);

        // 长度不足指定长度则随机补全
        var len = result.Length;
        if (len >= CODE_LEN)
            return result;
        var sb = new StringBuilder();
        sb.Append(SUFFIX_CHAR);
        var random = new Random();
        // 去除SUFFIX_CHAR本身占位之后需要补齐的位数
        for (var i = 0; i < CODE_LEN - len - 1; i++)
        {
            sb.Append(BASE[random.Next(BIN_LEN)]);
        }

        result += sb.ToString();

        return result;
    }

    /// <summary>
    /// 邀请码解析出ID,基本操作思路恰好与idToCode反向操作。
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static int CodeToIdByInt(string code)
    {
        var charArray = code.ToCharArray();
        var result = 0;
        for (var i = 0; i < charArray.Length; i++)
        {
            var index = 0;
            for (var j = 0; j < BIN_LEN; j++)
            {
                if (charArray[i] != BASE[j])
                    continue;
                index = j;
                break;
            }

            if (charArray[i] == SUFFIX_CHAR)
            {
                break;
            }

            if (i > 0)
            {
                result = result * BIN_LEN + index;
            }
            else
            {
                result = index;
            }
        }

        return result;
    }

    /// <summary>
    /// 显示用于进制编码的所有字符串
    /// </summary>
    /// <returns></returns>
    public static string PrintBase()
    {
        var UpperCase = new StringBuilder();
        var LowerCase = new StringBuilder();
        var Number = new StringBuilder();

        Array.Sort(BASE);
        foreach (var item in BASE)
        {
            int ascii = item;
            if (ascii >= 48 && ascii <= 57)
                Number.Append(item);
            else if (ascii >= 65 && ascii <= 90)
                UpperCase.Append(item);
            else if (ascii >= 97 && ascii <= 122)
                LowerCase.Append(item);
        }

        var allStr = UpperCase.Append(",")
            .Append(LowerCase)
            .Append(",")
            .Append(Number)
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
        return $"{new Random().Next((int) Math.Pow(10, len - 1), (int) Math.Pow(10, len) - 1)}";
    }

    /// <summary>
    /// 生成字符串验证码
    /// </summary>
    /// <param name="len"><see cref="int"/> 验证码长度，默认6位</param>
    /// <returns><see cref="string"/></returns>
    public static string GenStrVerCode(int len = CODE_LEN)
    {
        var result = "";
        var random = new Random(Convert.ToInt32($"{DateTime.Now:HHmmssfff}"));

        for (var i = 0; i < len; i++)
        {
            var randomInt = random.Next(0, BIN_LEN);
            var randomChar = BASE[randomInt];
            result += randomChar;
        }

        // 休眠,以使随机数不重叠.
        Thread.Sleep(1);

        return result;
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
        if (isInclude)
        {
            maxVal--;
        }

        if (maxVal < minVal)
        {
            throw new Exception("最大值不能小于最小值");
        }

        var random = new Random();
        return random.Next(minVal, isInclude ? maxVal - 1 : maxVal);
    }
}