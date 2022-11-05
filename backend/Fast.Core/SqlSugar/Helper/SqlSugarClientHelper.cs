using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.AdminFactory.ModelFactory.Tenant;
using Fast.Core.SqlSugar.Filter;
using Furion.FriendlyException;

namespace Fast.Core.SqlSugar.Helper;

/// <summary>
/// SqlSugarClient帮助类
/// </summary>
static class SqlSugarClientHelper
{
    /// <summary>
    /// 得到DbInfo
    /// </summary>
    /// <param name="_db"></param>
    /// <param name="dbType"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    public static SysTenantDataBaseModel GetDbInfo(ISqlSugarClient _db, SysDataBaseTypeEnum dbType, long tenantId)
    {
        var _cache = App.GetService<ICache>();
        // 数据库信息缓存
        var dbInfoList = _cache.Get<List<SysTenantDataBaseModel>>($"{CacheConst.CACHE_KEY_TENANT_DB_INFO}{tenantId}");
        if (dbInfoList == null || !dbInfoList.Any())
        {
            dbInfoList = _db.Queryable<SysTenantDataBaseModel>().Where(wh => wh.TenantId == tenantId).Filter(null, true).ToList();
            _cache.Set($"{CacheConst.CACHE_KEY_TENANT_DB_INFO}{tenantId}", dbInfoList);
        }

        var db = dbInfoList.FirstOrDefault(f => f.SysDbType == dbType);
        if (db == null)
        {
            throw Oops.Oh(ErrorCode.TenantDbError);
        }

        return db;
    }

    /// <summary>
    /// 得到SqlSugar客户端
    /// </summary>
    /// <param name="_tenant"></param>
    /// <param name="dbInfo"></param>
    /// <returns></returns>
    public static ISqlSugarClient GetSqlSugarClient(ITenant _tenant, SysTenantDataBaseModel dbInfo)
    {
        var connectionId = $"{dbInfo.SysDbType}_{dbInfo.TenantId}";

        if (_tenant.IsAnyConnection(connectionId))
            return _tenant.GetConnection(connectionId);

        _tenant.AddConnection(new ConnectionConfig
        {
            ConfigId = connectionId, // 此链接标志，用以后面切库使用
            ConnectionString = DataBaseHelper.GetConnectionStr(dbInfo), // 租户库连接字符串
            DbType = dbInfo.DbType,
            IsAutoCloseConnection = true, // 开启自动释放模式和EF原理一样我就不多解释了
            InitKeyType = InitKeyType.Attribute, // 从特性读取主键和自增列信息
            //InitKeyType = InitKeyType.SystemTable // 从数据库读取主键和自增列信息
            ConfigureExternalServices = DataBaseHelper.GetSugarExternalServices(dbInfo.DbType)
        });

        var _db = _tenant.GetConnection(connectionId);

        // 过滤器
        SugarEntityFilter.LoadSugarFilter(_db);

        return _db;
    }
}