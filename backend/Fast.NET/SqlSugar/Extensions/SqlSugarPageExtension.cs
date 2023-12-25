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
    /// <param name="input"><see cref="SqlSugarPageInput"/> 通用SqlSugar 分页输入</param>
    /// <returns></returns>
    public static SqlSugarPageResult<TEntity> ToPagedList<TEntity>(this ISugarQueryable<TEntity> queryable,
        SqlSugarPageInput input)
    {
        return queryable.ToPagedList(input.PageIndex, input.PageSize);
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="input"><see cref="SqlSugarPageInput"/> 通用SqlSugar 分页输入</param>
    /// <returns></returns>
    public static async Task<SqlSugarPageResult<TEntity>> ToPagedListAsync<TEntity>(this ISugarQueryable<TEntity> queryable,
        SqlSugarPageInput input)
    {
        return await queryable.ToPagedListAsync(input.PageIndex, input.PageSize);
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="input"><see cref="SqlSugarPageInput"/> 通用SqlSugar 分页输入</param>
    /// <param name="expression"><see cref="Expression"/> where 条件</param>
    /// <returns></returns>
    public static SqlSugarPageResult<TResult> ToPagedList<TEntity, TResult>(this ISugarQueryable<TEntity> queryable,
        SqlSugarPageInput input, Expression<Func<TEntity, TResult>> expression)
    {
        return queryable.ToPagedList(input.PageIndex, input.PageSize, expression);
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="input"><see cref="SqlSugarPageInput"/> 通用SqlSugar 分页输入</param>
    /// <param name="expression"><see cref="Expression"/> where 条件</param>
    /// <returns></returns>
    public static async Task<SqlSugarPageResult<TResult>> ToPagedListAsync<TEntity, TResult>(
        this ISugarQueryable<TEntity> queryable, SqlSugarPageInput input, Expression<Func<TEntity, TResult>> expression)
    {
        return await queryable.ToPagedListAsync(input.PageIndex, input.PageSize, expression);
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="pageIndex"><see cref="int"/> 页数</param>
    /// <param name="pageSize"><see cref="int"/> 页码</param>
    /// <returns></returns>
    public static SqlSugarPageResult<TEntity> ToPagedList<TEntity>(this ISugarQueryable<TEntity> queryable, int pageIndex,
        int pageSize)
    {
        var totalRows = 0;
        var rows = queryable.ToPageList(pageIndex, pageSize, ref totalRows);
        var totalPage = (int) Math.Ceiling(totalRows / (double) pageSize);
        return new SqlSugarPageResult<TEntity>
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
    public static async Task<SqlSugarPageResult<TEntity>> ToPagedListAsync<TEntity>(this ISugarQueryable<TEntity> queryable,
        int pageIndex, int pageSize)
    {
        RefAsync<int> totalRows = 0;
        var rows = await queryable.ToPageListAsync(pageIndex, pageSize, totalRows);
        var totalPage = (int) Math.Ceiling(totalRows.Value / (double) pageSize);
        return new SqlSugarPageResult<TEntity>
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
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="pageIndex"><see cref="int"/> 页数</param>
    /// <param name="pageSize"><see cref="int"/> 页码</param>
    /// <param name="expression"><see cref="Expression"/> where 条件</param>
    /// <returns></returns>
    public static SqlSugarPageResult<TResult> ToPagedList<TEntity, TResult>(this ISugarQueryable<TEntity> queryable,
        int pageIndex, int pageSize, Expression<Func<TEntity, TResult>> expression)
    {
        var totalRows = 0;
        var rows = queryable.ToPageList(pageIndex, pageSize, ref totalRows, expression);
        var totalPage = (int) Math.Ceiling(totalRows / (double) pageSize);
        return new SqlSugarPageResult<TResult>
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
    /// <typeparam name="TResult"></typeparam>
    /// <param name="queryable"><see cref="ISugarQueryable{T}"/></param>
    /// <param name="pageIndex"><see cref="int"/> 页数</param>
    /// <param name="pageSize"><see cref="int"/> 页码</param>
    /// <param name="expression"><see cref="Expression"/> where 条件</param>
    /// <returns></returns>
    public static async Task<SqlSugarPageResult<TResult>> ToPagedListAsync<TEntity, TResult>(
        this ISugarQueryable<TEntity> queryable, int pageIndex, int pageSize, Expression<Func<TEntity, TResult>> expression)
    {
        RefAsync<int> totalRows = 0;
        var rows = await queryable.ToPageListAsync(pageIndex, pageSize, totalRows, expression);
        var totalPage = (int) Math.Ceiling(totalRows.Value / (double) pageSize);
        return new SqlSugarPageResult<TResult>
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
}