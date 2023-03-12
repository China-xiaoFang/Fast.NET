using Fast.SDK.Common.Cache.Options;

namespace Fast.SDK.Common.Cache;

/// <summary>
/// 缓存通用上下文
/// </summary>
public class CacheContext
{
    /// <summary>
    /// 缓存配置
    /// </summary>
    public static CacheOptions CacheOptions { get; set; }
}