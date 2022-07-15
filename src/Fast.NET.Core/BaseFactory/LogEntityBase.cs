namespace Fast.NET.Core.BaseFactory;

/// <summary>
/// 日志基类
/// </summary>
public class LogEntityBase : AutoIncrementEntity
{
    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id", IsNullable = true)]
    public virtual long? TenantId { get; set; }
}