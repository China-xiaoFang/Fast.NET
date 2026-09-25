// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

// ReSharper disable once CheckNamespace

namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// 验证 <see cref="DateTime"/> 类型属性必填
/// </summary>
/// <remarks>默认必须在 1949-10-01 ~ 2099-12-31 之间</remarks>
[SuppressSniffer]
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

    /// <inheritdoc />
    public override bool IsValid(object value)
    {
        if (value == null)
        {
            return false;
        }

        if (DateTime.TryParse(value.ToString(), out DateTime valueParse))
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

            return true;
        }

        return false;
    }
}
