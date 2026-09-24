// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

// ReSharper disable once CheckNamespace

namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// 验证 <see cref="float"/> 类型属性必填
/// </summary>
[SuppressSniffer]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class FloatRequiredAttribute : ValidationAttribute
{
    /// <summary>
    /// 允许零
    /// </summary>
    public bool AllowZero { get; set; } = true;

    /// <summary>
    /// 允许负数
    /// </summary>
    public bool AllowNegative { get; set; } = false;

    /// <inheritdoc />
    public override bool IsValid(object value)
    {
        if (value == null)
        {
            return false;
        }

        if (float.TryParse(value.ToString(), out float valueParse))
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
