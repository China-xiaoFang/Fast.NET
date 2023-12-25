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
using Fast.SqlSugar.Filters;
using Fast.SqlSugar.Handlers;
using Fast.SqlSugar.IBaseEntities;
using Fast.SqlSugar.Options;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace Fast.SqlSugar.Repository;

/// <summary>
/// <see cref="SqlSugarRepository{TEntity}"/> SqlSugar仓储实现
/// </summary>
public sealed class SqlSugarRepository<TEntity> : SqlSugarClient, ISqlSugarRepository<TEntity> where TEntity : class, new()
{
    /// <summary>
    /// 服务提供器
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// <see cref="SqlSugarRepository{TEntity}"/> SqlSugar仓储实现
    /// </summary>
    /// <param name="serviceProvider"></param>
    public SqlSugarRepository(IServiceProvider serviceProvider) : base(SqlSugarContext.DefaultConnectionConfig)
    {
        _serviceProvider = serviceProvider;

        // 根据 TEntity 加载对应的数据库连接字符串
        var sqlSugarEntityHandler = _serviceProvider.GetService<ISqlSugarEntityHandler>();

        // 获取新的连接字符串
        var connectionSettings = sqlSugarEntityHandler?.GetConnectionSettings<TEntity>(Context).Result;
        if (connectionSettings != null)
        {
            DataBaseInfo = connectionSettings;
            if (connectionSettings.ConnectionId != (string) SqlSugarContext.DefaultConnectionConfig.ConfigId)
            {
                var newConnectionConfig = SqlSugarContext.GetConnectionConfig(connectionSettings);

                // 重新初始化Context
                InitContext(newConnectionConfig);

                // 执行超时时间
                Context.Ado.CommandTimeOut = connectionSettings.CommandTimeOut;

                // 判断是否禁用 Aop
                if (!connectionSettings.DisableAop)
                {
                    // Aop
                    SugarEntityFilter.LoadSugarAop(Context, connectionSettings.SugarSqlExecMaxSeconds, connectionSettings.DiffLog,
                        sqlSugarEntityHandler);
                }

                // 过滤器
                SugarEntityFilter.LoadSugarFilter(Context, sqlSugarEntityHandler);
            }
        }
        else
        {
            DataBaseInfo = SqlSugarContext.ConnectionSettings;
        }
    }

    /// <summary>
    /// 实体集合
    /// </summary>
    private ISugarQueryable<TEntity> Entities => Queryable<TEntity>();

    /// <summary>
    /// 当前仓储的数据库信息
    /// </summary>
    public ConnectionSettingsOptions DataBaseInfo { get; set; }

    #region Function

    /// <summary>
    /// 构建查询分析器
    /// </summary>
    /// <returns></returns>
    public ISugarQueryable<TEntity> AsQueryable()
    {
        return Entities;
    }

    /// <summary>
    /// 构建查询分析器
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public ISugarQueryable<TEntity> AsQueryable(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Entities.Where(whereExpression);
    }

    /// <summary>
    /// 切换仓储/切换租户仓储
    /// </summary>
    /// <typeparam name="TChangeEntity">实体类型</typeparam>
    /// <returns>仓储</returns>
    public ISqlSugarRepository<TChangeEntity> Change<TChangeEntity>() where TChangeEntity : class, new()
    {
        return _serviceProvider.GetService(typeof(ISqlSugarRepository<TChangeEntity>)) as ISqlSugarRepository<TChangeEntity>;
    }

    #endregion

    #region Select

    /// <summary>
    /// 获取总数
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public int Count(Expression<Func<TEntity, bool>> whereExpression = null)
    {
        return Entities.WhereIF(whereExpression != null, whereExpression).Count();
    }

    /// <summary>
    /// 获取总数
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public Task<int> CountAsync(Expression<Func<TEntity, bool>> whereExpression = null)
    {
        return Entities.WhereIF(whereExpression != null, whereExpression).CountAsync();
    }

    /// <summary>
    /// 检查是否存在
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public bool Any(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Entities.Any(whereExpression);
    }

