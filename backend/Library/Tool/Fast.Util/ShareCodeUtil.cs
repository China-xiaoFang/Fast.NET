using System;
using System.Text;

namespace Fast.Util;

/// <summary>
/// 邀请码工具类
/// </summary>
public static class ShareCodeUtil
{
    //自定义进制所用的编码，大小写和数字(初始62位)，但去掉7位相似：O/o/0,I/i/1/l，去掉一个补位：A;最终只留(26+26+10)-(7+1)=54位
    private static readonly char[] BASE =
    {
        '8', 'S', '2', 'H', 'b', 'V', 'c', 'E', 'Z', 'g', 'X', 'h', '9', 'z', 'y', 'C', 'x', '7', 'P', 'p', '5', 'K', 'B',
        'G', 'Q', 'U', 'F', 'R', '4', 'u', 'W', 'n', 'Y', 'D', 'd', 'e', 'f', 'a', '3', 't', 'M', 'q', 'J', 'r', 's', 'L',
        'm', 'T', 'N', 'w', '6', 'v', 'j', 'k',
    };

    //A补位字符，不能与自定义重复
    private static readonly char SUFFIX_CHAR = 'A';

    //进制长度
    private static readonly int BIN_LEN = BASE.Length;

    //生成邀请码最小长度
    private static readonly int CODE_LEN = 6;

    /// <summary>
    /// ID转换为邀请码
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string IdToCode(long id)
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
    public static long CodeToId(string code)
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

        var allStr = UpperCase.Append(",").Append(LowerCase).Append(",").Append(Number).ToString();
        return $"Count({allStr.Length - 2}):{allStr}";
    }
}