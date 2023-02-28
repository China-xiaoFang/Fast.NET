using Fast.Admin.Service.Cache.Dto;

namespace Fast.Admin.Service.Cache;

/// <summary>
/// 系统缓存服务接口
/// </summary>
public interface ISysCacheService
{
    public ICache _cache { get; }

    /// <summary>
    /// 获取缓存类型
    /// </summary>
    /// <returns></returns>
    string GetCacheType();

    /// <summary>
    /// 获取所有缓存Key
    /// </summary>
    /// <returns></returns>
    List<string> GetAllCacheKey();

    /// <summary>
    /// 获取缓存值
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<string> GetCacheValue(string key);

    /// <summary>
    /// 设置缓存值
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task EditCacheValue(EditCacheValueInput input);

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task DeleteCacheValue(DeleteCacheValueInput input);
}