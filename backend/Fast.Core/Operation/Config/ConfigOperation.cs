using Fast.Core.AdminFactory.ModelFactory.Sys;
using Fast.Core.AdminFactory.ModelFactory.Tenant;
using Fast.Core.Cache;
using Fast.Core.Const;
using Fast.Core.Operation.Config.Dto;
using Fast.SqlSugar.Tenant;
using Fast.SqlSugar.Tenant.Extension;
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
                    var _db = db.AsTenant().GetConnection(SugarContext.ConnectionStringsOptions.DefaultConnectionId);

                    // 获取所有数据
                    var data = await _db.Queryable<SysConfigModel>().Select<ConfigInfo>().ToListAsync();

                    // 组装数据
                    cacheSysRes = data.ToDictionary(item => item.Code, item => item.Adapt<ConfigInfo>());

                    // 放入缓存
                    await _cache.SetAsync(CacheConst.SysConfigInfo, cacheSysRes);
                }

                // 查找结果
                foreach (var item in configInputs)
                {
                    var info = cacheSysRes[item];
                    if (info != null)
                    {
                        result.Add(item, info);
                    }
                }
            });

            return result;
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
            // 返回值
            var result = new Dictionary<string, ConfigInfo>();

            await Scoped.CreateAsync(async (_, scope) =>
            {
                var service = scope.ServiceProvider;

                var _cache = service.GetService<ICache>();

                // 从缓存中读取租户配置
                var cacheTenRes = await _cache.GetAsync<Dictionary<string, ConfigInfo>>(CacheConst.TenConfigInfo);
                // 判断缓存中是否存在
                if (cacheTenRes == null || cacheTenRes.Count == 0)
                {
                    var _db = service.GetService<ISqlSugarClient>().LoadSqlSugar<TenConfigModel>();

                    // 获取所有数据
                    var data = await _db.Queryable<TenConfigModel>().Select<ConfigInfo>().ToListAsync();

                    // 组装数据
                    cacheTenRes = data.ToDictionary(item => item.Code, item => item.Adapt<ConfigInfo>());

                    // 放入缓存
                    await _cache.SetAsync(CacheConst.TenConfigInfo, cacheTenRes);
                }

                // 查找结果
                foreach (var item in configInputs)
                {
                    var info = cacheTenRes[item];
                    if (info != null)
                    {
                        result.Add(item, info);
                    }
                }
            });

            return result;
        }
    }
}