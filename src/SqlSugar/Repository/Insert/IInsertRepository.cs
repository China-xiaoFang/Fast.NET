// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

namespace Fast.SqlSugar;

/// <summary>
/// SqlSugar 插入仓储接口
/// </summary>
public partial interface ISqlSugarRepository<TEntity>
{
    /// <summary>
    /// 新增一条记录
    /// </summary>
    /// <param name="entity">要处理的实体</param>
    /// <returns>新增一条记录</returns>
    int Insert(TEntity entity);

    /// <summary>
    /// 新增一条记录
    /// </summary>
    /// <param name="entity">要处理的实体</param>
    /// <returns>表示异步新增一条记录的任务，任务结果为新增一条记录</returns>
    Task<int> InsertAsync(TEntity entity);

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities">要批量处理的实体集合</param>
    /// <returns>新增多条记录</returns>
    int Insert(params TEntity[] entities);

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities">要批量处理的实体集合</param>
    /// <returns>表示异步新增多条记录的任务，任务结果为新增多条记录</returns>
    Task<int> InsertAsync(params TEntity[] entities);

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities">要批量处理的实体集合</param>
    /// <returns>新增多条记录</returns>
    int Insert(IEnumerable<TEntity> entities);

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities">要批量处理的实体集合</param>
    /// <returns>表示异步新增多条记录的任务，任务结果为新增多条记录</returns>
    Task<int> InsertAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// 新增一条记录返回自增Id
    /// </summary>
    /// <param name="entity">要处理的实体</param>
    /// <returns>新增一条记录返回自增Id</returns>
    int InsertReturnIdentity(TEntity entity);

    /// <summary>
    /// 新增一条记录返回自增Id
    /// </summary>
    /// <param name="entity">要处理的实体</param>
    /// <returns>表示异步新增一条记录返回自增Id的任务，任务结果为新增一条记录返回自增Id</returns>
    Task<int> InsertReturnIdentityAsync(TEntity entity);

    /// <summary>
    /// 新增一条记录返回 Long 类型的自增Id
    /// </summary>
    /// <param name="entity">要处理的实体</param>
    /// <returns>新增一条记录返回 Long 类型的自增Id</returns>
    long ExecuteReturnBigIdentity(TEntity entity);

    /// <summary>
    /// 新增一条记录返回 Long 类型的自增Id
    /// </summary>
    /// <param name="entity">要处理的实体</param>
    /// <returns>表示异步新增一条记录返回 Long 类型的自增Id的任务，任务结果为新增一条记录返回 Long 类型的自增Id</returns>
    Task<long> ExecuteReturnBigIdentityAsync(TEntity entity);

    /// <summary>
    /// 新增一条记录返回新增的数据
    /// </summary>
    /// <param name="entity">要处理的实体</param>
    /// <returns>新增一条记录返回新增的数据</returns>
    TEntity InsertReturnEntity(TEntity entity);

    /// <summary>
    /// 新增一条记录返回新增的数据
    /// </summary>
    /// <param name="entity">要处理的实体</param>
    /// <returns>表示异步新增一条记录返回新增的数据的任务，任务结果为新增一条记录返回新增的数据</returns>
    Task<TEntity> InsertReturnEntityAsync(TEntity entity);
}
