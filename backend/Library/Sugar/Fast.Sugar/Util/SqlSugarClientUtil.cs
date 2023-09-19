using System.Linq;
using System.Threading.Tasks;
using Fast.Cache;
using Fast.Sugar.Filter;
using Fast.Sugar.Options;
using SqlSugar;

namespace Fast.Sugar.Util;

/// <summary>
/// SqlSugarClient工具类
/// </summary>
static class SqlSugarClientUtil
{
    private const string CacheKey = "Tenant:DbInfo:";

    /// <summary>
    /// 得到DbInfo
    /// </summary>
    /// <param name="_db"></param>
    /// <param name="tableName">表名称</param>
    /// <param name="dbType"></param>
    /// <param name="tenantId"></param>
    /// <param name="_cache"></param>
    /// <returns></returns>
    /// <exception cref="SqlSugarException"></exception>
    public static async Task<ConnectionConfigOption> GetDbInfo(ISqlSugarClient _db, string tableName, int dbType, long tenantId,
        ICache _cache)
    {
        // 数据库信息缓存
        var dbInfoList = await _cache.GetAndSetAsync($"{CacheKey}{tenantId}",
            async () =>
            {
                return await _db.Queryable<ConnectionConfigOption>().AS(tableName).Where(wh => wh.TenantId == tenantId)
                    .Filter(null, true).ToListAsync();
            });

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
    public static ISqlSugarClient GetSqlSugarClient(ITenant _tenant, ConnectionConfigOption dbInfo)
    {
        var connectionId = $"{dbInfo.SugarSysDbType}_{dbInfo.TenantId}";

        if (_tenant.IsAnyConnection(connectionId))
            return _tenant.GetConnection(connectionId);

        _tenant.AddConnection(new ConnectionConfig
        {
            ConfigId = connectionId, // 此链接标志，用以后面切库使用
            ConnectionString = DataBaseUtil.GetConnectionStr(dbInfo), // 租户库连接字符串
            DbType = dbInfo.DbType,
            IsAutoCloseConnection = true, // 开启自动释放模式和EF原理一样我就不多解释了
            InitKeyType = InitKeyType.Attribute, // 从特性读取主键和自增列信息
            //InitKeyType = InitKeyType.SystemTable // 从数据库读取主键和自增列信息
            ConfigureExternalServices = DataBaseUtil.GetSugarExternalServices(dbInfo.DbType)
        });

        var _db = _tenant.GetConnection(connectionId);

        // 过滤器
        SugarEntityFilter.LoadSugarFilter(_db, dbInfo.CommandTimeOut, dbInfo.SugarSqlExecMaxSeconds, dbInfo.DiffLog);

        return _db;
    }
}