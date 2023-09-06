using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Gejia.WMS.Iaas.Extension
{
    /// <summary>
    /// GroupBy 扩展
    /// </summary>
    public static class Extensions
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
}