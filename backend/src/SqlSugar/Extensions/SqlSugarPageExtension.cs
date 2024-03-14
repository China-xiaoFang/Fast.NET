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
using System.Reflection;
using Fast.IaaS;
using SqlSugar;

namespace Fast.SqlSugar.Extensions;

/// <summary>
/// <see cref="ISugarQueryable{T}"/> ISugarQueryable 分页拓展类
/// </summary>
[SuppressSniffer]
public static class SqlSugarPageExtension
{
    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="input"><see cref="PagedInput"/> 通用SqlSugar 分页输入</param>
    /// <returns></returns>
    public static PagedResult<TEntity> ToPagedList<TEntity>(this ISugarQueryable<TEntity> queryable, PagedInput input)
    {
        return queryable.PagedWhere(input).PagedOrderBy(input.PagedSortList).ToPagedList(input.PageIndex, input.PageSize);
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="input"><see cref="PagedInput"/> 通用SqlSugar 分页输入</param>
    /// <returns></returns>
    public static async Task<PagedResult<TEntity>> ToPagedListAsync<TEntity>(this ISugarQueryable<TEntity> queryable,
        PagedInput input)
    {
        return await queryable.PagedWhere(input).PagedOrderBy(input.PagedSortList)
            .ToPagedListAsync(input.PageIndex, input.PageSize);
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="input"><see cref="PagedInput"/> 通用SqlSugar 分页输入</param>
    /// <param name="select"><see cref="string"/> 对应的Sql语句映射，例如 t1.*</param>
    /// <returns></returns>
    public static PagedResult<TResult> ToPagedList<TEntity, TResult>(this ISugarQueryable<TEntity> queryable, PagedInput input,
        string select = null)
    {
        queryable = queryable.PagedWhere(input).PagedOrderBy(input.PagedSortList);
        if (string.IsNullOrEmpty(select))
        {
            return queryable.Select<TResult>().ToPagedList(input.PageIndex, input.PageSize);
        }

        return queryable.PagedWhere(input).PagedOrderBy(input.PagedSortList).Select<TResult>(select)
            .ToPagedList(input.PageIndex, input.PageSize);
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="input"><see cref="PagedInput"/> 通用SqlSugar 分页输入</param>
    /// <param name="select"><see cref="string"/> 对应的Sql语句映射，例如 t1.*</param>
    /// <returns></returns>
    public static async Task<PagedResult<TResult>> ToPagedListAsync<TEntity, TResult>(this ISugarQueryable<TEntity> queryable,
        PagedInput input, string select = null)
    {
        queryable = queryable.PagedWhere(input).PagedOrderBy(input.PagedSortList);
        if (string.IsNullOrEmpty(select))
        {
            return await queryable.Select<TResult>().ToPagedListAsync(input.PageIndex, input.PageSize);
        }

        return await queryable.PagedWhere(input).PagedOrderBy(input.PagedSortList).Select<TResult>(select)
            .ToPagedListAsync(input.PageIndex, input.PageSize);
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="input"><see cref="PagedInput"/> 通用SqlSugar 分页输入</param>
    /// <param name="selectExpression"><see cref="Expression"/> Select关系映射</param>
    /// <param name="isAutoFill"><see cref="bool"/> 其余字段自动映射，根据字段名称，默认为 false</param>
    /// <returns></returns>
    public static PagedResult<TResult> ToPagedList<TEntity, TResult>(this ISugarQueryable<TEntity> queryable, PagedInput input,
        Expression<Func<TEntity, TResult>> selectExpression, bool isAutoFill = false)
    {
        return queryable.PagedWhere(input).PagedOrderBy(input.PagedSortList).Select(selectExpression, isAutoFill)
            .ToPagedList(input.PageIndex, input.PageSize);
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="input"><see cref="PagedInput"/> 通用SqlSugar 分页输入</param>
    /// <param name="selectExpression"><see cref="Expression"/> Select关系映射</param>
    /// <param name="isAutoFill"><see cref="bool"/> 其余字段自动映射，根据字段名称，默认为 false</param>
    /// <returns></returns>
    public static async Task<PagedResult<TResult>> ToPagedListAsync<TEntity, TResult>(this ISugarQueryable<TEntity> queryable,
        PagedInput input, Expression<Func<TEntity, TResult>> selectExpression, bool isAutoFill = false)
    {
        return await queryable.PagedWhere(input).PagedOrderBy(input.PagedSortList).Select(selectExpression, isAutoFill)
            .ToPagedListAsync(input.PageIndex, input.PageSize);
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="pageIndex"><see cref="int"/> 页数</param>
    /// <param name="pageSize"><see cref="int"/> 页码</param>
    /// <returns></returns>
    public static PagedResult<TEntity> ToPagedList<TEntity>(this ISugarQueryable<TEntity> queryable, int pageIndex, int pageSize)
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

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="pageIndex"><see cref="int"/> 页数</param>
    /// <param name="pageSize"><see cref="int"/> 页码</param>
    /// <returns></returns>
    public static async Task<PagedResult<TEntity>> ToPagedListAsync<TEntity>(this ISugarQueryable<TEntity> queryable,
        int pageIndex, int pageSize)
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

    /// <summary>
    /// 分页搜索
    /// <remarks>支持多库</remarks>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="input"><see cref="PagedInput"/> 统一分页输入</param>
    /// <returns></returns>
    public static ISugarQueryable<TEntity> PagedWhere<TEntity>(this ISugarQueryable<TEntity> queryable, PagedInput input)
    {
        var isSearchValue = !string.IsNullOrEmpty(input.SearchValue);
        var isSearchTime = input.SearchTimeList is {Count: > 0};

        if (!isSearchValue && !isSearchTime)
        {
            return queryable;
        }

        // 获取当前实体类型
        var type = typeof(TEntity);

        // 获取所有属性
        var properties = type.GetProperties();

        // 获取所有存在 SugarSearchValueAttribute 和 SugarSearchTimeAttribute 特性的属性
        var needProperties = properties.Select(sl => new
        {
            propertyInfo = sl,
            sugarColumn = sl.GetCustomAttribute<SugarColumn>(),
            sugarSearchValueAttribute = sl.GetCustomAttribute<SugarSearchValueAttribute>(),
            sugarSearchTimeAttribute = sl.GetCustomAttribute<SugarSearchTimeAttribute>(),
        }).ToList();

        var conditionals = new List<IConditionalModel>();

        // 关键字搜索
        if (isSearchValue)
        {
            var valConditionals = new List<KeyValuePair<WhereType, ConditionalModel>>();

            var index = 0;

            foreach (var valItem in needProperties.Where(wh => wh.sugarSearchValueAttribute != null))
            {
                var whereType = WhereType.Or;
                if (index == 0)
                {
                    whereType = WhereType.And;
                }

                valConditionals.Add(new KeyValuePair<WhereType, ConditionalModel>(whereType,
                    new ConditionalModel
                    {
                        FieldName = string.IsNullOrEmpty(valItem.sugarColumn?.ColumnName)
                            ? valItem.propertyInfo.Name
                            : valItem.sugarColumn.ColumnName,
                        ConditionalType = ConditionalType.Like,
                        FieldValue = input.SearchValue
                    }));

                index++;
            }

            if (valConditionals.Any())
            {
                conditionals.Add(new ConditionalCollections {ConditionalList = valConditionals});
            }
        }

        // 时间范围搜索
        if (isSearchTime)
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

            foreach (var valItem in needProperties.Where(wh => wh.sugarSearchTimeAttribute != null))
            {
                // 如果两个时间都存在，则使用范围搜索，如果只存在一个，则使用 >= 或者 <=
                var timeConditionals = new List<KeyValuePair<WhereType, ConditionalModel>>();

                if (time1 != null && time2 != null)
                {
                    // 范围搜索
                    timeConditionals.Add(new KeyValuePair<WhereType, ConditionalModel>(WhereType.And,
                        new ConditionalModel
                        {
                            FieldName = string.IsNullOrEmpty(valItem.sugarColumn?.ColumnName)
                                ? valItem.propertyInfo.Name
                                : valItem.sugarColumn.ColumnName,
                            ConditionalType = ConditionalType.GreaterThanOrEqual,
                            FieldValue = time1.ToString(),
                            CSharpTypeName = nameof(DateTime)
                        }));
                    timeConditionals.Add(new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or,
                        new ConditionalModel
                        {
                            FieldName = string.IsNullOrEmpty(valItem.sugarColumn?.ColumnName)
                                ? valItem.propertyInfo.Name
                                : valItem.sugarColumn.ColumnName,
                            ConditionalType = ConditionalType.LessThanOrEqual,
                            FieldValue = time2.ToString(),
                            CSharpTypeName = nameof(DateTime)
                        }));
                }
                else if (time1 != null)
                {
                    // 大于等于开始时间
                    timeConditionals.Add(new KeyValuePair<WhereType, ConditionalModel>(WhereType.And,
                        new ConditionalModel
                        {
                            FieldName = string.IsNullOrEmpty(valItem.sugarColumn?.ColumnName)
                                ? valItem.propertyInfo.Name
                                : valItem.sugarColumn.ColumnName,
                            ConditionalType = ConditionalType.GreaterThanOrEqual,
                            FieldValue = time1.ToString(),
                            CSharpTypeName = nameof(DateTime)
                        }));
                }
                else if (time2 != null)
                {
                    // 小于等于结束时间
                    timeConditionals.Add(new KeyValuePair<WhereType, ConditionalModel>(WhereType.And,
                        new ConditionalModel
                        {
                            FieldName = string.IsNullOrEmpty(valItem.sugarColumn?.ColumnName)
                                ? valItem.propertyInfo.Name
                                : valItem.sugarColumn.ColumnName,
                            ConditionalType = ConditionalType.LessThanOrEqual,
                            FieldValue = time2.ToString(),
                            CSharpTypeName = nameof(DateTime)
                        }));
                }

                conditionals.Add(new ConditionalCollections {ConditionalList = timeConditionals});
            }
        }

        if (conditionals.Any())
        {
            queryable = queryable.Where(conditionals);
        }

        return queryable;
    }

    /// <summary>
    /// 分页排序
    /// <remarks>支持多库</remarks>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="pagedSortList"><see cref="List{PagedSortInput}"/> 排序输入</param>
    /// <returns></returns>
    public static ISugarQueryable<TEntity> PagedOrderBy<TEntity>(this ISugarQueryable<TEntity> queryable,
        params PagedSortInput[] pagedSortList)
    {
        if (pagedSortList == null || pagedSortList.Length == 0)
        {
            return queryable;
        }

        // 这里必须要判断，排序的字段是否存在于 TEntity 中，不然会执行到Db层面的报错
        var type = typeof(TEntity);

        // 获取所有属性
        var properties = type.GetProperties();

        var orderList = new List<OrderByModel>();

        // 循环传入的集合
        foreach (var pagedSortInput in pagedSortList)
        {
            var propertyInfo = properties.FirstOrDefault(f =>
                f.Name.Equals(pagedSortInput.EnField, StringComparison.InvariantCultureIgnoreCase));

            if (propertyInfo == null)
            {
                throw new SqlSugarException($"排序字段 [{pagedSortInput.ChField}] 不存在于类型 [{type.Name}] 中！");
            }

            // 获取属性列的 SugarColumn 特性
            var sugarColumn = propertyInfo.GetCustomAttribute<SugarColumn>();

            if (sugarColumn?.IsIgnore == true)
            {
                // 如果存在特性，且 IsIgnore = true，则代表不是Db列，不能进行排序
                throw new SqlSugarException($"类型 [{type.Name}] 中的排序字段 [{pagedSortInput.ChField}] 不存在于对应的Db中！");
            }

            // 获取属性列的 Navigate 特性
            if (propertyInfo.GetCustomAttribute<Navigate>() != null)
            {
                // 如果存在特性，则代表是一个导航属性，不能进行排序
                throw new SqlSugarException($"类型 [{type.Name}] 中的排序字段 [{pagedSortInput.ChField}] 是一个导航属性！");
            }

            orderList.Add(new OrderByModel
            {
                FieldName = string.IsNullOrEmpty(sugarColumn?.ColumnName) ? propertyInfo.Name : sugarColumn.ColumnName,
                OrderByType = pagedSortInput.IsDescending ? OrderByType.Desc : OrderByType.Asc
            });
        }

        queryable = queryable.OrderBy(orderList);

        return queryable;
    }
}