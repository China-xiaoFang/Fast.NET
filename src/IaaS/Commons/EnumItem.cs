// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System;

namespace Fast.IaaS;

/// <summary>
/// 枚举项
/// </summary>
/// <typeparam name="TProperty">Value 属性类型</typeparam>
public class EnumItem<TProperty> where TProperty : struct, IComparable, IConvertible, IFormattable
{
    /// <summary>
    /// 枚举的描述
    /// </summary>
    public string Describe { set; get; }

    /// <summary>
    /// 枚举名称
    /// </summary>
    public string Name { set; get; }

    /// <summary>
    /// 枚举对象的值
    /// </summary>
    public TProperty Value { set; get; }
}
