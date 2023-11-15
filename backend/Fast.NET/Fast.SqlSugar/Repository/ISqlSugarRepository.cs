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
using Fast.SqlSugar.IBaseEntities;
using Fast.SqlSugar.Options;
using SqlSugar;

namespace Fast.SqlSugar.Repository;

/// <summary>
/// <see cref="ISqlSugarRepository{TEntity}"/> SqlSugar仓储接口
/// </summary>
public interface ISqlSugarRepository<TEntity> : ISqlSugarClient where TEntity : class, new()
{
    /// <summary>
    /// 当前仓储的数据库信息
    /// </summary>
    ConnectionSettingsOptions DataBaseInfo { get; }

    /// <summary>
    /// 构建查询分析器
    /// </summary>
    /// <returns></returns>
    ISugarQueryable<TEntity> AsQueryable();

    /// <summary>
    /// 构建查询分析器
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    ISugarQueryable<TEntity> AsQueryable(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 切换仓储/切换租户仓储
    /// </summary>
    /// <typeparam name="TChangeEntity">实体类型</typeparam>
    /// <returns>仓储</returns>
    ISqlSugarRepository<TChangeEntity> Change<TChangeEntity>() where TChangeEntity : class, new();

    /// <summary>
    /// 获取总数
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    int Count(Expression<Func<TEntity, bool>> whereExpression = null);

    /// <summary>
    /// 获取总数
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> whereExpression = null);

    /// <summary>
    /// 检查是否存在
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    bool Any(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 检查是否存在
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 根据主键获取实体
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    TEntity Single(object Id);

    /// <summary>
    /// 根据条件获取实体
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    TEntity Single(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 根据主键获取实体
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    Task<TEntity> SingleAsync(object Id);

    /// <summary>
    /// 根据条件获取实体
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 获取一个实体
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    TEntity FirstOrDefault(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 获取一个实体
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <returns></returns>
    Task<List<TEntity>> ToListAsync();

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<List<TEntity>> ToListAsync(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <param name="orderByExpression"></param>
    /// <param name="orderByType"></param>
    /// <returns></returns>
    Task<List<TEntity>> ToListAsync(Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<TEntity, object>> orderByExpression, OrderByType orderByType = OrderByType.Asc);

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    List<TEntity> ToList(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <param name="orderByExpression"></param>
    /// <param name="orderByType"></param>
    /// <returns></returns>
    List<TEntity> ToList(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression,
        OrderByType orderByType = OrderByType.Asc);

    /// <summary>
    /// 查询是否存在
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    bool IsExists(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 查询是否存在
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 根据表达式查询多条记录
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    ISugarQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 根据表达式查询多条记录
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    ISugarQueryable<TEntity> Where(bool condition, Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 直接返回数据库结果
    /// </summary>
    /// <returns></returns>
    List<TEntity> AsEnumerable();

    /// <summary>
    /// 直接返回数据库结果
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<TEntity> AsEnumerable(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 直接返回数据库结果
    /// </summary>
    /// <returns></returns>
    Task<List<TEntity>> AsAsyncEnumerable();

    /// <summary>
    /// 直接返回数据库结果
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    Task<List<TEntity>> AsAsyncEnumerable(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 新增一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    int Insert(TEntity entity);

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    int Insert(params TEntity[] entities);

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    int Insert(IEnumerable<TEntity> entities);

    /// <summary>
    /// 新增一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<int> InsertAsync(TEntity entity);

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    Task<int> InsertAsync(params TEntity[] entities);

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    Task<int> InsertAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// 新增一条记录返回自增Id
    /// </summary>
    /// <param name="insertObj"></param>
    /// <returns></returns>
    int InsertReturnIdentity(TEntity insertObj);

    /// <summary>
    /// 新增一条记录返回自增Id
    /// </summary>
    /// <param name="insertObj"></param>
    /// <returns></returns>
    Task<int> InsertReturnIdentityAsync(TEntity insertObj);

    /// <summary>
    /// 新增一条记录返回Long类型的Id
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    long ExecuteReturnBigIdentity(TEntity entity);

    /// <summary>
    /// 新增一条记录返回Long类型的Id
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<long> ExecuteReturnBigIdentityAsync(TEntity entity);

    /// <summary>
    /// 新增一条记录返回新增的数据
    /// </summary>
    /// <param name="insertObj"></param>
    /// <returns></returns>
    TEntity InsertReturnEntity(TEntity insertObj);

    /// <summary>
    /// 新增一条记录返回新增的数据
    /// </summary>
    /// <param name="insertObj"></param>
    /// <returns></returns>
    Task<TEntity> InsertReturnEntityAsync(TEntity insertObj);

    /// <summary>
    /// 更新一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="isNoUpdateNull">是否排除NULL值字段更新</param>
    /// <returns></returns>
    int Update(TEntity entity, bool isNoUpdateNull = false);

    /// <summary>
    /// 更新多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    int Update(params TEntity[] entities);

    /// <summary>
    /// 更新多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    int Update(IEnumerable<TEntity> entities);

    /// <summary>
    /// 更新一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<int> UpdateAsync(TEntity entity);

    /// <summary>
    /// 更新多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    Task<int> UpdateAsync(params TEntity[] entities);

    /// <summary>
    /// 更新多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    Task<int> UpdateAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// 无主键更新一条记录
    /// </summary>
    /// <param name="entity">更新的实体</param>
    /// <param name="columns">根据那些字段更新</param>
    /// <returns></returns>
    Task<int> UpdateNoPrimaryKey(TEntity entity, Expression<Func<TEntity, object>> columns);

    /// <summary>
    /// 无主键更新多条记录
    /// </summary>
    /// <param name="entity">更新的实体</param>
    /// <param name="columns">根据那些字段更新</param>
    /// <returns></returns>
    Task<int> UpdateNoPrimaryKey(List<TEntity> entity, Expression<Func<TEntity, object>> columns);

    /// <summary>
    /// 无主键更新一条记录
    /// </summary>
    /// <param name="entity">更新的实体</param>
    /// <param name="columns">根据那些字段更新</param>
    /// <returns></returns>
    Task<int> UpdateNoPrimaryKeyAsync(TEntity entity, Expression<Func<TEntity, object>> columns);

    /// <summary>
    /// 无主键更新多条记录
    /// </summary>
    /// <param name="entity">更新的实体</param>
    /// <param name="columns">根据那些字段更新</param>
    /// <returns></returns>
    Task<int> UpdateNoPrimaryKeyAsync(List<TEntity> entity, Expression<Func<TEntity, object>> columns);

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    int Delete(TEntity entity);

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int Delete(object key);

    /// <summary>
    /// 删除多条记录
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    int Delete(params object[] keys);

    /// <summary>
    /// 自定义条件删除记录
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    int Delete(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<int> DeleteAsync(TEntity entity);

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<int> DeleteAsync(object key);

    /// <summary>
    /// 删除多条记录
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    Task<int> DeleteAsync(params object[] keys);

    /// <summary>
    /// 自定义条件删除记录
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<int> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 自定义条件逻辑删除记录
    /// <remarks>注意，实体必须继承 <see cref="IBaseDeletedEntity"/></remarks>
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    int LogicDelete(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 自定义条件逻辑删除记录
    /// <remarks>注意，实体必须继承 <see cref="IBaseDeletedEntity"/></remarks>
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    Task<int> LogicDeleteAsync(Expression<Func<TEntity, bool>> whereExpression);
}