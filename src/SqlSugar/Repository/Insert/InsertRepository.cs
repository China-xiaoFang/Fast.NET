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

// ReSharper disable once CheckNamespace

namespace Fast.SqlSugar;

/// <summary>
/// <see cref="SqlSugarRepository{TEntity}"/> SqlSugar 插入仓储实现
/// </summary>
internal sealed partial class SqlSugarRepository<TEntity>
{
    /// <summary>
    /// 新增一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public int Insert(TEntity entity)
    {
        var insertable = Insertable(entity)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value);

        if (IsSplitTable)
        {
            return insertable.SplitTable()
                .ExecuteCommand();
        }

        return insertable.ExecuteCommand();
    }

    /// <summary>
    /// 新增一条记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public Task<int> InsertAsync(TEntity entity)
    {
        var insertable = Insertable(entity)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value);

        if (IsSplitTable)
        {
            return insertable.SplitTable()
                .ExecuteCommandAsync();
        }

        return insertable.ExecuteCommandAsync();
    }

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public int Insert(params TEntity[] entities)
    {
        var insertable = Insertable(entities)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value);

        if (IsSplitTable)
        {
            return insertable.SplitTable()
                .ExecuteCommand();
        }

        return insertable.ExecuteCommand();
    }

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public Task<int> InsertAsync(params TEntity[] entities)
    {
        var insertable = Insertable(entities)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value);

        if (IsSplitTable)
        {
            return insertable.SplitTable()
                .ExecuteCommandAsync();
        }

        return insertable.ExecuteCommandAsync();
    }

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public int Insert(IEnumerable<TEntity> entities)
    {
        var _entities = entities?.ToArray();
        if (_entities?.Length > 0)
        {
            var insertable = Insertable(_entities.ToArray())
                .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value);

            if (IsSplitTable)
            {
                return insertable.SplitTable()
                    .ExecuteCommand();
            }

            return insertable.ExecuteCommand();
        }

        return 0;
    }

    /// <summary>
    /// 新增多条记录
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public Task<int> InsertAsync(IEnumerable<TEntity> entities)
    {
        var _entities = entities?.ToArray();
        if (_entities?.Length > 0)
        {
            var insertable = Insertable(_entities.ToArray())
                .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value);

            if (IsSplitTable)
            {
                return insertable.SplitTable()
                    .ExecuteCommandAsync();
            }

            return insertable.ExecuteCommandAsync();
        }

        return Task.FromResult(0);
    }

    /// <summary>
    /// 新增一条记录返回自增Id
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public int InsertReturnIdentity(TEntity entity)
    {
        return Insertable(entity)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value)
            .ExecuteReturnIdentity();
    }

    /// <summary>
    /// 新增一条记录返回自增Id
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public Task<int> InsertReturnIdentityAsync(TEntity entity)
    {
        return Insertable(entity)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value)
            .ExecuteReturnIdentityAsync();
    }

    /// <summary>
    /// 新增一条记录返回Long类型的自增Id
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public long ExecuteReturnBigIdentity(TEntity entity)
    {
        return Insertable(entity)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value)
            .ExecuteReturnBigIdentity();
    }

    /// <summary>
    /// 新增一条记录返回Long类型的自增Id
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public Task<long> ExecuteReturnBigIdentityAsync(TEntity entity)
    {
        return Insertable(entity)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value)
            .ExecuteReturnBigIdentityAsync();
    }

    /// <summary>
    /// 新增一条记录返回新增的数据
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public TEntity InsertReturnEntity(TEntity entity)
    {
        return Insertable(entity)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value)
            .ExecuteReturnEntity();
    }

    /// <summary>
    /// 新增一条记录返回新增的数据
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public Task<TEntity> InsertReturnEntityAsync(TEntity entity)
    {
        return Insertable(entity)
            .EnableDiffLogEventIF(DatabaseInfo.DiffLog!.Value)
            .ExecuteReturnEntityAsync();
    }
}