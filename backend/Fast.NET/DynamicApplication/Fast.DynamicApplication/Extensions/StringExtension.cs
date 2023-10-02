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
using System.Text.RegularExpressions;

namespace Fast.DynamicApplication.Extensions;

/// <summary>
/// <see cref="string"/> 拓展类
/// </summary>
internal static class StringExtension
{
    /// <summary>
    /// 切割骆驼命名式字符串
    /// </summary>
    /// <param name="str"><see cref="string"/></param>
    /// <returns><see cref="string"/></returns>
    internal static string[] SplitCamelCase(this string str)
    {
        if (str == null)
            return Array.Empty<string>();

        if (string.IsNullOrWhiteSpace(str))
            return new[] {str};
        if (str.Length == 1)
            return new[] {str};

        return Regex.Split(str, @"(?=\p{Lu}\p{Ll})|(?<=\p{Ll})(?=\p{Lu})").Where(u => u.Length > 0).ToArray();
    }

    /// <summary>
    /// 字符串首字母小写
    /// </summary>
    /// <param name="str"><see cref="string"/></param>
    /// <returns><see cref="string"/></returns>
    internal static string FirstCharToLower(this string str)
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
}