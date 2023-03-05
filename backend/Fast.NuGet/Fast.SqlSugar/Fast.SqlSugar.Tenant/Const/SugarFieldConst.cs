using Fast.SqlSugar.Tenant.BaseModel;

namespace Fast.SqlSugar.Tenant.Const;

/// <summary>
/// 字段常量
/// </summary>
public class SugarFieldConst
{
    /// <summary>
    /// Id字段
    /// </summary>
    public const string Id = nameof(BaseTEntity.Id);

    /// <summary>
    /// 创建时间字段
    /// </summary>
    public const string CreatedTime = nameof(BaseTEntity.CreatedTime);

    /// <summary>
    /// 创建者Id字段
    /// </summary>
    public const string CreatedUserId = nameof(BaseTEntity.CreatedUserId);

    /// <summary>
    /// 创建者名称字段
    /// </summary>
    public const string CreatedUserName = nameof(BaseTEntity.CreatedUserName);

    /// <summary>
    /// 更新时间字段
    /// </summary>
    public const string UpdatedTime = nameof(BaseTEntity.UpdatedTime);

    /// <summary>
    /// 更新者Id字段
    /// </summary>
    public const string UpdatedUserId = nameof(BaseTEntity.UpdatedUserId);

    /// <summary>
    /// 更新者名称字段
    /// </summary>
    public const string UpdatedUserName = nameof(BaseTEntity.UpdatedUserName);

    /// <summary>
    /// 更新版本控制字段
    /// </summary>
    public const string UpdatedVersion = nameof(BaseTEntity.UpdatedVersion);

    /// <summary>
    /// 软删除字段
    /// </summary>
    public const string IsDeleted = nameof(BaseTEntity.IsDeleted);

    /// <summary>
    /// 租户Id字段
    /// </summary>
    public const string TenantId = nameof(BaseTEntity.TenantId);
}