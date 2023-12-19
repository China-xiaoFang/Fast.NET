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

using System.ComponentModel.DataAnnotations;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="DateTimeRequiredAttribute"/> 验证 <see cref="DateTime"/> 类型属性必填
/// <remarks>默认必须在 1949-10-01 ~ 2099-12-31 之间</remarks>
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class DateTimeRequiredAttribute : ValidationAttribute
{
    /// <summary>
    /// 最小值
    /// </summary>
    public DateTime MinValue { get; set; } = new(1949, 10, 01, 00, 00, 00);

    /// <summary>
    /// 最大值
    /// </summary>
    public DateTime MaxValue { get; set; } = new(2099, 12, 31, 23, 59, 59);

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

        if (DateTime.TryParse(value.ToString(), out var valueParse))
        {
            // 默认值
            if (valueParse == default)
            {
                return false;
            }

            // 最小值
            if (valueParse < MinValue)
            {
                return false;
            }

            // 最大值
            if (valueParse > MaxValue)
            {
                return false;
            }
        }

        return false;
    }
}