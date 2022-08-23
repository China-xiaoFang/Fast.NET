namespace Fast.NET.Core.AdminFactory.BaseModelFactory;

/// <summary>
/// BaseTEntity 接口定义类
/// </summary>
public interface IBaseTenant : IDbEntity
{
    /// <summary>
    /// 租户Id
    /// </summary>
    long? TenantId { get; set; }
}