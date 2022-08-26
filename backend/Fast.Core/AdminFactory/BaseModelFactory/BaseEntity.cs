namespace Fast.Core.AdminFactory.BaseModelFactory;

/// <summary>
/// 实体基类
/// </summary>
public class BaseEntity : PrimaryKeyEntity, IBaseEntity
{
    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public virtual DateTime? CreatedTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间")]
    public virtual DateTime? UpdatedTime { get; set; }

    /// <summary>
    /// 创建者Id
    /// </summary>
    [SugarColumn(ColumnDescription = "创建者Id")]
    public virtual long? CreatedUserId { get; set; }

    /// <summary>
    /// 创建者名称
    /// </summary>
    [SugarColumn(ColumnDescription = "创建者名称", Length = 20)]
    public virtual string CreatedUserName { get; set; }

    /// <summary>
    /// 更新者Id
    /// </summary>
    [SugarColumn(ColumnDescription = "修改者Id")]
    public virtual long? UpdatedUserId { get; set; }

    /// <summary>
    /// 更新者名称
    /// </summary>
    [SugarColumn(ColumnDescription = "修改者名称", Length = 20)]
    public virtual string UpdatedUserName { get; set; }

    /// <summary>
    /// 软删除
    /// </summary>
    [SugarColumn(ColumnDescription = "软删除")]
    public virtual bool IsDeleted { get; set; }

    /// <summary>
    /// 更新信息列
    /// </summary>
    /// <returns></returns>
    public string[] UpdateColumn()
    {
        var result = new[] {nameof(UpdatedUserId), nameof(UpdatedUserName), nameof(UpdatedTime)};
        return result;
    }

    /// <summary>
    /// 假删除的列，包含更新信息
    /// </summary>
    /// <returns></returns>
    public string[] FalseDeleteColumn()
    {
        var updateColumn = UpdateColumn();
        var deleteColumn = new[] {nameof(IsDeleted)};
        var result = new string [updateColumn.Length + deleteColumn.Length];
        deleteColumn.CopyTo(result, 0);
        updateColumn.CopyTo(result, deleteColumn.Length);
        return result;
    }
}