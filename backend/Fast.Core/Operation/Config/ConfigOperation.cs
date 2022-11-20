using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.AdminFactory.ModelFactory.Basis;
using Fast.Core.AdminFactory.ModelFactory.Sys;
using Fast.Core.Operation.Config.Dto;
using Fast.Core.SqlSugar.Extension;
using Furion.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Core.Operation.Config;

/// <summary>
/// 配置操作静态类
/// </summary>
public static class ConfigOperation
{
    /// <summary>
    /// 根据配置Code和类型获取配置信息
    /// </summary>
    /// <param name="configCode"></param>
    /// <param name="configType"></param>
    /// <returns></returns>
    public static ConfigInfo GetConfig(string configCode, SysConfigTypeEnum configType = SysConfigTypeEnum.System)
    {
        var result = GetConfig(new GetConfigInput {Code = configCode, ConfigType = configType});
        return result[configCode];
    }

    /// <summary>
    /// 根据配置Code和类型获取配置信息
    /// </summary>
    /// <param name="configInputs"></param>
    /// <returns></returns>
    public static Dictionary<string, ConfigInfo> GetConfig(params GetConfigInput[] configInputs)
    {
        return GetConfigAsync(configInputs).Result;
    }

    /// <summary>
    /// 根据配置Code和类型获取配置信息
    /// </summary>
    /// <param name="configCode"></param>
    /// <param name="configType"></param>
    /// <returns></returns>
    public static async Task<ConfigInfo> GetConfigAsync(string configCode,
        SysConfigTypeEnum configType = SysConfigTypeEnum.System)
    {
        var result = await GetConfigAsync(new GetConfigInput {Code = configCode, ConfigType = configType});
        return result[configCode];
    }

    /// <summary>
    /// 根据配置Code和类型获取配置信息
    /// </summary>
    /// <param name="configInputs"></param>
    /// <returns></returns>
    public static async Task<Dictionary<string, ConfigInfo>> GetConfigAsync(params GetConfigInput[] configInputs)
    {
        // 返回值
        var result = new Dictionary<string, ConfigInfo>();

        await Scoped.CreateAsync(async (_, scope) =>
        {
            var service = scope.ServiceProvider;

            var _cache = service.GetService<ICache>();

            // 从缓存中读取系统配置
            // ReSharper disable once PossibleNullReferenceException
            var cacheSysRes = await _cache.GetAsync<Dictionary<string, ConfigInfo>>(CacheConst.SysConfigInfo);

            // 判断缓存中是否存在
            if (cacheSysRes == null || cacheSysRes.Count == 0)
            {
                var db = service.GetService<ISqlSugarClient>();

                // ReSharper disable once PossibleNullReferenceException
                var _db = db.AsTenant().GetConnection(GlobalContext.ConnectionStringsOptions.DefaultConnectionId);

                // 获取所有数据
                var data = await _db.Queryable<SysConfigModel>().Select<ConfigInfo>().ToListAsync();

                // 组装数据
                cacheSysRes = data.ToDictionary(item => item.Code, item => item.Adapt<ConfigInfo>());

                // 放入缓存
                await _cache.SetAsync(CacheConst.SysConfigInfo, cacheSysRes);
            }

            // 从缓存中读取租户配置
            var cacheTenRes = await _cache.GetAsync<Dictionary<string, ConfigInfo>>(CacheConst.TenConfigInfo);
            // 判断缓存中是否存在
            if (cacheTenRes == null || cacheTenRes.Count == 0)
            {
                try
                {
                    var _db = service.GetService<ISqlSugarClient>().LoadSqlSugar<TenConfigModel>();

                    // 获取所有数据
                    var data = await _db.Queryable<TenConfigModel>().Select<ConfigInfo>().ToListAsync();

                    // 组装数据
                    cacheTenRes = data.ToDictionary(item => item.Code, item => item.Adapt<ConfigInfo>());

                    // 放入缓存
                    await _cache.SetAsync(CacheConst.TenConfigInfo, cacheTenRes);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            // 查找结果
            foreach (var item in configInputs)
            {
                if (item.ConfigType == SysConfigTypeEnum.Tenant)
                {
                    var info = cacheTenRes?[item.Code];
                    if (info != null)
                    {
                        result.Add(item.Code, info);
                    }
                }
                else
                {
                    var info = cacheSysRes[item.Code];
                    result.Add(item.Code, info);
                }
            }
        });

        return result;
    }
}