    /// <summary>
    /// 检查是否存在
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        return await Entities.AnyAsync(whereExpression);
    }

    /// <summary>
    /// 根据主键获取实体
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public TEntity Single(object Id)
    {
        return Entities.InSingle(Id);
    }

    /// <summary>
    /// 根据主键获取实体
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public Task<TEntity> SingleAsync(object Id)
    {
        return Entities.InSingleAsync(Id);
    }

    /// <summary>
    /// 根据条件获取实体
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public TEntity Single(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Entities.Single(whereExpression);
    }

    /// <summary>
    /// 根据条件获取实体
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Entities.SingleAsync(whereExpression);
    }

    /// <summary>
    /// 获取一个实体
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Entities.First(whereExpression);
    }

    /// <summary>
    /// 获取一个实体
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        return await Entities.FirstAsync(whereExpression);
    }

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <returns></returns>
    public Task<List<TEntity>> ToListAsync()
    {
        return Entities.ToListAsync();
    }

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public List<TEntity> ToList(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Entities.Where(whereExpression).ToList();
    }

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public Task<List<TEntity>> ToListAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Entities.Where(whereExpression).ToListAsync();
    }

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <param name="orderByExpression"></param>
    /// <param name="orderByType"></param>
    /// <returns></returns>
    public List<TEntity> ToList(Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<TEntity, object>> orderByExpression, OrderByType orderByType = OrderByType.Asc)
    {
        return Entities.Where(whereExpression).OrderBy(orderByExpression, orderByType).ToList();
    }

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <param name="orderByExpression"></param>
    /// <param name="orderByType"></param>
    /// <returns></returns>
    public Task<List<TEntity>> ToListAsync(Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<TEntity, object>> orderByExpression, OrderByType orderByType = OrderByType.Asc)
    {
        return Entities.Where(whereExpression).OrderBy(orderByExpression, orderByType).ToListAsync();
    }

    /// <summary>
    /// 查询是否存在
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public bool IsExists(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Entities.Any(whereExpression);
    }

    /// <summary>
    /// 查询是否存在
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Entities.AnyAsync(whereExpression);
    }

    /// <summary>
    /// 根据表达式查询多条记录
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public ISugarQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        return AsQueryable(predicate);
    }

    /// <summary>
    /// 根据表达式查询多条记录
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public ISugarQueryable<TEntity> Where(bool condition, Expression<Func<TEntity, bool>> predicate)
    {
        return AsQueryable().WhereIF(condition, predicate);
    }

    #endregion

    #region Add

    /// <summary>
    /// 新增一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public int Insert(TEntity entity)
    {
        return Insertable(entity).ExecuteCommand();
    }

    /// <summary>
    /// 新增一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public Task<int> InsertAsync(TEntity entity)
    {
        return Insertable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public int Insert(params TEntity[] entities)
    {
        return Insertable(entities).ExecuteCommand();
    }

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public Task<int> InsertAsync(params TEntity[] entities)
    {
        return Insertable(entities).ExecuteCommandAsync();
    }

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public int Insert(IEnumerable<TEntity> entities)
    {
        return Insertable(entities.ToArray()).ExecuteCommand();
    }

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public Task<int> InsertAsync(IEnumerable<TEntity> entities)
    {
        if (entities != null && entities.Any())
        {
            return Insertable(entities.ToArray()).ExecuteCommandAsync();
        }

        return Task.FromResult(0);
    }

    /// <summary>
    /// 新增一条记录返回自增Id
    /// </summary>
    /// <param name="insertObj"></param>
    /// <returns></returns>
    public int InsertReturnIdentity(TEntity insertObj)
    {
        return Insertable(insertObj).ExecuteReturnIdentity();
    }

    /// <summary>
    /// 新增一条记录返回自增Id
    /// </summary>
    /// <param name="insertObj"></param>
    /// <returns></returns>
    public Task<int> InsertReturnIdentityAsync(TEntity insertObj)
    {
        return Insertable(insertObj).ExecuteReturnIdentityAsync();
    }

    /// <summary>
    /// 新增一条记录返回Long类型的Id
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public long ExecuteReturnBigIdentity(TEntity entity)
    {
        return Insertable(entity).ExecuteReturnBigIdentity();
    }

    /// <summary>
    /// 新增一条记录返回Long类型的Id
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<long> ExecuteReturnBigIdentityAsync(TEntity entity)
    {
        return await Insertable(entity).ExecuteReturnBigIdentityAsync();
    }

    /// <summary>
    /// 新增一条记录返回新增的数据
    /// </summary>
    /// <param name="insertObj"></param>
    /// <returns></returns>
    public TEntity InsertReturnEntity(TEntity insertObj)
    {
        return Insertable(insertObj).ExecuteReturnEntity();
    }

    /// <summary>
    /// 新增一条记录返回新增的数据
    /// </summary>
    /// <param name="insertObj"></param>
    /// <returns></returns>
    public Task<TEntity> InsertReturnEntityAsync(TEntity insertObj)
    {
        return Insertable(insertObj).ExecuteReturnEntityAsync();
    }

    #endregion

    #region Update

    /// <summary>
    /// 更新一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="isNoUpdateNull">是否排除NULL值字段更新</param>
    /// <returns></returns>
    public int Update(TEntity entity, bool isNoUpdateNull = false)
    {
        return Updateable(entity).IgnoreColumns(isNoUpdateNull).ExecuteCommand();
    }

    /// <summary>
    /// 更新一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public Task<int> UpdateAsync(TEntity entity)
    {
        return Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public int Update(params TEntity[] entities)
    {
        return Updateable(entities).ExecuteCommand();
    }

    /// <summary>
    /// 更新多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public Task<int> UpdateAsync(params TEntity[] entities)
    {
        return Updateable(entities).ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public int Update(IEnumerable<TEntity> entities)
    {
        return Updateable(entities.ToArray()).ExecuteCommand();
    }

    /// <summary>
    /// 更新多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public Task<int> UpdateAsync(IEnumerable<TEntity> entities)
    {
        return Updateable(entities.ToArray()).ExecuteCommandAsync();
    }

    /// <summary>
    /// 无主键更新一条记录
    /// </summary>
    /// <param name="entity">更新的实体</param>
    /// <param name="columns">根据那些字段更新</param>
    /// <returns></returns>
    public Task<int> UpdateNoPrimaryKey(TEntity entity, Expression<Func<TEntity, object>> columns)
    {
        return Updateable(entity).WhereColumns(columns).ExecuteCommandAsync();
    }

    /// <summary>
    /// 无主键更新一条记录
    /// </summary>
    /// <param name="entity">更新的实体</param>
    /// <param name="columns">根据那些字段更新</param>
    /// <returns></returns>
    public Task<int> UpdateNoPrimaryKeyAsync(TEntity entity, Expression<Func<TEntity, object>> columns)
    {
        return Updateable(entity).WhereColumns(columns).ExecuteCommandAsync();
    }

    /// <summary>
    /// 无主键更新多条记录
    /// </summary>
    /// <param name="entity">更新的实体</param>
    /// <param name="columns">根据那些字段更新</param>
    /// <returns></returns>
    public Task<int> UpdateNoPrimaryKey(List<TEntity> entity, Expression<Func<TEntity, object>> columns)
    {
        return Updateable(entity).WhereColumns(columns).ExecuteCommandAsync();
    }

    /// <summary>
    /// 无主键更新多条记录
    /// </summary>
    /// <param name="entity">更新的实体</param>
    /// <param name="columns">根据那些字段更新</param>
    /// <returns></returns>
    public Task<int> UpdateNoPrimaryKeyAsync(List<TEntity> entity, Expression<Func<TEntity, object>> columns)
    {
        return Updateable(entity).WhereColumns(columns).ExecuteCommandAsync();
    }

    #endregion

    #region Delete

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public int Delete(TEntity entity)
    {
        return Deleteable(entity).ExecuteCommand();
    }

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int Delete(object key)
    {
        return Deleteable<TEntity>().In(key).ExecuteCommand();
    }

    /// <summary>
    /// 删除多条记录
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public int Delete(params object[] keys)
    {
        return Deleteable<TEntity>().In(keys).ExecuteCommand();
    }

    /// <summary>
    /// 自定义条件删除记录
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public int Delete(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Deleteable<TEntity>().Where(whereExpression).ExecuteCommand();
    }

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public Task<int> DeleteAsync(TEntity entity)
    {
        return Deleteable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Task<int> DeleteAsync(object key)
    {
        return Deleteable<TEntity>().In(key).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除多条记录
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public Task<int> DeleteAsync(params object[] keys)
    {
        return Deleteable<TEntity>().In(keys).ExecuteCommandAsync();
    }

    /// <summary>
    /// 自定义条件删除记录
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        return await Deleteable<TEntity>().Where(whereExpression).ExecuteCommandAsync();
    }

    /// <summary>
    /// 自定义条件逻辑删除记录
    /// <remarks>注意，实体必须继承 <see cref="IBaseDeletedEntity"/></remarks>
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public int LogicDelete(Expression<Func<TEntity, bool>> whereExpression)
    {
        // 获取 TEntity 的类型
        var entityType = typeof(TEntity);

        // 判断是否继承了 IBaseDeletedEntity
        if (!entityType.GetInterfaces().Contains(typeof(IBaseDeletedEntity)))
            throw new InvalidOperationException(
                $"{nameof(TEntity)} does not inherit {nameof(IBaseDeletedEntity)} interface, Logical deletion cannot be used.");

        // 反射创建实体
        var deletedEntity = Activator.CreateInstance<TEntity>();

        // 获取 IsDeleted 字段属性
        var isDeletedProperty = entityType.GetProperty(nameof(IBaseDeletedEntity.IsDeleted));

        // 设置 IsDeleted 字段属性值
        isDeletedProperty!.SetValue(deletedEntity, true);

        // 执行逻辑删除
        return Updateable<TEntity>().SetColumns(_ => deletedEntity, true).Where(whereExpression).ExecuteCommand();
    }

    /// <summary>
    /// 自定义条件逻辑删除记录
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <remarks>注意，实体必须继承 <see cref="IBaseDeletedEntity"/></remarks>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<int> LogicDeleteAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        // 获取 TEntity 的类型
        var entityType = typeof(TEntity);

        // 判断是否继承了 IBaseDeletedEntity
        if (!entityType.GetInterfaces().Contains(typeof(IBaseDeletedEntity)))
            throw new InvalidOperationException(
                $"{nameof(TEntity)} does not inherit {nameof(IBaseDeletedEntity)} interface, Logical deletion cannot be used.");

        // 反射创建实体
        var deletedEntity = Activator.CreateInstance<TEntity>();

        // 获取 IsDeleted 字段属性
        var isDeletedProperty = entityType.GetProperty(nameof(IBaseDeletedEntity.IsDeleted));

        // 设置 IsDeleted 字段属性值
        isDeletedProperty!.SetValue(deletedEntity, true);

        // 执行逻辑删除
        return await Updateable<TEntity>().SetColumns(_ => deletedEntity, true).Where(whereExpression).ExecuteCommandAsync();
    }

    #endregion
}