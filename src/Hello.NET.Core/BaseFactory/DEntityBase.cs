namespace Hello.NET.Core.BaseFactory;

/// <summary>
/// 自定义实体基类
/// </summary>
public abstract class DEntityBase : PrimaryKeyEntity
{
    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间", ColumnDataType = "datetimeoffset", IsNullable = true)]
    public virtual DateTime? CreatedTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间", ColumnDataType = "datetimeoffset", IsNullable = true)]
    public virtual DateTime? UpdatedTime { get; set; }

    /// <summary>
    /// 创建者Id
    /// </summary>
    [SugarColumn(ColumnDescription = "创建者Id", IsNullable = true)]
    public virtual long? CreatedUserId { get; set; }

    /// <summary>
    /// 创建者名称
    /// </summary>
    [SugarColumn(ColumnDescription = "创建者名称", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public virtual string CreatedUserName { get; set; }

    /// <summary>
    /// 修改者Id
    /// </summary>
    [SugarColumn(ColumnDescription = "修改者Id", IsNullable = true)]
    public virtual long? UpdatedUserId { get; set; }

    /// <summary>
    /// 修改者名称
    /// </summary>
    [SugarColumn(ColumnDescription = "修改者名称", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public virtual string UpdatedUserName { get; set; }

    /// <summary>
    /// 软删除
    /// </summary>
    [SugarColumn(ColumnDescription = "软删除", IsNullable = false)]
    public virtual bool IsDeleted { get; set; }

    /// <summary>
    /// 创建
    /// </summary>
    public virtual void Create()
    {
        var userId = App.User.FindFirst(ClaimConst.CLAINM_USERID)?.Value;
        var userName = App.User.FindFirst(ClaimConst.CLAINM_ACCOUNT)?.Value;
        Id = YitIdHelper.NextId();
        CreatedTime = DateTime.Now;

        if (!string.IsNullOrEmpty(userId))
        {
            CreatedUserId = userId.ParseToLong();
            CreatedUserName = userName;
        }

        if (!string.IsNullOrEmpty(userId))
            return;

        CreatedUserName = App.User.FindFirst(ClaimConst.TENANT_NAME)?.Value;
    }

    /// <summary>
    /// 更新
    /// </summary>
    public virtual void Modify()
    {
        var userId = App.User.FindFirst(ClaimConst.CLAINM_USERID)?.Value;
        var userName = App.User.FindFirst(ClaimConst.CLAINM_ACCOUNT)?.Value;
        UpdatedTime = DateTime.Now;
        if (!string.IsNullOrEmpty(userId))
        {
            UpdatedUserId = userId.ParseToLong();
            UpdatedUserName = userName;
        }
        else
        {
            var tenantId = App.User.FindFirst(ClaimConst.TENANT_ID)?.Value;
            if (string.IsNullOrEmpty(tenantId))
                return;
            UpdatedUserId = tenantId.ParseToLong();
            UpdatedUserName = App.User.FindFirst(ClaimConst.TENANT_NAME)?.Value;
        }
    }

    /// <summary>
    /// 更新列，多用于只更新一个字段使用
    /// </summary>
    /// <returns></returns>
    public virtual string[] UpdateColumn()
    {
        var result = new[] {nameof(UpdatedUserId), nameof(UpdatedUserName), nameof(UpdatedTime)};
        return result;
    }

    /// <summary>
    /// 假删除列
    /// </summary>
    /// <returns></returns>
    public virtual string[] FalseDeleteColumn()
    {
        var updateColumn = UpdateColumn();
        var deleteColumn = new[] {nameof(IsDeleted)};
        var result = new string [updateColumn.Length + deleteColumn.Length];
        deleteColumn.CopyTo(result, 0);
        updateColumn.CopyTo(result, deleteColumn.Length);
        return result;
    }
}