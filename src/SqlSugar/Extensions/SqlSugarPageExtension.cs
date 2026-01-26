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

using System.Linq.Expressions;
using System.Reflection;
using SqlSugar;

namespace Fast.SqlSugar;

/// <summary>
/// <see cref="ISugarQueryable{T}"/> ISugarQueryable 分页拓展类
/// </summary>
[SuppressSniffer]
public static class SqlSugarPageExtension
{
    /// <summary>
    /// 分页转换类型
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="pagedResult"><see cref="PagedResult{TResult}"/> 源类型</param>
    /// <param name="selectExpression"><see cref="Expression"/> Select关系映射</param>
    /// <returns></returns>
    public static PagedResult<TResult> ToPagedData<TEntity, TResult>(this PagedResult<TEntity> pagedResult,
        Func<TEntity, TResult> selectExpression)
    {
        return new PagedResult<TResult>
        {
            PageIndex = pagedResult.PageIndex,
            PageSize = pagedResult.PageSize,
            Rows = pagedResult.Rows.Select(selectExpression)
                .ToList(),
            TotalRows = pagedResult.TotalRows,
            TotalPage = pagedResult.TotalPage,
            HasNextPages = pagedResult.HasNextPages,
            HasPrevPages = pagedResult.HasPrevPages
        };
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="input"><see cref="PagedInput"/> 通用SqlSugar 分页输入</param>
    /// <remarks>
    /// 多表查询（LeftJoin/Join）场景需注意：分页统计依赖 MergeTable，建议在分页前进行结构收敛：
    /// <code>
    /// .Select((t1, t2) => new { ... }).MergeTable()
    /// </code>
    /// 或
    /// <code>
    /// .SelectMergeTable((t1, t2) => new { ... })
    /// </code>
    /// 未使用 MergeTable 可能导致总数统计异常或结果不准确。
    /// </remarks>
    /// <returns></returns>
    public static PagedResult<TEntity> ToPagedList<TEntity>(this ISugarQueryable<TEntity> queryable, PagedInput input)
    {
        return queryable.SugarPaged(input)
            .ToPagedList(input.PageIndex, input.PageSize, input.EnablePaged);
    }


    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="input"><see cref="PagedInput"/> 通用SqlSugar 分页输入</param>
    /// <remarks>
    /// 多表查询（LeftJoin/Join）场景需注意：分页统计依赖 MergeTable，建议在分页前进行结构收敛：
    /// <code>
    /// .Select((t1, t2) => new { ... }).MergeTable()
    /// </code>
    /// 或
    /// <code>
    /// .SelectMergeTable((t1, t2) => new { ... })
    /// </code>
    /// 未使用 MergeTable 可能导致总数统计异常或结果不准确。
    /// </remarks>
    /// <returns></returns>
    public static async Task<PagedResult<TEntity>> ToPagedListAsync<TEntity>(this ISugarQueryable<TEntity> queryable,
        PagedInput input)
    {
        return await queryable.SugarPaged(input)
            .ToPagedListAsync(input.PageIndex, input.PageSize, input.EnablePaged);
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="pageIndex"><see cref="int"/> 页数</param>
    /// <param name="pageSize"><see cref="int"/> 页码，默认 20</param>
    /// <param name="enablePaged"><see cref="bool"/> 启用分页，默认 true</param>
    /// <returns></returns>
    public static PagedResult<TEntity> ToPagedList<TEntity>(this ISugarQueryable<TEntity> queryable, int pageIndex,
        int pageSize = 20, bool enablePaged = true)
    {
        if (enablePaged)
        {
            var totalRows = 0;
            var rows = queryable.ToPageList(pageIndex, pageSize, ref totalRows);
            var totalPage = (int) Math.Ceiling(totalRows / (double) pageSize);

            return new PagedResult<TEntity>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Rows = rows,
                TotalRows = totalRows,
                TotalPage = totalPage,
                HasNextPages = pageIndex < totalPage,
                HasPrevPages = pageIndex - 1 > 0
            };
        }
        else
        {
            var rows = queryable.Take(SqlSugarContext.MaxNotPageSize)
                .ToList();
            if (rows.Count >= SqlSugarContext.MaxNotPageSize)
            {
                throw new SqlSugarException($"当前查询数据量超过 {SqlSugarContext.MaxNotPageSize} 条，请使用分页查询或缩小查询范围。");
            }

            return new PagedResult<TEntity>
            {
                PageIndex = 1,
                PageSize = rows.Count,
                Rows = rows,
                TotalRows = rows.Count,
                TotalPage = 1,
                HasNextPages = false,
                HasPrevPages = false
            };
        }
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="pageIndex"><see cref="int"/> 页数</param>
    /// <param name="pageSize"><see cref="int"/> 页码，默认 20</param>
    /// <param name="enablePaged"><see cref="bool"/> 启用分页，默认 true</param>
    /// <returns></returns>
    public static async Task<PagedResult<TEntity>> ToPagedListAsync<TEntity>(this ISugarQueryable<TEntity> queryable,
        int pageIndex, int pageSize = 20, bool enablePaged = true)
    {
        if (enablePaged)
        {
            RefAsync<int> totalRows = 0;
            var rows = await queryable.ToPageListAsync(pageIndex, pageSize, totalRows);
            var totalPage = (int) Math.Ceiling(totalRows.Value / (double) pageSize);

            return new PagedResult<TEntity>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Rows = rows,
                TotalRows = totalRows.Value,
                TotalPage = totalPage,
                HasNextPages = pageIndex < totalPage,
                HasPrevPages = pageIndex - 1 > 0
            };
        }
        else
        {
            var rows = await queryable.Take(SqlSugarContext.MaxNotPageSize)
                .ToListAsync();
            if (rows.Count >= SqlSugarContext.MaxNotPageSize)
            {
                throw new SqlSugarException($"当前查询数据量超过 {SqlSugarContext.MaxNotPageSize} 条，请使用分页查询或缩小查询范围。");
            }

            return new PagedResult<TEntity>
            {
                PageIndex = 1,
                PageSize = rows.Count,
                Rows = rows,
                TotalRows = rows.Count,
                TotalPage = 1,
                HasNextPages = false,
                HasPrevPages = false
            };
        }
    }

    /// <summary>
    /// Sugar 统一分页处理
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="input"><see cref="PagedInput"/> 统一分页输入</param>
    /// <remarks>支持多库</remarks>
    /// <returns></returns>
    public static ISugarQueryable<TEntity> SugarPaged<TEntity>(this ISugarQueryable<TEntity> queryable, PagedInput input)
    {
        // 这里必须要判断，字段是否存在于 TEntity 中，不然会执行到Db层面的报错
        var type = typeof(TEntity);

        // 获取所有属性
        var properties = type.GetProperties()
            .Select(sl => new
            {
                propertyInfo = sl,
                sugarColumn = sl.GetCustomAttribute<SugarColumn>(true),
                navigate = sl.GetCustomAttribute<Navigate>(true),
                sugarSearchValueAttribute = sl.GetCustomAttribute<SugarSearchValueAttribute>(),
                sugarSearchTimeAttribute = sl.GetCustomAttribute<SugarSearchTimeAttribute>(),
                sugarOrderByAttribute = sl.GetCustomAttribute<SugarOrderByAttribute>()
            })
            .ToList();

        var whereList = new List<IConditionalModel>();

        // 搜索列
        if (!string.IsNullOrEmpty(input.SearchValue))
        {
            var searchList = new List<KeyValuePair<WhereType, ConditionalModel>>();

            var index = 0;

            foreach (var item in properties.Where(wh => wh.sugarSearchValueAttribute != null)
                         .ToList())
            {
                var whereType = WhereType.Or;
                if (index == 0)
                {
                    whereType = WhereType.And;
                }

                searchList.Add(new KeyValuePair<WhereType, ConditionalModel>(whereType,
                    new ConditionalModel
                    {
                        FieldName = string.IsNullOrEmpty(item.sugarColumn?.ColumnName)
                            ? item.propertyInfo.Name
                            : item.sugarColumn.ColumnName,
                        ConditionalType = ConditionalType.Like,
                        FieldValue = input.SearchValue
                    }));

                index++;
            }

            if (searchList.Any())
            {
                whereList.Add(new ConditionalCollections {ConditionalList = searchList});
            }
        }

        // 搜素时间列
        if (input.SearchTimeList is {Count: > 0})
        {
            DateTime? time1 = null;
            DateTime? time2 = null;

            switch (input.SearchTimeList.Count)
            {
                case >= 2:
                    time1 = input.SearchTimeList[0];
                    time2 = input.SearchTimeList[1];
                    break;
                case 1:
                    time1 = input.SearchTimeList[0];
                    break;
            }

            // 如果两个时间都存在，则使用范围搜索，如果只存在一个，则使用 >= 或者 <=
            var searchList = new List<KeyValuePair<WhereType, ConditionalModel>>();

            foreach (var item in properties.Where(wh => wh.sugarSearchTimeAttribute != null)
                         .ToList())
            {
                if (time1 != null && time2 != null)
                {
                    // 范围搜索
                    searchList.Add(new KeyValuePair<WhereType, ConditionalModel>(WhereType.And,
                        new ConditionalModel
                        {
                            FieldName = string.IsNullOrEmpty(item.sugarColumn?.ColumnName)
                                ? item.propertyInfo.Name
                                : item.sugarColumn.ColumnName,
                            ConditionalType = ConditionalType.GreaterThanOrEqual,
                            FieldValue = time1.ToString(),
                            CSharpTypeName = nameof(DateTime)
                        }));
                    searchList.Add(new KeyValuePair<WhereType, ConditionalModel>(WhereType.And,
                        new ConditionalModel
                        {
                            FieldName = string.IsNullOrEmpty(item.sugarColumn?.ColumnName)
                                ? item.propertyInfo.Name
                                : item.sugarColumn.ColumnName,
                            ConditionalType = ConditionalType.LessThanOrEqual,
                            FieldValue = time2.ToString(),
                            CSharpTypeName = nameof(DateTime)
                        }));
                }
                else if (time1 != null)
                {
                    // 大于等于开始时间
                    searchList.Add(new KeyValuePair<WhereType, ConditionalModel>(WhereType.And,
                        new ConditionalModel
                        {
                            FieldName = string.IsNullOrEmpty(item.sugarColumn?.ColumnName)
                                ? item.propertyInfo.Name
                                : item.sugarColumn.ColumnName,
                            ConditionalType = ConditionalType.GreaterThanOrEqual,
                            FieldValue = time1.ToString(),
                            CSharpTypeName = nameof(DateTime)
                        }));
                }
                else if (time2 != null)
                {
                    // 小于等于结束时间
                    searchList.Add(new KeyValuePair<WhereType, ConditionalModel>(WhereType.And,
                        new ConditionalModel
                        {
                            FieldName = string.IsNullOrEmpty(item.sugarColumn?.ColumnName)
                                ? item.propertyInfo.Name
                                : item.sugarColumn.ColumnName,
                            ConditionalType = ConditionalType.LessThanOrEqual,
                            FieldValue = time2.ToString(),
                            CSharpTypeName = nameof(DateTime)
                        }));
                }
            }

            if (searchList.Any())
            {
                whereList.Add(new ConditionalCollections {ConditionalList = searchList});
            }
        }

        // 循环传入的集合
        foreach (var searchInput in input.SearchList)
        {
            var item = properties.FirstOrDefault(f =>
                f.propertyInfo.Name.Equals(searchInput.EnField, StringComparison.InvariantCultureIgnoreCase));

            if (item == null)
            {
                throw new SqlSugarException($"搜索字段 [{searchInput.ChField}] 不存在于类型 [{type.Name}] 中！");
            }

            if (item.sugarColumn?.IsIgnore == true)
            {
                // 如果存在特性，且 IsIgnore = true，则代表不是Db列，不能进行搜索
                throw new SqlSugarException($"类型 [{type.Name}] 中的搜索字段 [{searchInput.ChField}] 不存在于对应的Db中！");
            }

            // 获取属性列的 Navigate 特性
            if (item.navigate != null)
            {
                // 如果存在特性，则代表是一个导航属性，不能进行搜索
                throw new SqlSugarException($"类型 [{type.Name}] 中的搜索字段 [{searchInput.ChField}] 是一个导航属性！");
            }

            var conditionalType = searchInput.Type switch
            {
                PagedSearchTypeEnum.Equal => ConditionalType.Equal,
                PagedSearchTypeEnum.NotEqual => ConditionalType.NoEqual,
                PagedSearchTypeEnum.GreaterThan => ConditionalType.GreaterThan,
                PagedSearchTypeEnum.GreaterThanOrEqual => ConditionalType.GreaterThanOrEqual,
                PagedSearchTypeEnum.LessThan => ConditionalType.LessThan,
                PagedSearchTypeEnum.LessThanOrEqual => ConditionalType.LessThanOrEqual,
                PagedSearchTypeEnum.Include => ConditionalType.In,
                PagedSearchTypeEnum.NotInclude => ConditionalType.NotIn,
                _ => ConditionalType.Like
            };

            whereList.Add(new ConditionalModel
            {
                FieldName = string.IsNullOrEmpty(item.sugarColumn?.ColumnName)
                    ? item.propertyInfo.Name
                    : item.sugarColumn.ColumnName,
                FieldValue = searchInput.Value,
                ConditionalType = conditionalType
            });
        }

        if (whereList.Any())
        {
            queryable = queryable.Where(whereList);
        }

        var orderList = new List<OrderByModel>();

        // 排序列
        if (input.SortList is {Length: > 0})
        {
            // 循环传入的集合
            foreach (var sortInput in input.SortList)
            {
                var item = properties.FirstOrDefault(f =>
                    f.propertyInfo.Name.Equals(sortInput.EnField, StringComparison.InvariantCultureIgnoreCase));

                if (item == null)
                {
                    throw new SqlSugarException($"排序字段 [{sortInput.ChField}] 不存在于类型 [{type.Name}] 中！");
                }

                if (item.sugarColumn?.IsIgnore == true)
                {
                    // 如果存在特性，且 IsIgnore = true，则代表不是Db列，不能进行排序
                    throw new SqlSugarException($"类型 [{type.Name}] 中的排序字段 [{sortInput.ChField}] 不存在于对应的Db中！");
                }

                // 获取属性列的 Navigate 特性
                if (item.navigate != null)
                {
                    // 如果存在特性，则代表是一个导航属性，不能进行排序
                    throw new SqlSugarException($"类型 [{type.Name}] 中的排序字段 [{sortInput.ChField}] 是一个导航属性！");
                }

                orderList.Add(new OrderByModel
                {
                    FieldName = string.IsNullOrEmpty(item.sugarColumn?.ColumnName)
                        ? item.propertyInfo.Name
                        : item.sugarColumn.ColumnName,
                    OrderByType = sortInput.IsDescending ? OrderByType.Desc : OrderByType.Asc
                });
            }
        }
        else
        {
            foreach (var item in properties.Where(wh => wh.sugarOrderByAttribute != null)
                         .OrderBy(ob => ob.sugarOrderByAttribute.Order)
                         .ToList())
            {
                orderList.Add(new OrderByModel
                {
                    FieldName = string.IsNullOrEmpty(item.sugarColumn?.ColumnName)
                        ? item.propertyInfo.Name
                        : item.sugarColumn.ColumnName,
                    OrderByType = item.sugarOrderByAttribute.Type
                });
            }
        }

        if (orderList.Any())
        {
            queryable = queryable.OrderBy(orderList);
        }

        return queryable;
    }
}