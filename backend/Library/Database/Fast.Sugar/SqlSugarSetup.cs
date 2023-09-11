using Fast.Core.SqlSugar.Filter;
using Fast.Core.SqlSugar.Repository;
using Fast.Iaas;
using Fast.Sugar.Util;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.Core.SqlSugar;

/// <summary>
/// SqlSugar
/// </summary>
public static class SqlSugarSetup
{
    /// <summary>
    /// SqlSugarClient的配置
    /// Client不能单例注入
    /// </summary>
    /// <param name="service"></param>
    public static void AddSqlSugarClientService(this IServiceCollection service)
    {
        // Add Snowflakes Id.
        // 设置雪花Id的workerId，确保每个实例workerId都应不同
        var workerId = ushort.Parse(FastContext.Configuration["SnowId:WorkerId"] ?? "1");
        YitIdHelper.SetIdGenerator(new IdGeneratorOptions {WorkerId = workerId});

        // 获取默认连接配置
        var defaultDataBaseInfo = Extension.Extension.GetDefaultDataBaseInfo();

        // 得到连接字符串
        var connectionStr = DataBaseUtil.GetConnectionStr(defaultDataBaseInfo);

        var connectConfig = new ConnectionConfig
        {
            ConfigId = defaultDataBaseInfo.ConnectionId, // 此链接标志，用以后面切库使用
            ConnectionString = connectionStr, // 核心库连接字符串
            DbType = defaultDataBaseInfo.DbType,
            IsAutoCloseConnection = true, // 开启自动释放模式和EF原理一样我就不多解释了
            InitKeyType = InitKeyType.Attribute, // 从特性读取主键和自增列信息
            //InitKeyType = InitKeyType.SystemTable // 从数据库读取主键和自增列信息
            ConfigureExternalServices = DataBaseUtil.GetSugarExternalServices(defaultDataBaseInfo.DbType)
        };

        // 注册 SqlSugarClient
        service.AddScoped<ISqlSugarClient>(_ =>
        {
            var sqlSugarClient = new SqlSugarClient(connectConfig);
            // 过滤器
            SugarEntityFilter.LoadSugarFilter(sqlSugarClient, defaultDataBaseInfo.CommandTimeOut,
                defaultDataBaseInfo.SugarSqlExecMaxSeconds, defaultDataBaseInfo.DiffLog);

            return sqlSugarClient;
        });

        // 注册非泛型仓储
        service.AddScoped<ISqlSugarRepository, SqlSugarRepository>();

        // 注册 SqlSugar 
        service.AddScoped(typeof(ISqlSugarRepository<>), typeof(SqlSugarRepository<>));
    }
}