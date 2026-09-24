// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System;
using System.Linq.Expressions;

namespace Fast.IaaS;

/// <summary>
/// 为 <see cref="Expression"/> 提供扩展方法
/// </summary>
public static class LinqExpressionExtension
{
    /// <summary>
    /// 解析表达式属性名称
    /// </summary>
    /// <exception cref="ArgumentException">Expression is not valid for property selection</exception>
    /// <param name="propertySelector">用于选择目标属性的表达式</param>
    /// <typeparam name="T">表达式参数所表示的对象类型</typeparam>
    /// <typeparam name="TProperty">属性值类型</typeparam>
    /// <returns>解析后的表达式属性名称</returns>
    public static string GetPropertyName<T, TProperty>(this Expression<Func<T, TProperty>> propertySelector)
    {
        return propertySelector.Body switch
        {
            // 检查 Lambda 表达式的主体是否是 MemberExpression 类型
            MemberExpression memberExpression => GetPropertyName<T>(memberExpression),

            // 如果主体是 UnaryExpression 类型，则继续解析
            UnaryExpression {Operand: MemberExpression nestedMemberExpression} => GetPropertyName<T>(nestedMemberExpression),

            _ => throw new ArgumentException("Expression is not valid for property selection.")
        };
    }

    /// <summary>
    /// 解析表达式属性名称
    /// </summary>
    /// <exception cref="ArgumentException">Invalid property selection</exception>
    /// <param name="memberExpression">用于解析表达式属性名称的表达式</param>
    /// <typeparam name="T">表达式参数所表示的对象类型</typeparam>
    /// <returns>解析后的表达式属性名称</returns>
    public static string GetPropertyName<T>(MemberExpression memberExpression)
    {
        if (memberExpression is null)
        {
            throw new ArgumentNullException(nameof(memberExpression));
        }

        // 获取属性声明类型
        Type propertyType = memberExpression.Member.DeclaringType;

        // 检查是否越界访问属性
        if (propertyType != typeof(T))
        {
            throw new ArgumentException("Invalid property selection.");
        }

        // 返回属性名称
        return memberExpression.Member.Name;
    }
}
