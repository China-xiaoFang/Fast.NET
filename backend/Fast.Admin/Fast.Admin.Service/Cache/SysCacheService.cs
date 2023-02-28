using System.Linq.Expressions;
using Fast.Admin.Service.Cache.Dto;
using Fast.Core.AdminFactory.ModelFactory.Sys;
using Fast.Core.AdminFactory.ModelFactory.Tenant;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Service.Cache;

/// <summary>
/// 系统缓存服务
/// </summary>
public class SysCacheService : ISysCacheService, ISingleton
{
    public ICache _cache { get; }
    private readonly ISqlSugarClient _tenant;

    public SysCacheService(ICache cache, ISqlSugarClient tenant)
    {
        _cache = cache;
        _tenant = tenant;
    }

    #region 缓存基础操作

    /// <summary>
    /// 获取缓存类型
    /// </summary>
    /// <returns></returns>
    public string GetCacheType()
    {
        return GlobalContext.CacheOptions.CacheType.GetDescription();
    }

    /// <summary>
    /// 获取所有缓存Key
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllCacheKey()
    {
        return _cache.GetAllKeys();
    }

    /// <summary>
    /// 获取缓存值
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<string> GetCacheValue(string key)
    {
        return await _cache.GetAsync(key);
    }

    /// <summary>
    /// 设置缓存值
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task EditCacheValue(EditCacheValueInput input)
    {
        // 判断Key是否存在
        if (await _cache.ExistsAsync(input.Key))
        {
            await _cache.SetAsync(input.Key, input.Value);
        }
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task DeleteCacheValue(DeleteCacheValueInput input)
    {
        await _cache.DelAsync(input.Key);
    }

    #endregion

    /// <summary>
    /// 获取所有租户信息
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<List<SysTenantModel>> GetAllTenantInfo(Expression<Func<SysTenantModel, bool>> predicate = null)
    {
        // 先从缓存获取
        var tenantList = await _cache.GetAsync<List<SysTenantModel>>(CacheConst.TenantInfo);

        if (tenantList != null && tenantList.Any())
            return predicate == null ? tenantList : tenantList.Where(predicate.Compile()).ToList();

        // 获取租户基本信息
        tenantList = await _tenant.Queryable<SysTenantModel>().Includes(app => app.AppList).Includes(db => db.DataBaseList)
            .ToListAsync();
        // 获取租户两个管理员信息
        // 注：这里如果租户过多的话可能存在卡顿
        foreach (var tenant in tenantList)
        {
            // 加载租户数据库Db
            var _db = _tenant.LoadSqlSugar<TenUserModel>(tenant.Id);
            // 查询两个管理员信息
            var userList = await _db.Queryable<TenUserModel>().Where(wh =>
                (wh.AdminType == AdminTypeEnum.SystemAdmin || wh.AdminType == AdminTypeEnum.TenantAdmin)).ToListAsync();
            tenant.SystemAdminUser = userList.First(f => f.AdminType == AdminTypeEnum.SystemAdmin);
            tenant.TenantAdminUser = userList.First(f => f.AdminType == AdminTypeEnum.TenantAdmin);
        }

        await _cache.SetAsync(CacheConst.TenantInfo, tenantList);

        return predicate == null ? tenantList : tenantList.Where(predicate.Compile()).ToList();
    }
}