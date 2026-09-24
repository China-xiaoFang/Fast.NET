// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System.Linq.Expressions;

namespace Fast.SqlSugar;

/// <summary>
/// SqlSugar 删除仓储接口
/// </summary>
public partial interface ISqlSugarRepository<TEntity>
{
    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="entity">要处理的实体</param>
    /// <returns>实际删除的项数</returns>
    int Delete(TEntity entity);

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="entity">要处理的实体</param>
    /// <returns>表示异步删除一条记录的任务，任务结果为实际删除的项数</returns>
    Task<int> DeleteAsync(TEntity entity);

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="key">要删除实体的主键值</param>
    /// <returns>实际删除的项数</returns>
    int Delete(object key);

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="key">要删除实体的主键值</param>
    /// <returns>表示异步删除一条记录的任务，任务结果为实际删除的项数</returns>
    Task<int> DeleteAsync(object key);

    /// <summary>
    /// 删除多条记录
    /// </summary>
    /// <param name="keys">用于定位目标数据的键集合</param>
    /// <returns>实际删除的项数</returns>
    int Delete(params object[] keys);

    /// <summary>
    /// 删除多条记录
    /// </summary>
    /// <param name="keys">用于定位目标数据的键集合</param>
    /// <returns>表示异步删除多条记录的任务，任务结果为实际删除的项数</returns>
    Task<int> DeleteAsync(params object[] keys);

    /// <summary>
    /// 删除多条记录
    /// </summary>
    /// <param name="entities">要批量处理的实体集合</param>
    /// <returns>实际删除的项数</returns>
    int Delete(params TEntity[] entities);

    /// <summary>
    /// 删除多条记录
    /// </summary>
    /// <param name="entities">要批量处理的实体集合</param>
    /// <returns>表示异步删除多条记录的任务，任务结果为实际删除的项数</returns>
    Task<int> DeleteAsync(params TEntity[] entities);

    /// <summary>
    /// 删除多条记录
    /// </summary>
    /// <param name="entities">要批量处理的实体集合</param>
    /// <returns>实际删除的项数</returns>
    int Delete(IEnumerable<TEntity> entities);

    /// <summary>
    /// 删除多条记录
    /// </summary>
    /// <param name="entities">要批量处理的实体集合</param>
    /// <returns>表示异步删除多条记录的任务，任务结果为实际删除的项数</returns>
    Task<int> DeleteAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// 自定义条件删除记录
    /// </summary>
    /// <param name="whereExpression">用于筛选实体的条件表达式</param>
    /// <returns>实际删除的项数</returns>
    int Delete(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 自定义条件删除记录
    /// </summary>
    /// <param name="whereExpression">用于筛选实体的条件表达式</param>
    /// <returns>表示异步自定义条件删除记录的任务，任务结果为实际删除的项数</returns>
    Task<int> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 自定义条件逻辑删除记录
    /// </summary>
    /// <remarks>注意，实体必须继承 <see cref="IDeletedEntity"/></remarks>
    /// <param name="whereExpression">用于筛选实体的条件表达式</param>
    /// <returns>自定义条件逻辑删除记录</returns>
    int LogicDelete(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 自定义条件逻辑删除记录
    /// </summary>
    /// <remarks>注意，实体必须继承 <see cref="IDeletedEntity"/></remarks>
    /// <param name="whereExpression">用于筛选实体的条件表达式</param>
    /// <returns>表示异步自定义条件逻辑删除记录的任务，任务结果为自定义条件逻辑删除记录</returns>
    Task<int> LogicDeleteAsync(Expression<Func<TEntity, bool>> whereExpression);
}
