// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System.Text;

namespace Fast.DynamicApplication;

/// <summary>
/// 为 <see cref="string"/> 提供扩展方法
/// </summary>
internal static class StringExtension
{
    /// <summary>
    /// 字符串首字母小写
    /// </summary>
    /// <param name="str">要转换的字符串</param>
    /// <returns>首字母小写后的字符串；输入为空时返回原值</returns>
    public static string FirstCharToLower(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        var stringBuilder = new StringBuilder(str);

        stringBuilder[0] = char.ToLowerInvariant(stringBuilder[0]);

        return stringBuilder.ToString();
    }
}
