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

// ReSharper disable once CheckNamespace

namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// <see cref="StringRequiredAttribute"/> 验证 <see cref="string"/> 类型属性必填
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class StringRequiredAttribute : ValidationAttribute
{
    /// <summary>
    /// 允许空字符串
    /// </summary>
    public bool AllowEmptyString { get; set; } = false;

    /// <summary>
    /// 允许前后空格
    /// </summary>
    public bool AllowWhitespace { get; set; } = false;

    /// <summary>Determines whether the specified value of the object is valid.</summary>
    /// <param name="value">The value of the object to validate.</param>
    /// <exception cref="T:System.InvalidOperationException">The current attribute is malformed.</exception>
    /// <exception cref="T:System.NotImplementedException">Neither overload of <see langword="IsValid" /> has been implemented by a derived class.</exception>
    /// <returns>
    /// <see langword="true" /> if the specified value is valid; otherwise, <see langword="false" />.</returns>
    public override bool IsValid(object value)
    {
        var valueParse = value?.ToString();

        // 判断是否为空
        if (valueParse == null)
        {
            return false;
        }

        // 不允许前后空格
        if (!AllowWhitespace)
        {
            var sourceLength = valueParse.Length;
            valueParse = valueParse.TrimStart().TrimEnd();
            if (valueParse.Length != sourceLength)
            {
                throw new ValidationException(
                    $"The \"{ErrorMessageResourceName}\" value cannot contain blank strings before and after it.");
            }
        }

        // 不允许空字符串
        if (!AllowEmptyString)
        {
            if (string.IsNullOrEmpty(valueParse))
            {
                return false;
            }
        }

        return true;
    }
}