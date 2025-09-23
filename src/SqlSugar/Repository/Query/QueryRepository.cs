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

// ReSharper disable once CheckNamespace
namespace Fast.SqlSugar;

/// <summary>
/// <see cref="SqlSugarRepository{TEntity}"/> SqlSugar 查询仓储实现
/// </summary>
internal sealed partial class SqlSugarRepository<TEntity>
{
    /// <summary>
    /// 获取总数
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public int Count(Expression<Func<TEntity, bool>> whereExpression = null)
    {
        return Entities.WhereIF(whereExpression != null, whereExpression)
            .Count();
    }

    /// <summary>
    /// 获取总数
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public Task<int> CountAsync(Expression<Func<TEntity, bool>> whereExpression = null)
    {
        return Entities.WhereIF(whereExpression != null, whereExpression)
            .CountAsync();
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
    public TEntity SingleOrDefault(object Id)
    {
        return Entities.InSingle(Id);
    }

    /// <summary>
    /// 根据主键获取实体
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public Task<TEntity> SingleOrDefaultAsync(object Id)
    {
        return Entities.InSingleAsync(Id);
    }

    /// <summary>
    /// 根据条件获取实体
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Entities.Single(whereExpression);
    }

    /// <summary>
    /// 根据条件获取实体
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> whereExpression)
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
    public List<TEntity> ToList()
    {
        return Entities.ToList();
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
        return Entities.Where(whereExpression)
            .ToList();
    }

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public Task<List<TEntity>> ToListAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Entities.Where(whereExpression)
            .ToListAsync();
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
        return Entities.Where(whereExpression)
            .OrderBy(orderByExpression, orderByType)
            .ToList();
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
        return Entities.Where(whereExpression)
            .OrderBy(orderByExpression, orderByType)
            .ToListAsync();
    }
}