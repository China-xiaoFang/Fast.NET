using System.ComponentModel;

namespace Fast.Admin.Model.Enum;

/// <summary>
/// 缓存类型枚举
/// </summary>
[FastEnum("缓存类型枚举")]
public enum CacheTypeEnum
{
    /// <summary>
    /// 内存缓存
    /// </summary>
    [Description("内存缓存")]
    MemoryCache,

    /// <summary>
    /// Redis缓存
    /// </summary>
    [Description("Redis缓存")]
    RedisCache
}