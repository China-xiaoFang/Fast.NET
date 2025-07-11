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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="EnumExtension"/> GroupBy 拓展类
/// </summary>
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