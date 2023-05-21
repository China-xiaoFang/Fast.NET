using Fast.Admin.Model.Enum;
using Fast.Admin.Model.Model.Sys.Dic;
using Fast.Admin.Model.Model.Tenant.Dic;
using Fast.Core.Const;
using Fast.Core.Operation.Dict.Dto;
using Fast.Iaas.Cache;
using Fast.SqlSugar.Extension;
using Furion.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Core.Operation.Dict;

/// <summary>
/// 字典操作静态类
/// </summary>
public static class DictOperation
{
    /// <summary>
    /// 系统字典
    /// </summary>
    public static class System
    {
        /// <summary>
        /// 根据字典Code获取字典
        /// </summary>
        /// <param name="dictCode"></param>
        /// <returns></returns>
        public static DictTypeInfo GetDictionary(string dictCode)
        {
            var result = GetDictionary(new[] {dictCode});
            return result[dictCode];
        }

        /// <summary>
        /// 根据字典Code获取多个字典
        /// </summary>
        /// <param name="dictCodeList"></param>
        /// <returns></returns>
        public static Dictionary<string, DictTypeInfo> GetDictionary(params string[] dictCodeList)
        {
            return GetDictionaryAsync(dictCodeList).Result;
        }

        /// <summary>
        /// 根据字典Code获取字典
        /// </summary>
        /// <param name="dictCode"></param>
        /// <returns></returns>
        public static async Task<DictTypeInfo> GetDictionaryAsync(string dictCode)
        {
            var result = await GetDictionaryAsync(new[] {dictCode});
            return result[dictCode];
        }

        /// <summary>
        /// 根据字典Code获取多个字典
        /// </summary>
        /// <param name="dictCodeList"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, DictTypeInfo>> GetDictionaryAsync(params string[] dictCodeList)
        {
            return await DictOperation.GetDictionaryAsync(true, dictCodeList);
        }
    }

    /// <summary>
    /// 租户字典
    /// 注：这里必须是登陆后或者存在Token的情况下才能使用
    /// </summary>
    public static class Tenant
    {
        /// <summary>
        /// 根据字典Code获取字典
        /// </summary>
        /// <param name="dictCode"></param>
        /// <returns></returns>
        public static DictTypeInfo GetDictionary(string dictCode)
        {
            var result = GetDictionary(new[] {dictCode});
            return result[dictCode];
        }

        /// <summary>
        /// 根据字典Code获取多个字典
        /// </summary>
        /// <param name="dictCodeList"></param>
        /// <returns></returns>
        public static Dictionary<string, DictTypeInfo> GetDictionary(params string[] dictCodeList)
        {
            return GetDictionaryAsync(dictCodeList).Result;
        }

        /// <summary>
        /// 根据字典Code获取字典
        /// </summary>
        /// <param name="dictCode"></param>
        /// <returns></returns>
        public static async Task<DictTypeInfo> GetDictionaryAsync(string dictCode)
        {
            var result = await GetDictionaryAsync(new[] {dictCode});
            return result[dictCode];
        }

        /// <summary>
        /// 根据字典Code获取多个字典
        /// </summary>
        /// <param name="dictCodeList"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, DictTypeInfo>> GetDictionaryAsync(params string[] dictCodeList)
        {
            return await DictOperation.GetDictionaryAsync(false, dictCodeList);
        }
    }

    /// <summary>
    /// 根据字典Code获取多个字典
    /// </summary>
    /// <param name="isSystem"></param>
    /// <param name="dictCodeList"></param>
    /// <returns></returns>
    private static async Task<Dictionary<string, DictTypeInfo>> GetDictionaryAsync(bool isSystem, params string[] dictCodeList)
    {
        // 返回值
        var result = new Dictionary<string, DictTypeInfo>();

        await Scoped.CreateAsync(async (_, scope) =>
        {
            var service = scope.ServiceProvider;

            var _cache = service.GetService<ICache>();

            // 获取缓存的Key
            var cacheKey = isSystem ? CacheConst.SysDictInfo : CacheConst.TenDictInfo;

            // 从缓存中读取配置
            var cacheRes = await _cache.GetAsync<Dictionary<string, DictTypeInfo>>(cacheKey);

            // 判断缓存中是否存在
            if (cacheRes == null || cacheRes.Count == 0)
            {
                if (isSystem)
                {
                    var db = service.GetService<ISqlSugarClient>();

                    // ReSharper disable once PossibleNullReferenceException
                    var _db = db.AsTenant().GetConnection(Extension.GetDefaultDataBaseInfo().ConnectionId);

                    // 获取所有数据
                    var data = await _db.Queryable<SysDictTypeModel>().Where(wh => wh.Status == CommonStatusEnum.Enable)
                        .Includes(i =>
                            i.DataList.Where(wh => wh.Status == CommonStatusEnum.Enable).OrderBy(ob => ob.Sort).Select(sl =>
                                new SysDictDataModel {Code = sl.Code, ChValue = sl.ChValue, EnValue = sl.EnValue}).ToList())
                        .ToListAsync(sl =>
                            new SysDictTypeModel
                            {
                                Code = sl.Code, ChName = sl.ChName, EnName = sl.EnName, DataList = sl.DataList
                            });

                    // 组装数据
                    cacheRes = data.ToDictionary(item => item.Code, item => item.Adapt<DictTypeInfo>());

                    // 放入缓存
                    await _cache.SetAsync(cacheKey, cacheRes);
                }
                else
                {
                    var (_db, _) = service.GetService<ISqlSugarClient>().LoadSqlSugar<TenDictTypeModel>(_cache);

                    // 获取所有数据
                    var data = await _db.Queryable<TenDictTypeModel>().Where(wh => wh.Status == CommonStatusEnum.Enable)
                        .Includes(i =>
                            i.DataList.Where(wh => wh.Status == CommonStatusEnum.Enable).OrderBy(ob => ob.Sort).Select(sl =>
                                new SysDictDataModel {Code = sl.Code, ChValue = sl.ChValue, EnValue = sl.EnValue}).ToList())
                        .ToListAsync(sl =>
                            new TenDictTypeModel
                            {
                                Code = sl.Code, ChName = sl.ChName, EnName = sl.EnName, DataList = sl.DataList
                            });

                    // 组装数据
                    cacheRes = data.ToDictionary(item => item.Code, item => item.Adapt<DictTypeInfo>());

                    // 放入缓存
                    await _cache.SetAsync(cacheKey, cacheRes);
                }
            }

            // 查找结果
            foreach (var item in dictCodeList)
            {
                var info = cacheRes[item];
                result.Add(item, info);
            }
        });

        return result;
    }
}