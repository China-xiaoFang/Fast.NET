using Fast.Admin.Model.BaseModel;
using Fast.Admin.Model.BaseModel.Interface;

namespace Fast.Core.Const;

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
    public const string CreatedTime = nameof(IBaseEntity.CreatedTime);

    /// <summary>
    /// 创建者Id字段
    /// </summary>
    public const string CreatedUserId = nameof(IBaseEntity.CreatedUserId);

    /// <summary>
    /// 创建者名称字段
    /// </summary>
    public const string CreatedUserName = nameof(IBaseEntity.CreatedUserName);

    /// <summary>
    /// 更新时间字段
    /// </summary>
    public const string UpdatedTime = nameof(IBaseEntity.UpdatedTime);

    /// <summary>
    /// 更新者Id字段
    /// </summary>
    public const string UpdatedUserId = nameof(IBaseEntity.UpdatedUserId);

    /// <summary>
    /// 更新者名称字段
    /// </summary>
    public const string UpdatedUserName = nameof(IBaseEntity.UpdatedUserName);

    /// <summary>
    /// 更新版本控制字段
    /// </summary>
    public const string UpdatedVersion = nameof(IBaseEntity.UpdatedVersion);

    /// <summary>
    /// 软删除字段
    /// </summary>
    public const string IsDeleted = nameof(IBaseDeleted.IsDeleted);

    /// <summary>
    /// 租户Id字段
    /// </summary>
    public const string TenantId = nameof(IBaseTenant.TenantId);
}