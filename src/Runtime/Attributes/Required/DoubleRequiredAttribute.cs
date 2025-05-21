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

// ReSharper disable once CheckNamespace

namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// <see cref="DoubleRequiredAttribute"/> 验证 <see cref="double"/> 类型属性必填
/// </summary>
[SuppressSniffer]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class DoubleRequiredAttribute : ValidationAttribute
{
    /// <summary>
    /// 允许零
    /// </summary>
    public bool AllowZero { get; set; } = true;

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

        if (double.TryParse(value.ToString(), out var valueParse))
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