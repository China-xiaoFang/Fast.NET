namespace Fast.Core.CodeFirst.Internal;

/// <summary>
/// 系统种子数据接口
/// </summary>
public interface ISystemSeedData
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <param name="_db"></param>
    /// <returns></returns>
    Task SeedData(SqlSugarProvider _db);
}

/// <summary>
/// 租户种子数据接口
/// </summary>
public interface ITenantSeedData
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <param name="_db"></param>
    /// <returns></returns>
    Task SeedData(SqlSugarProvider _db);
}