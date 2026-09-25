// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using System.Linq.Expressions;

namespace Fast.SqlSugar;

/// <summary>
/// SqlSugar 更新仓储接口
/// </summary>
public partial interface ISqlSugarRepository<TEntity>
{
    /// <summary>
    /// 更新一条记录
    /// </summary>
    /// <param name="entity">要处理的实体</param>
    /// <param name="isNoUpdateNull">更新时是否跳过值为 <see langword="null"/> 的属性</param>
    /// <returns>更新一条记录</returns>
    int Update(TEntity entity, bool isNoUpdateNull = false);

    /// <summary>
    /// 更新一条记录
    /// </summary>
    /// <param name="entity">要处理的实体</param>
    /// <param name="isNoUpdateNull">更新时是否跳过值为 <see langword="null"/> 的属性</param>
    /// <returns>表示异步更新一条记录的任务，任务结果为更新一条记录</returns>
    Task<int> UpdateAsync(TEntity entity, bool isNoUpdateNull = false);

    /// <summary>
    /// 更新多条记录
    /// </summary>
    /// <param name="entities">要批量处理的实体集合</param>
    /// <returns>更新多条记录</returns>
    int Update(params TEntity[] entities);

    /// <summary>
    /// 更新多条记录
    /// </summary>
    /// <param name="entities">要批量处理的实体集合</param>
    /// <returns>表示异步更新多条记录的任务，任务结果为更新多条记录</returns>
    Task<int> UpdateAsync(params TEntity[] entities);

    /// <summary>
    /// 更新多条记录
    /// </summary>
    /// <param name="entities">要批量处理的实体集合</param>
    /// <returns>更新多条记录</returns>
    int Update(IEnumerable<TEntity> entities);

    /// <summary>
    /// 更新多条记录
    /// </summary>
    /// <param name="entities">要批量处理的实体集合</param>
    /// <returns>表示异步更新多条记录的任务，任务结果为更新多条记录</returns>
    Task<int> UpdateAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// 无主键更新一条记录
    /// </summary>
    /// <param name="entity">要处理的实体</param>
    /// <param name="columns">参与更新的列集合</param>
    /// <returns>无主键更新一条记录</returns>
    int UpdateNoPrimaryKey(TEntity entity, Expression<Func<TEntity, object>> columns);

    /// <summary>
    /// 无主键更新一条记录
    /// </summary>
    /// <param name="entity">要处理的实体</param>
    /// <param name="columns">参与更新的列集合</param>
    /// <returns>表示异步无主键更新一条记录的任务，任务结果为无主键更新一条记录</returns>
    Task<int> UpdateNoPrimaryKeyAsync(TEntity entity, Expression<Func<TEntity, object>> columns);

    /// <summary>
    /// 无主键更新多条记录
    /// </summary>
    /// <param name="entity">要处理的实体</param>
    /// <param name="columns">参与更新的列集合</param>
    /// <returns>无主键更新多条记录</returns>
    int UpdateNoPrimaryKey(List<TEntity> entity, Expression<Func<TEntity, object>> columns);

    /// <summary>
    /// 无主键更新多条记录
    /// </summary>
    /// <param name="entity">要处理的实体</param>
    /// <param name="columns">参与更新的列集合</param>
    /// <returns>表示异步无主键更新多条记录的任务，任务结果为无主键更新多条记录</returns>
    Task<int> UpdateNoPrimaryKeyAsync(List<TEntity> entity, Expression<Func<TEntity, object>> columns);
}
