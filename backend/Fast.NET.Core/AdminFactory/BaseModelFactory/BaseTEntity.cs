namespace Fast.NET.Core.AdminFactory.BaseModelFactory;

/// <summary>
/// 租户实体基类
/// </summary>
public class BaseTEntity : BaseEntity, IBaseTenant
{
    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id")]
    public virtual long? TenantId { get; set; }
}