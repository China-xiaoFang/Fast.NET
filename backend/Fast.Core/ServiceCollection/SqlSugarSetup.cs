using Fast.Admin.Model.Enum;
using Fast.Admin.Model.Model.Sys;
using Fast.Core.Filter;
using Fast.Core.SqlSugar.Helper;
using Fast.Core.SqlSugar.Repository;
using Microsoft.Extensions.DependencyInjection;
using Yitter.IdGenerator;

namespace Fast.Core.ServiceCollection;

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
        var workerId = ushort.Parse(App.Configuration["SnowId:WorkerId"] ?? "1");
        YitIdHelper.SetIdGenerator(new IdGeneratorOptions {WorkerId = workerId});

        // 得到连接字符串
        var connectionStr = DataBaseHelper.GetConnectionStr(new SysTenantDataBaseModel
        {
            ServiceIp = GlobalContext.ConnectionStringsOptions.DefaultServiceIp,
            Port = GlobalContext.ConnectionStringsOptions.DefaultPort,
            DbName = GlobalContext.ConnectionStringsOptions.DefaultDbName,
            DbUser = GlobalContext.ConnectionStringsOptions.DefaultDbUser,
            DbPwd = GlobalContext.ConnectionStringsOptions.DefaultDbPwd,
            SugarSysDbType = SugarDbTypeEnum.Default.GetHashCode(),
            SugarDbTypeName = SugarDbTypeEnum.Default.GetDescription(),
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
        service.AddScoped<ISqlSugarClient>(_ =>
        {
            var sqlSugarClient = new SqlSugarClient(connectConfig);
            // 过滤器
            SugarEntityFilter.LoadSugarFilter(sqlSugarClient, GlobalContext.ConnectionStringsOptions.CommandTimeOut,
                GlobalContext.ConnectionStringsOptions.SugarSqlExecMaxSeconds, GlobalContext.ConnectionStringsOptions.DiffLog);

            return sqlSugarClient;
        });

        // 注册非泛型仓储
        service.AddScoped<ISqlSugarRepository, SqlSugarRepository>();

        // 注册 SqlSugar 
        service.AddScoped(typeof(ISqlSugarRepository<>), typeof(SqlSugarRepository<>));
    }
}