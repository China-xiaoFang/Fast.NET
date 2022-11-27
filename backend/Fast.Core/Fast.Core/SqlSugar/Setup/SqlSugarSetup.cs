using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.AdminFactory.ModelFactory.Tenant;
using Fast.Core.SqlSugar.Filter;
using Fast.Core.SqlSugar.Helper;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Core.SqlSugar.Setup;

/// <summary>
/// SqlSugar设置/配置/入口
/// </summary>
public static class SqlSugarSetup
{
    /// <summary>
    /// SqlSugarClient的配置
    /// Client不能单例注入
    /// </summary>
    public static void SqlSugarClientConfigure(this IServiceCollection services)
    {
        // 清除ModelType缓存
        EntityHelper.ClearCacheEntityType();

        // 得到连接字符串
        var connectionStr = DataBaseHelper.GetConnectionStr(new SysTenantDataBaseModel
        {
            ServiceIp = GlobalContext.ConnectionStringsOptions.DefaultServiceIp,
            Port = GlobalContext.ConnectionStringsOptions.DefaultPort,
            DbName = GlobalContext.ConnectionStringsOptions.DefaultDbName,
            DbUser = GlobalContext.ConnectionStringsOptions.DefaultDbUser,
            DbPwd = GlobalContext.ConnectionStringsOptions.DefaultDbPwd,
            SysDbType = SysDataBaseTypeEnum.Admin,
            DbType = GlobalContext.ConnectionStringsOptions.DefaultDbType
        });

        var connectConfig = new ConnectionConfig
        {
            ConfigId = GlobalContext.ConnectionStringsOptions.DefaultConnectionId, // 此链接标志，用以后面切库使用
            ConnectionString = connectionStr, // 核心库连接字符串
            DbType = GlobalContext.ConnectionStringsOptions.DefaultDbType,
            IsAutoCloseConnection = true, // 开启自动释放模式和EF原理一样我就不多解释了
            InitKeyType = InitKeyType.Attribute, // 从特性读取主键和自增列信息
            //InitKeyType = InitKeyType.SystemTable // 从数据库读取主键和自增列信息
            ConfigureExternalServices =
                DataBaseHelper.GetSugarExternalServices(GlobalContext.ConnectionStringsOptions.DefaultDbType)
        };

        // 注册 SqlSugarClient
        services.AddScoped<ISqlSugarClient>(_ =>
        {
            var sqlSugarClient = new SqlSugarClient(connectConfig);
            // 过滤器
            SugarEntityFilter.LoadSugarFilter(sqlSugarClient);

            return sqlSugarClient;
        });

        // 注册非泛型仓储
        services.AddScoped<ISqlSugarRepository, SqlSugarRepository>();

        // 注册 SqlSugar 
        services.AddScoped(typeof(ISqlSugarRepository<>), typeof(SqlSugarRepository<>));
    }
}