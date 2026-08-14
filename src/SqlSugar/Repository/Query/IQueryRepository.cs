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
using SqlSugar;

namespace Fast.SqlSugar;

/// <summary>
/// SqlSugar 查询仓储接口
/// </summary>
public partial interface ISqlSugarRepository<TEntity>
{
    /// <summary>
    /// 获取总数
    /// </summary>
    /// <param name="whereExpression">用于筛选实体的条件表达式</param>
    /// <returns>满足条件的项数</returns>
    int Count(Expression<Func<TEntity, bool>> whereExpression = null);

    /// <summary>
    /// 获取总数
    /// </summary>
    /// <param name="whereExpression">用于筛选实体的条件表达式</param>
    /// <returns>表示异步获取总数的任务，任务结果为满足条件的项数</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> whereExpression = null);

    /// <summary>
    /// 检查是否存在
    /// </summary>
    /// <param name="whereExpression">用于筛选实体的条件表达式</param>
    /// <returns>满足条件时返回 <see langword="true"/>；否则返回 <see langword="false"/></returns>
    bool Any(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 检查是否存在
    /// </summary>
    /// <param name="whereExpression">用于筛选实体的条件表达式</param>
    /// <returns>满足条件时返回 <see langword="true"/>；否则返回 <see langword="false"/></returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 根据主键获取实体
    /// </summary>
    /// <param name="Id">唯一标识</param>
    /// <returns>根据主键获取实体</returns>
    TEntity SingleOrDefault(object Id);

    /// <summary>
    /// 根据主键获取实体
    /// </summary>
    /// <param name="Id">唯一标识</param>
    /// <returns>表示异步根据主键获取实体的任务，任务结果为根据主键获取实体</returns>
    Task<TEntity> SingleOrDefaultAsync(object Id);

    /// <summary>
    /// 根据条件获取实体
    /// </summary>
    /// <param name="whereExpression">用于筛选实体的条件表达式</param>
    /// <returns>根据条件获取实体</returns>
    TEntity SingleOrDefault(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 根据条件获取实体
    /// </summary>
    /// <param name="whereExpression">用于筛选实体的条件表达式</param>
    /// <returns>表示异步根据条件获取实体的任务，任务结果为根据条件获取实体</returns>
    Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 获取一个实体
    /// </summary>
    /// <param name="whereExpression">用于筛选实体的条件表达式</param>
    /// <returns>获取到的一个实体</returns>
    TEntity FirstOrDefault(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 获取一个实体
    /// </summary>
    /// <param name="whereExpression">用于筛选实体的条件表达式</param>
    /// <returns>表示异步获取一个实体的任务，任务结果为获取到的一个实体</returns>
    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <returns>获取到的列表集合</returns>
    List<TEntity> ToList();

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <returns>表示异步获取列表的任务，任务结果为获取到的列表集合</returns>
    Task<List<TEntity>> ToListAsync();

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <param name="whereExpression">用于筛选实体的条件表达式</param>
    /// <returns>获取到的列表集合</returns>
    List<TEntity> ToList(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <param name="whereExpression">用于筛选实体的条件表达式</param>
    /// <returns>表示异步获取列表的任务，任务结果为获取到的列表集合</returns>
    Task<List<TEntity>> ToListAsync(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <param name="whereExpression">用于筛选实体的条件表达式</param>
    /// <param name="orderByExpression">用于指定排序字段的表达式</param>
    /// <param name="orderByType">排序方向</param>
    /// <returns>获取到的列表集合</returns>
    List<TEntity> ToList(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression,
        OrderByType orderByType = OrderByType.Asc);

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <param name="whereExpression">用于筛选实体的条件表达式</param>
    /// <param name="orderByExpression">用于指定排序字段的表达式</param>
    /// <param name="orderByType">排序方向</param>
    /// <returns>表示异步获取列表的任务，任务结果为获取到的列表集合</returns>
    Task<List<TEntity>> ToListAsync(Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<TEntity, object>> orderByExpression, OrderByType orderByType = OrderByType.Asc);
}
