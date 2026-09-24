// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System.Collections.Generic;

namespace Fast.IaaS;

/// <summary>
/// 序号工具类
/// </summary>
public static class NumberUtil
{
    /// <summary>
    /// 自定义进制所用的编码，大写和数字(初始 36 位)，但去掉 3 位相似：O,0,I，去掉一个补位：A;最终只留(26+10)-(3+1)=32 位
    /// </summary>
    private static readonly char[] BASE =
    {
        '8', 'R', 'T', 'G', 'V', '3', 'S', 'Y', '9', 'C', 'X', 'U', 'J', 'L', '7', 'Q', '2', 'H', 'M', '4', 'Z', '5', 'D',
        'P', 'B', '6', 'N', 'W', '1', 'F', 'K'
    };

    /// <summary>
    /// A 补位字符，不能与自定义重复
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
    /// 将Id转换为 Base32 Crockford 字符串
    /// </summary>
    /// <param name="id">要编码的Id</param>
    /// <param name="maxLength">目标长度</param>
    /// <returns>转换后的 Base32 Crockford 字符串</returns>
    public static string IdToCodeByLong(long id, int maxLength = 12)
    {
        var chars = new List<char>();
        long value = id;

        // 进行 Base32 Crockford 转换
        do
        {
            int index = (int)(value % BASE_LEN);
            chars.Add(BASE[index]);
            value /= BASE_LEN;
        } while (value > 0);

        // 倒序排列，得到最终编码
        chars.Reverse();
        string result = new string(chars.ToArray()).ToUpperInvariant();

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
