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

using System.Linq.Expressions;
using Fast.IaaS;

// ReSharper disable once CheckNamespace
namespace System.Linq;

/// <summary>
/// <see cref="IEnumerableExtension"/> IEnumerable 拓展
/// </summary>
[SuppressSniffer]
public static class IEnumerableExtension
{
    /// <summary>
    /// 根据条件成立再构建 Where 查询
    /// </summary>
    /// <typeparam name="TSource">泛型类型</typeparam>
    /// <param name="sources">集合对象</param>
    /// <param name="condition">布尔条件</param>
    /// <param name="expression">表达式</param>
    /// <returns>新的集合对象</returns>
    public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> sources, bool condition,
        Expression<Func<TSource, bool>> expression)
    {
        return condition ? sources.Where(expression) : sources;
    }

    /// <summary>
    /// 根据条件成立再构建 Where 查询，支持索引器
    /// </summary>
    /// <typeparam name="TSource">泛型类型</typeparam>
    /// <param name="sources">集合对象</param>
    /// <param name="condition">布尔条件</param>
    /// <param name="expression">表达式</param>
    /// <returns>新的集合对象</returns>
    public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> sources, bool condition,
        Expression<Func<TSource, int, bool>> expression)
    {
        return condition ? sources.Where(expression) : sources;
    }

    /// <summary>
    /// 与操作合并多个表达式
    /// </summary>
    /// <typeparam name="TSource">泛型类型</typeparam>
    /// <param name="sources">集合对象</param>
    /// <param name="expressions">表达式数组</param>
    /// <returns>新的集合对象</returns>
    public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> sources,
        params Expression<Func<TSource, bool>>[] expressions)
    {
        if (expressions == null || !expressions.Any())
            return sources;
        if (expressions.Length == 1)
            return sources.Where(expressions[0]);

        var expression = LinqExpression.Or<TSource>();
        foreach (var _expression in expressions)
        {
            expression = expression.Or(_expression);
        }

        return sources.Where(expression);
    }

    /// <summary>
    /// 与操作合并多个表达式，支持索引器
    /// </summary>
    /// <typeparam name="TSource">泛型类型</typeparam>
    /// <param name="sources">集合对象</param>
    /// <param name="expressions">表达式数组</param>
    /// <returns>新的集合对象</returns>
    public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> sources,
        params Expression<Func<TSource, int, bool>>[] expressions)
    {
        if (expressions == null || !expressions.Any())
            return sources;
        if (expressions.Length == 1)
            return sources.Where(expressions[0]);

        var expression = LinqExpression.IndexOr<TSource>();
        foreach (var _expression in expressions)
        {
            expression = expression.Or(_expression);
        }

        return sources.Where(expression);
    }

    /// <summary>
    /// 根据条件成立再构建 WhereOr 查询
    /// </summary>
    /// <typeparam name="TSource">泛型类型</typeparam>
    /// <param name="sources">集合对象</param>
    /// <param name="conditionExpressions">条件表达式</param>
    /// <returns>新的集合对象</returns>
    public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> sources,
        params (bool condition, Expression<Func<TSource, bool>> expression)[] conditionExpressions)
    {
        var expressions = new List<Expression<Func<TSource, bool>>>();
        foreach (var (condition, expression) in conditionExpressions)
        {
            if (condition)
                expressions.Add(expression);
        }

        return Where(sources, expressions.ToArray());
    }

    /// <summary>
    /// 根据条件成立再构建 WhereOr 查询，支持索引器
    /// </summary>
    /// <typeparam name="TSource">泛型类型</typeparam>
    /// <param name="sources">集合对象</param>
    /// <param name="conditionExpressions">条件表达式</param>
    /// <returns>新的集合对象</returns>
    public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> sources,
        params (bool condition, Expression<Func<TSource, int, bool>> expression)[] conditionExpressions)
    {
        var expressions = new List<Expression<Func<TSource, int, bool>>>();
        foreach (var (condition, expression) in conditionExpressions)
        {
            if (condition)
                expressions.Add(expression);
        }

        return Where(sources, expressions.ToArray());
    }

    /// <summary>
    /// 根据条件成立再构建 Where 查询
    /// </summary>
    /// <typeparam name="TSource">泛型类型</typeparam>
    /// <param name="sources">集合对象</param>
    /// <param name="condition">布尔条件</param>
    /// <param name="expression">表达式</param>
    /// <returns>新的集合对象</returns>
    public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> sources, bool condition,
        Func<TSource, bool> expression)
    {
        return condition ? sources.Where(expression) : sources;
    }

    /// <summary>
    /// 根据条件成立再构建 Where 查询，支持索引器
    /// </summary>
    /// <typeparam name="TSource">泛型类型</typeparam>
    /// <param name="sources">集合对象</param>
    /// <param name="condition">布尔条件</param>
    /// <param name="expression">表达式</param>
    /// <returns>新的集合对象</returns>
    public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> sources, bool condition,
        Func<TSource, int, bool> expression)
    {
        return condition ? sources.Where(expression) : sources;
    }
}