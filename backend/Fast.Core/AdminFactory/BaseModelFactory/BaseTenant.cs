namespace Fast.Core.AdminFactory.BaseModelFactory;

/// <summary>
/// 租户基类
/// </summary>
public class BaseTenant : IBaseTenant
{
    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id", CreateTableFieldSort = 998)]
    public virtual long? TenantId { get; set; }
}