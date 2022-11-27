using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.AdminFactory.ModelFactory.Sys;
using Fast.Core.Operation.Dict.Dto;
using Furion.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Core.Operation.Dict;

/// <summary>
/// 字典操作静态类
/// </summary>
public static class DictOperation
{
    /// <summary>
    /// 根据字典Code获取字典
    /// </summary>
    /// <param name="dictCode"></param>
    /// <returns></returns>
    public static SysDictTypeInfo GetDictionary(string dictCode)
    {
        var result = GetDictionary(new[] {dictCode});
        return result[dictCode];
    }

    /// <summary>
    /// 根据字典Code获取多个字典
    /// </summary>
    /// <param name="dictCodeList"></param>
    /// <returns></returns>
    public static Dictionary<string, SysDictTypeInfo> GetDictionary(params string[] dictCodeList)
    {
        return GetDictionaryAsync(dictCodeList).Result;
    }

    /// <summary>
    /// 根据字典Code获取字典
    /// </summary>
    /// <param name="dictCode"></param>
    /// <returns></returns>
    public static async Task<SysDictTypeInfo> GetDictionaryAsync(string dictCode)
    {
        var result = await GetDictionaryAsync(new[] {dictCode});
        return result[dictCode];
    }

    /// <summary>
    /// 根据字典Code获取多个字典
    /// </summary>
    /// <param name="dictCodeList"></param>
    /// <returns></returns>
    public static async Task<Dictionary<string, SysDictTypeInfo>> GetDictionaryAsync(params string[] dictCodeList)
    {
        // 返回值
        var result = new Dictionary<string, SysDictTypeInfo>();

        await Scoped.CreateAsync(async (_, scope) =>
        {
            var service = scope.ServiceProvider;

            var _cache = service.GetService<ICache>();

            // 从缓存中读取
            // ReSharper disable once PossibleNullReferenceException
            var cacheRes = await _cache.GetAsync<Dictionary<string, SysDictTypeInfo>>(CacheConst.SysDictInfo);

            // 判断缓存中是否存在
            if (cacheRes == null || cacheRes.Count == 0)
            {
                var db = service.GetService<ISqlSugarClient>();

                // ReSharper disable once PossibleNullReferenceException
                var _db = db.AsTenant().GetConnection(GlobalContext.ConnectionStringsOptions.DefaultConnectionId);

                // 获取所有数据
                var data = await _db.Queryable<SysDictTypeModel>().Where(wh => wh.Status == CommonStatusEnum.Enable).Includes(i =>
                        i.DataList.Where(wh => wh.Status == CommonStatusEnum.Enable).OrderBy(ob => ob.Sort).Select(sl =>
                            new SysDictDataModel {Code = sl.Code, ChValue = sl.ChValue, EnValue = sl.EnValue}).ToList())
                    .ToListAsync(sl =>
                        new SysDictTypeModel {Code = sl.Code, ChName = sl.ChName, EnName = sl.EnName, DataList = sl.DataList});

                // 组装数据
                cacheRes = data.ToDictionary(item => item.Code, item => item.Adapt<SysDictTypeInfo>());

                // 放入缓存
                await _cache.SetAsync(CacheConst.SysDictInfo, cacheRes);
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