// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System;
using System.Reflection;

namespace Fast.IaaS;

/// <summary>
/// 为 <see cref="FieldInfo"/> 提供扩展方法
/// </summary>
public static class FieldInfoExtension
{
    /// <summary>
    /// 获取字段特性
    /// </summary>
    /// <param name="field">目标字段</param>
    /// <typeparam name="T">要读取的特性类型</typeparam>
    /// <returns>获取到的字段特性</returns>
    public static T GetDescriptionValue<T>(this FieldInfo field) where T : Attribute
    {
        // 获取字段的指定特性，不包含继承中的特性
        object[] customAttributes = field.GetCustomAttributes(typeof(T), false);

        // 如果没有数据返回 null
        return customAttributes.Length > 0 ? (T)customAttributes[0] : null;
    }
}
