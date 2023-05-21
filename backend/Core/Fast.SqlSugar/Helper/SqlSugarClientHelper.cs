using Fast.Core.Filter;

namespace Fast.Core.SqlSugar.Helper;

/// <summary>
/// SqlSugarClient帮助类
/// </summary>
static class SqlSugarClientHelper
{
    private const string CacheKey = "Tenant:DbInfo:";

    /// <summary>
    /// 得到DbInfo
    /// </summary>
    /// <param name=""></param>
    /// <param name="_cache"></param>
    /// <param name="_db"></param>
    /// <param name="dbType"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    /// <exception cref="SqlSugarException"></exception>
    public static SysTenantDataBaseModel GetDbInfo(ICache _cache, ISqlSugarClient _db, int dbType, long tenantId)
    {
        // 数据库信息缓存
        var dbInfoList = _cache.Get<List<SysTenantDataBaseModel>>($"{CacheKey}{tenantId}");
        if (dbInfoList == null || !dbInfoList.Any())
        {
            dbInfoList = _db.Queryable<SysTenantDataBaseModel>().Where(wh => wh.TenantId == tenantId).Filter(null, true).ToList();
            _cache.Set($"{CacheKey}{tenantId}", dbInfoList);
        }

        var db = dbInfoList.FirstOrDefault(f => f.SugarSysDbType == dbType);
        if (db == null)
        {
            throw new SqlSugarException("租户数据库配置异常！");
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
        var connectionId = $"{dbInfo.SugarSysDbType}_{dbInfo.TenantId}";

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
        SugarEntityFilter.LoadSugarFilter(_db, dbInfo.CommandTimeOut, dbInfo.SugarSqlExecMaxSeconds, dbInfo.DiffLog);

        return _db;
    }
}