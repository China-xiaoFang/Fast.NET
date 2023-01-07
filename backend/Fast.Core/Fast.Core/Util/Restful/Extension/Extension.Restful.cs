using System.Linq.Expressions;
using Fast.Core.Util.Restful.Internal;
using Fast.SqlSugar.Tenant.Extension;
using Fast.SqlSugar.Tenant.Internal.Dto;

namespace Fast.Core.Util.Restful.Extension;

/// <summary>
/// Restful风格返回扩展
/// </summary>
public static class Extension
{
    /// <summary>
    /// 替换SqlSugar分页
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="page"></param>
    /// <returns></returns>
    public static PageResult<T> XnPagedResult<T>(this SqlSugarPagedList<T> page)
    {
        return new PageResult<T>
        {
            PageNo = page.PageIndex,
            PageSize = page.PageSize,
            TotalPage = page.TotalPages,
            TotalRows = page.TotalCount,
            Rows = page.Items
        };
    }

    /// <summary>
    /// 小诺SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static PageResult<TEntity> ToXnPagedList<TEntity>(this ISugarQueryable<TEntity> queryable, int pageIndex, int pageSize)
    {
        return queryable.ToPagedList(pageIndex, pageSize).XnPagedResult();
    }

    /// <summary>
    /// 小诺SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static async Task<PageResult<TEntity>> ToXnPagedListAsync<TEntity>(this ISugarQueryable<TEntity> queryable,
        int pageIndex, int pageSize)
    {
        return (await queryable.ToPagedListAsync(pageIndex, pageSize)).XnPagedResult();
    }

    /// <summary>
    /// 小诺SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static PageResult<TResult> ToXnPagedList<TEntity, TResult>(this ISugarQueryable<TEntity> queryable, int pageIndex,
        int pageSize, Expression<Func<TEntity, TResult>> expression)
    {
        return queryable.ToPagedList(pageIndex, pageSize, expression).XnPagedResult();
    }

    /// <summary>
    /// 小诺SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static async Task<PageResult<TResult>> ToXnPagedListAsync<TEntity, TResult>(this ISugarQueryable<TEntity> queryable,
        int pageIndex, int pageSize, Expression<Func<TEntity, TResult>> expression)
    {
        return (await queryable.ToPagedListAsync(pageIndex, pageSize, expression)).XnPagedResult();
    }
}