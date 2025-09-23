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

// ReSharper disable once CheckNamespace
namespace Fast.SqlSugar;

/// <summary>
/// <see cref="SqlSugarRepository{TEntity}"/> SqlSugar 删除仓储实现
/// </summary>
internal sealed partial class SqlSugarRepository<TEntity>
{
    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public int Delete(TEntity entity)
    {
        var deleteable = Deleteable(entity)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value);

        if (IsSplitTable)
        {
            return deleteable.SplitTable()
                .ExecuteCommand();
        }

        return deleteable.ExecuteCommand();
    }

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public Task<int> DeleteAsync(TEntity entity)
    {
        var deleteable = Deleteable(entity)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value);

        if (IsSplitTable)
        {
            return deleteable.SplitTable()
                .ExecuteCommandAsync();
        }

        return deleteable.ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int Delete(object key)
    {
        var deleteable = Deleteable<TEntity>()
            .In(key)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value);

        if (IsSplitTable)
        {
            return deleteable.SplitTable()
                .ExecuteCommand();
        }

        return deleteable.ExecuteCommand();
    }

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Task<int> DeleteAsync(object key)
    {
        var deleteable = Deleteable<TEntity>()
            .In(key)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value);

        if (IsSplitTable)
        {
            return deleteable.SplitTable()
                .ExecuteCommandAsync();
        }

        return deleteable.ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除多条记录
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public int Delete(params object[] keys)
    {
        var deleteable = Deleteable<TEntity>()
            .In(keys)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value);

        if (IsSplitTable)
        {
            return deleteable.SplitTable()
                .ExecuteCommand();
        }

        return deleteable.ExecuteCommand();
    }

    /// <summary>
    /// 删除多条记录
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public Task<int> DeleteAsync(params object[] keys)
    {
        var deleteable = Deleteable<TEntity>()
            .In(keys)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value);

        if (IsSplitTable)
        {
            return deleteable.SplitTable()
                .ExecuteCommandAsync();
        }

        return deleteable.ExecuteCommandAsync();
    }

    /// <summary>
    /// 自定义条件删除记录
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public int Delete(Expression<Func<TEntity, bool>> whereExpression)
    {
        var deleteable = Deleteable<TEntity>()
            .Where(whereExpression)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value);

        if (IsSplitTable)
        {
            return deleteable.SplitTable()
                .ExecuteCommand();
        }

        return deleteable.ExecuteCommand();
    }

    /// <summary>
    /// 自定义条件删除记录
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public Task<int> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        var deleteable = Deleteable<TEntity>()
            .Where(whereExpression)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value);

        if (IsSplitTable)
        {
            return deleteable.SplitTable()
                .ExecuteCommandAsync();
        }

        return deleteable.ExecuteCommandAsync();
    }

    /// <summary>
    /// 自定义条件逻辑删除记录
    /// </summary>
    /// <remarks>注意，实体必须继承 <see cref="IDeletedEntity"/></remarks>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public int LogicDelete(Expression<Func<TEntity, bool>> whereExpression)
    {
        // 判断是否支持逻辑删除
        if (!SupportsLogicDelete)
            throw new InvalidOperationException(
                $"{nameof(TEntity)} does not inherit {nameof(IDeletedEntity)} interface, Logical deletion cannot be used.");

        // 反射创建实体
        var deletedEntity = Activator.CreateInstance<TEntity>();

        // 获取 IsDeleted 字段属性
        var isDeletedProperty = typeof(TEntity).GetProperty(nameof(IDeletedEntity.IsDeleted));

        // 设置 IsDeleted 字段属性值
        isDeletedProperty!.SetValue(deletedEntity, true);

        // 执行逻辑删除
        var updateable = Updateable<TEntity>()
            .Where(whereExpression)
            .SetColumns(_ => deletedEntity, true)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value);

        if (IsSplitTable)
        {
            return updateable.SplitTable()
                .ExecuteCommand();
        }

        return updateable.ExecuteCommand();
    }

    /// <summary>
    /// 自定义条件逻辑删除记录
    /// </summary>
    /// <remarks>注意，实体必须继承 <see cref="IDeletedEntity"/></remarks>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public Task<int> LogicDeleteAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        // 判断是否支持逻辑删除
        if (!SupportsLogicDelete)
            throw new InvalidOperationException(
                $"{nameof(TEntity)} does not inherit {nameof(IDeletedEntity)} interface, Logical deletion cannot be used.");

        // 反射创建实体
        var deletedEntity = Activator.CreateInstance<TEntity>();

        // 获取 IsDeleted 字段属性
        var isDeletedProperty = typeof(TEntity).GetProperty(nameof(IDeletedEntity.IsDeleted));

        // 设置 IsDeleted 字段属性值
        isDeletedProperty!.SetValue(deletedEntity, true);

        // 执行逻辑删除
        var updateable = Updateable<TEntity>()
            .Where(whereExpression)
            .SetColumns(_ => deletedEntity, true)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value);

        if (IsSplitTable)
        {
            return updateable.SplitTable()
                .ExecuteCommandAsync();
        }

        return updateable.ExecuteCommandAsync();
    }
}