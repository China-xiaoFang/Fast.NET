namespace Fast.Core.ServiceCollection.Cache.Enum;

/// <summary>
/// 缓存类型枚举
/// </summary>
[FastEnum("缓存类型枚举")]
public enum CacheTypeEnum
{
    /// <summary>
    /// 内存缓存
    /// </summary>
    MemoryCache,

    /// <summary>
    /// Redis缓存
    /// </summary>
    RedisCache
}