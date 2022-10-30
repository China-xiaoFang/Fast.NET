/*
 * SqlSugar简单仓储实现类
 *
 * Author: 1.8K仔
 * DateTime：2022-07-24
 *
 */


namespace Fast.SqlSugar.Repository;

/// <summary>
/// 非泛型 SqlSugar 仓储
/// </summary>
public class SqlSugarRepository : ISqlSugarRepository
{
    /// <summary>
    /// 服务提供器
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public SqlSugarRepository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 切换仓储
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>仓储</returns>
    public virtual ISqlSugarRepository<TEntity> Change<TEntity>() where TEntity : class, new()
    {
        return _serviceProvider.GetService<ISqlSugarRepository<TEntity>>();
    }
}

/// <summary>
/// SqlSugar 仓储实现类
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class SqlSugarRepository<TEntity> : ISqlSugarRepository<TEntity> where TEntity : class, new()
{
    /// <summary>
    /// 非泛型 SqlSugar 仓储
    /// </summary>
    private readonly ISqlSugarRepository _sqlSugarRepository;

    /// <summary>
    /// 初始化 SqlSugar 客户端
    /// </summary>
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="sqlSugarRepository"></param>
    /// <param name="db"></param>
    public SqlSugarRepository(ISqlSugarRepository sqlSugarRepository, ISqlSugarClient db)
    {
        _sqlSugarRepository = sqlSugarRepository;
        _db = db.LoadSqlSugar<TEntity>();
        Context = _db;
        Ado = _db.Ado;
    }

    /// <summary>
    /// 实体集合
    /// </summary>
    public virtual ISugarQueryable<TEntity> Entities => _db.Queryable<TEntity>();

    /// <summary>
    /// 数据库上下文
    /// </summary>
    public virtual ISqlSugarClient Context { get; }

    /// <summary>
    /// 原生 Ado 对象
    /// </summary>
    public virtual IAdo Ado { get; }

    #region Function

    /// <summary>
    /// 构建查询分析器
    /// </summary>
    /// <returns></returns>
    public virtual ISugarQueryable<TEntity> AsQueryable()
    {
        return Entities;
    }

    /// <summary>
    /// 构建查询分析器
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public virtual ISugarQueryable<TEntity> AsQueryable(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Entities.Where(whereExpression);
    }

    /// <summary>
    /// 切换仓储/切换租户仓储
    /// </summary>
    /// <typeparam name="TChangeEntity">实体类型</typeparam>
    /// <returns>仓储</returns>
    public virtual ISqlSugarRepository<TChangeEntity> Change<TChangeEntity>() where TChangeEntity : class, new()
    {
        return _sqlSugarRepository.Change<TChangeEntity>();
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
    public virtual ISugarQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        return AsQueryable(predicate);
    }

    /// <summary>
    /// 根据表达式查询多条记录
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual ISugarQueryable<TEntity> Where(bool condition, Expression<Func<TEntity, bool>> predicate)
    {
        return AsQueryable().WhereIF(condition, predicate);
    }

    /// <summary>
    /// 直接返回数据库结果
    /// </summary>
    /// <returns></returns>
    public virtual List<TEntity> AsEnumerable()
    {
        return AsQueryable().ToList();
    }

    /// <summary>
    /// 直接返回数据库结果
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual List<TEntity> AsEnumerable(Expression<Func<TEntity, bool>> predicate)
    {
        return AsQueryable(predicate).ToList();
    }

    /// <summary>
    /// 直接返回数据库结果
    /// </summary>
    /// <returns></returns>
    public virtual Task<List<TEntity>> AsAsyncEnumerable()
    {
        return AsQueryable().ToListAsync();
    }

    /// <summary>
    /// 直接返回数据库结果
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual Task<List<TEntity>> AsAsyncEnumerable(Expression<Func<TEntity, bool>> predicate)
    {
        return AsQueryable(predicate).ToListAsync();
    }

    #endregion

    #region Add

    /// <summary>
    /// 新增一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual int Insert(TEntity entity)
    {
        return _db.Insertable(entity).ExecuteCommand();
    }

    /// <summary>
    /// 新增一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual Task<int> InsertAsync(TEntity entity)
    {
        return _db.Insertable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public virtual int Insert(params TEntity[] entities)
    {
        return _db.Insertable(entities).ExecuteCommand();
    }

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public virtual Task<int> InsertAsync(params TEntity[] entities)
    {
        return _db.Insertable(entities).ExecuteCommandAsync();
    }

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public virtual int Insert(IEnumerable<TEntity> entities)
    {
        return _db.Insertable(entities.ToArray()).ExecuteCommand();
    }

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public virtual Task<int> InsertAsync(IEnumerable<TEntity> entities)
    {
        if (entities != null && entities.Any())
        {
            return _db.Insertable(entities.ToArray()).ExecuteCommandAsync();
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
        return _db.Insertable(insertObj).ExecuteReturnIdentity();
    }

    /// <summary>
    /// 新增一条记录返回自增Id
    /// </summary>
    /// <param name="insertObj"></param>
    /// <returns></returns>
    public Task<int> InsertReturnIdentityAsync(TEntity insertObj)
    {
        return _db.Insertable(insertObj).ExecuteReturnIdentityAsync();
    }

    /// <summary>
    /// 新增一条记录返回Long类型的Id
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public long ExecuteReturnBigIdentity(TEntity entity)
    {
        return _db.Insertable(entity).ExecuteReturnBigIdentity();
    }

    /// <summary>
    /// 新增一条记录返回Long类型的Id
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<long> ExecuteReturnBigIdentityAsync(TEntity entity)
    {
        return await _db.Insertable(entity).ExecuteReturnBigIdentityAsync();
    }

    /// <summary>
    /// 新增一条记录返回新增的数据
    /// </summary>
    /// <param name="insertObj"></param>
    /// <returns></returns>
    public TEntity InsertReturnEntity(TEntity insertObj)
    {
        return _db.Insertable(insertObj).ExecuteReturnEntity();
    }

    /// <summary>
    /// 新增一条记录返回新增的数据
    /// </summary>
    /// <param name="insertObj"></param>
    /// <returns></returns>
    public Task<TEntity> InsertReturnEntityAsync(TEntity insertObj)
    {
        return _db.Insertable(insertObj).ExecuteReturnEntityAsync();
    }

    #endregion

    #region Update

    /// <summary>
    /// 更新一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="isNoUpdateNull">是否排除NULL值字段更新</param>
    /// <returns></returns>
    public virtual int Update(TEntity entity, bool isNoUpdateNull = false)
    {
        return _db.Updateable(entity).IgnoreColumns(isNoUpdateNull).ExecuteCommand();
    }

    /// <summary>
    /// 更新一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual Task<int> UpdateAsync(TEntity entity)
    {
        return _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public virtual int Update(params TEntity[] entities)
    {
        return _db.Updateable(entities).ExecuteCommand();
    }

    /// <summary>
    /// 更新多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public virtual Task<int> UpdateAsync(params TEntity[] entities)
    {
        return _db.Updateable(entities).ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public virtual int Update(IEnumerable<TEntity> entities)
    {
        return _db.Updateable(entities.ToArray()).ExecuteCommand();
    }

    /// <summary>
    /// 更新多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public virtual Task<int> UpdateAsync(IEnumerable<TEntity> entities)
    {
        return _db.Updateable(entities.ToArray()).ExecuteCommandAsync();
    }

    /// <summary>
    /// 无主键更新一条记录
    /// </summary>
    /// <param name="entity">更新的实体</param>
    /// <param name="columns">根据那些字段更新</param>
    /// <returns></returns>
    public virtual Task<int> UpdateNoPrimaryKey(TEntity entity, Expression<Func<TEntity, object>> columns)
    {
        return _db.Updateable(entity).WhereColumns(columns).ExecuteCommandAsync();
    }

    /// <summary>
    /// 无主键更新一条记录
    /// </summary>
    /// <param name="entity">更新的实体</param>
    /// <param name="columns">根据那些字段更新</param>
    /// <returns></returns>
    public virtual Task<int> UpdateNoPrimaryKeyAsync(TEntity entity, Expression<Func<TEntity, object>> columns)
    {
        return _db.Updateable(entity).WhereColumns(columns).ExecuteCommandAsync();
    }

    /// <summary>
    /// 无主键更新多条记录
    /// </summary>
    /// <param name="entity">更新的实体</param>
    /// <param name="columns">根据那些字段更新</param>
    /// <returns></returns>
    public virtual Task<int> UpdateNoPrimaryKey(List<TEntity> entity, Expression<Func<TEntity, object>> columns)
    {
        return _db.Updateable(entity).WhereColumns(columns).ExecuteCommandAsync();
    }

    /// <summary>
    /// 无主键更新多条记录
    /// </summary>
    /// <param name="entity">更新的实体</param>
    /// <param name="columns">根据那些字段更新</param>
    /// <returns></returns>
    public virtual Task<int> UpdateNoPrimaryKeyAsync(List<TEntity> entity, Expression<Func<TEntity, object>> columns)
    {
        return _db.Updateable(entity).WhereColumns(columns).ExecuteCommandAsync();
    }

    #endregion

    #region Delete

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual int Delete(TEntity entity)
    {
        return _db.Deleteable(entity).ExecuteCommand();
    }

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual int Delete(object key)
    {
        return _db.Deleteable<TEntity>().In(key).ExecuteCommand();
    }

    /// <summary>
    /// 删除多条记录
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public virtual int Delete(params object[] keys)
    {
        return _db.Deleteable<TEntity>().In(keys).ExecuteCommand();
    }

    /// <summary>
    /// 自定义条件删除记录
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public int Delete(Expression<Func<TEntity, bool>> whereExpression)
    {
        return _db.Deleteable<TEntity>().Where(whereExpression).ExecuteCommand();
    }

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual Task<int> DeleteAsync(TEntity entity)
    {
        return _db.Deleteable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual Task<int> DeleteAsync(object key)
    {
        return _db.Deleteable<TEntity>().In(key).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除多条记录
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public virtual Task<int> DeleteAsync(params object[] keys)
    {
        return _db.Deleteable<TEntity>().In(keys).ExecuteCommandAsync();
    }

    /// <summary>
    /// 自定义条件删除记录
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        return await _db.Deleteable<TEntity>().Where(whereExpression).ExecuteCommandAsync();
    }

    #endregion
}