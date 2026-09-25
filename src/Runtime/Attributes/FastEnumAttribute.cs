// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

// ReSharper disable once CheckNamespace

namespace System;

/// <summary>
/// 枚举特性
/// </summary>
/// <remarks>用于区分是否可以写入枚举字典的特性</remarks>
[SuppressSniffer]
[AttributeUsage(AttributeTargets.Enum)]
public class FastEnumAttribute : Attribute
{
    /// <summary>
    /// 中文名称
    /// </summary>
    public string ChName { get; set; }

    /// <summary>
    /// 英文名称
    /// </summary>
    public string EnName { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 枚举特性
    /// </summary>
    /// <remarks>用于区分是否可以写入枚举字典的特性</remarks>
    public FastEnumAttribute()
    {
    }

    /// <summary>
    /// 枚举特性
    /// </summary>
    /// <remarks>用于区分是否可以写入枚举字典的特性</remarks>
    /// <param name="chName">中文显示名称</param>
    /// <param name="enName">英文显示名称</param>
    /// <param name="remark">补充说明</param>
    public FastEnumAttribute(string chName, string enName, string remark)
    {
        ChName = chName;
        EnName = enName;
        Remark = remark;
    }

    /// <summary>
    /// 枚举特性
    /// </summary>
    /// <remarks>用于区分是否可以写入枚举字典的特性</remarks>
    /// <param name="chName">中文显示名称</param>
    /// <param name="enName">英文显示名称</param>
    public FastEnumAttribute(string chName, string enName)
    {
        ChName = chName;
        EnName = enName;
    }

    /// <summary>
    /// 枚举特性
    /// </summary>
    /// <remarks>用于区分是否可以写入枚举字典的特性</remarks>
    /// <param name="chName">中文显示名称</param>
    public FastEnumAttribute(string chName)
    {
        ChName = chName;
    }
}
