// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using System.Linq.Expressions;


// ReSharper disable once CheckNamespace
namespace System.Linq;

/// <summary>
/// 提供 Linq Expression 相关扩展方法
/// </summary>
[SuppressSniffer]
public static class LinqExpression
{
    /// <summary>
    /// 创建 Linq/Lambda 表达式
    /// </summary>
    /// <param name="expression">要组合或执行的表达式</param>
    /// <typeparam name="TSource">源对象类型</typeparam>
    /// <returns>创建的 Linq/Lambda 表达式</returns>
    public static Expression<Func<TSource, bool>> Create<TSource>(Expression<Func<TSource, bool>> expression)
    {
        return expression;
    }

    /// <summary>
    /// 创建 Linq/Lambda 表达式，支持索引器
    /// </summary>
    /// <param name="expression">要组合或执行的表达式</param>
    /// <typeparam name="TSource">源对象类型</typeparam>
    /// <returns>创建的 Linq/Lambda 表达式，支持索引器</returns>
    public static Expression<Func<TSource, int, bool>> Create<TSource>(Expression<Func<TSource, int, bool>> expression)
    {
        return expression;
    }

    /// <summary>
    /// 创建 And 表达式
    /// </summary>
    /// <typeparam name="TSource">源对象类型</typeparam>
    /// <returns>创建的 And 表达式</returns>
    public static Expression<Func<TSource, bool>> And<TSource>()
    {
        return u => true;
    }

    /// <summary>
    /// 创建 And 表达式，支持索引器
    /// </summary>
    /// <typeparam name="TSource">源对象类型</typeparam>
    /// <returns>创建的 And 表达式，支持索引器</returns>
    public static Expression<Func<TSource, int, bool>> IndexAnd<TSource>()
    {
        return (u, i) => true;
    }

    /// <summary>
    /// 创建 Or 表达式
    /// </summary>
    /// <typeparam name="TSource">源对象类型</typeparam>
    /// <returns>创建的 Or 表达式</returns>
    public static Expression<Func<TSource, bool>> Or<TSource>()
    {
        return u => false;
    }

    /// <summary>
    /// 创建 Or 表达式，支持索引器
    /// </summary>
    /// <typeparam name="TSource">源对象类型</typeparam>
    /// <returns>创建的 Or 表达式，支持索引器</returns>
    public static Expression<Func<TSource, int, bool>> IndexOr<TSource>()
    {
        return (u, i) => false;
    }
}
