using Fast.Admin.Model.Model.Sys.Config;
using Fast.Admin.Model.Model.Tenant.Config;
using Fast.Core.Cache;
using Fast.Core.Const;
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
    /// 系统配置
    /// </summary>
    public static class System
    {
        /// <summary>
        /// 根据配置Code和类型获取配置信息
        /// </summary>
        /// <param name="configCode"></param>
        /// <returns></returns>
        public static ConfigInfo GetConfig(string configCode)
        {
            var result = GetConfig(new[] {configCode});
            return result[configCode];
        }

        /// <summary>
        /// 根据配置Code和类型获取配置信息
        /// </summary>
        /// <param name="configInputs"></param>
        /// <returns></returns>
        public static Dictionary<string, ConfigInfo> GetConfig(params string[] configInputs)
        {
            return GetConfigAsync(configInputs).Result;
        }

        /// <summary>
        /// 根据配置Code和类型获取配置信息
        /// </summary>
        /// <param name="configCode"></param>
        /// <returns></returns>
        public static async Task<ConfigInfo> GetConfigAsync(string configCode)
        {
            var result = await GetConfigAsync(new[] {configCode});
            return result[configCode];
        }

        /// <summary>
        /// 根据配置Code和类型获取配置信息
        /// </summary>
        /// <param name="configInputs"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, ConfigInfo>> GetConfigAsync(params string[] configInputs)
        {
            return await ConfigOperation.GetConfigAsync(true, configInputs);
        }
    }

    /// <summary>
    /// 租户配置
    /// 注：这里必须是登陆后或者存在Token的情况下才能使用
    /// </summary>
    public static class Tenant
    {
        /// <summary>
        /// 根据配置Code和类型获取配置信息
        /// </summary>
        /// <param name="configCode"></param>
        /// <returns></returns>
        public static ConfigInfo GetConfig(string configCode)
        {
            var result = GetConfig(new[] {configCode});
            return result[configCode];
        }

        /// <summary>
        /// 根据配置Code和类型获取配置信息
        /// </summary>
        /// <param name="configInputs"></param>
        /// <returns></returns>
        public static Dictionary<string, ConfigInfo> GetConfig(params string[] configInputs)
        {
            return GetConfigAsync(configInputs).Result;
        }

        /// <summary>
        /// 根据配置Code和类型获取配置信息
        /// </summary>
        /// <param name="configCode"></param>
        /// <returns></returns>
        public static async Task<ConfigInfo> GetConfigAsync(string configCode)
        {
            var result = await GetConfigAsync(new[] {configCode});
            return result[configCode];
        }

        /// <summary>
        /// 根据配置Code和类型获取配置信息
        /// </summary>
        /// <param name="configInputs"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, ConfigInfo>> GetConfigAsync(params string[] configInputs)
        {
            return await ConfigOperation.GetConfigAsync(false, configInputs);
        }
    }

    /// <summary>
    /// 根据配置Code和类型获取配置信息
    /// </summary>
    /// <param name="isSystem"></param>
    /// <param name="configInputs"></param>
    /// <returns></returns>
    private static async Task<Dictionary<string, ConfigInfo>> GetConfigAsync(bool isSystem, params string[] configInputs)
    {
        // 返回值
        var result = new Dictionary<string, ConfigInfo>();

        await Scoped.CreateAsync(async (_, scope) =>
        {
            var service = scope.ServiceProvider;

            var _cache = service.GetService<ICache>();

            // 获取缓存的Key
            var cacheKey = isSystem ? CacheConst.SysConfigInfo : CacheConst.TenConfigInfo;

            // 从缓存中读取配置
            var cacheRes = await _cache.GetAsync<Dictionary<string, ConfigInfo>>(cacheKey);

            // 判断缓存中是否存在
            if (cacheRes == null || cacheRes.Count == 0)
            {
                if (isSystem)
                {
                    var db = service.GetService<ISqlSugarClient>();

                    // ReSharper disable once PossibleNullReferenceException
                    var _db = db.AsTenant().GetConnection(GlobalContext.ConnectionStringsOptions.DefaultConnectionId);

                    // 获取所有数据
                    var data = await _db.Queryable<SysConfigModel>().Select<ConfigInfo>().ToListAsync();

                    // 组装数据
                    cacheRes = data.ToDictionary(item => item.Code, item => item.Adapt<ConfigInfo>());

                    // 放入缓存
                    await _cache.SetAsync(cacheKey, cacheRes);
                }
                else
                {
                    var _db = service.GetService<ISqlSugarClient>().LoadSqlSugar<TenConfigModel>(_cache);

                    // 获取所有数据
                    var data = await _db.Queryable<TenConfigModel>().Select<ConfigInfo>().ToListAsync();

                    // 组装数据
                    cacheRes = data.ToDictionary(item => item.Code, item => item.Adapt<ConfigInfo>());

                    // 放入缓存
                    await _cache.SetAsync(cacheKey, cacheRes);
                }
            }

            // 查找结果
            foreach (var item in configInputs)
            {
                var info = cacheRes[item];
                if (info != null)
                {
                    result.Add(item, info);
                }
            }
        });

        return result;
    }
}