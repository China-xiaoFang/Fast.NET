// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
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
/// <see cref="IntRequiredAttribute"/> 验证 <see cref="int"/> 类型属性必填
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class IntRequiredAttribute : ValidationAttribute
{
    /// <summary>
    /// 允许零
    /// </summary>
    public bool AllowZero { get; set; } = false;

    /// <summary>
    /// 允许负数
    /// </summary>
    public bool AllowNegative { get; set; } = false;

    /// <summary>Determines whether the specified value of the object is valid.</summary>
    /// <param name="value">The value of the object to validate.</param>
    /// <exception cref="T:System.InvalidOperationException">The current attribute is malformed.</exception>
    /// <exception cref="T:System.NotImplementedException">Neither overload of <see langword="IsValid" /> has been implemented by a derived class.</exception>
    /// <returns>
    /// <see langword="true" /> if the specified value is valid; otherwise, <see langword="false" />.</returns>
    public override bool IsValid(object value)
    {
        // 判断是否为空
        if (value == null)
        {
            return false;
        }

        if (int.TryParse(value.ToString(), out var valueParse))
        {
            // 允许零，负数
            if (AllowZero && AllowNegative)
            {
                return true;
            }

            // 不允许零，不允许负数
            if (!AllowZero && !AllowNegative)
            {
                return valueParse > 0;
            }

            // 允许零，不允许负数
            if (AllowZero && !AllowNegative)
            {
                return valueParse >= 0;
            }

            // 不允许零，允许负数
            if (!AllowZero && AllowNegative)
            {
                return valueParse is > 0 or < 0;
            }
        }

        return false;
    }
}