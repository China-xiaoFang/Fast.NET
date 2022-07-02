namespace Hello.NET.Core.BaseFactory;

/// <summary>
/// 自定义租户基类实体
/// </summary>
public abstract class DBEntityTenant : DEntityBase
{
    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id", IsNullable = true)]
    public virtual long? TenantId { get; set; }

    /// <summary>
    /// 创建
    /// </summary>
    public override void Create()
    {
        var userId = App.User.FindFirst(ClaimConst.CLAINM_USERID)?.Value;
        var userName = App.User.FindFirst(ClaimConst.CLAINM_ACCOUNT)?.Value;
        var tenantId = App.User.FindFirst(ClaimConst.TENANT_ID)?.Value;
        Id = YitIdHelper.NextId();
        CreatedTime = DateTime.Now;

        if (!string.IsNullOrEmpty(userId))
        {
            CreatedUserId = userId.ParseToLong();
            CreatedUserName = userName;
        }

        if (string.IsNullOrWhiteSpace(tenantId))
            return;
        TenantId = tenantId.ParseToLong();
        if (!string.IsNullOrEmpty(userId))
            return;
        CreatedUserId = TenantId;
        CreatedUserName = App.User.FindFirst(ClaimConst.TENANT_NAME)?.Value;
    }

    public override void Modify()
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

    public void CreateNotFillTenantInfo()
    {
        base.Create();
    }
}