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

using System.Linq.Expressions;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="EnumExtension"/> GroupBy 拓展类
/// </summary>
[SuppressSniffer]
public static class GroupByExtension
{
    /// <summary>
    /// 多个GroupBy
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="source"></param>
    /// <param name="groupByProperties"></param>
    /// <returns></returns>
    public static IEnumerable<IGrouping<string, TKey>> GroupByMultiple<TKey>(this IEnumerable<TKey> source,
        params Expression<Func<TKey, object>>[] groupByProperties)
    {
        var query = source.AsQueryable();
        var parameter = Expression.Parameter(typeof(TKey), "gb");
        Expression keySelector = null;

        foreach (var property in groupByProperties)
        {
            var memberExpression = Expression.Invoke(property, parameter);
            var conversionExpression = Expression.Convert(memberExpression, typeof(object));
            var nullCheckExpression = Expression.Condition(Expression.Equal(memberExpression, Expression.Constant(null)),
                Expression.Constant(""), Expression.Call(conversionExpression, "ToString", null));

            if (keySelector == null)
            {
                keySelector = nullCheckExpression;
            }
            else
            {
                keySelector = Expression.Call(typeof(string).GetMethod("Concat", new[] {typeof(string), typeof(string)}),
                    keySelector, nullCheckExpression);
            }
        }

        var lambda = Expression.Lambda<Func<TKey, string>>(keySelector, parameter);
        var groupByExpression = Expression.Call(typeof(Queryable), "GroupBy", new[] {typeof(TKey), typeof(string)},
            query.Expression, lambda);
        var result = query.Provider.CreateQuery<IGrouping<string, TKey>>(groupByExpression);

        return result;
    }
}