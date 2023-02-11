using System.ComponentModel;
using System.Reflection;
using Fast.SqlSugar.Tenant.Extension;
using Fast.SqlSugar.Tenant.Filter;
using Fast.SqlSugar.Tenant.Helper;
using Fast.SqlSugar.Tenant.Internal.Enum;
using Fast.SqlSugar.Tenant.Internal.Options;
using Fast.SqlSugar.Tenant.Repository;
using Fast.SqlSugar.Tenant.SugarEntity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.SqlSugar.Tenant.Setup;

/// <summary>
/// SqlSugar设置/配置/入口
/// </summary>
public static class SqlSugarSetup
{
    /// <summary>
    /// 添加雪花Id
    /// 需要在JSON文件中配置获取传入WorkerId
    /// </summary>
    /// <param name="services"></param>
    /// <param name="snowIdWorkerId"></param>
    public static void AddSnowflakeId(this IServiceCollection services, string snowIdWorkerId = "1")
    {
        var buildServiceProvider = services.BuildServiceProvider();

        if (buildServiceProvider.GetService(typeof(IConfiguration)) is not IConfiguration configuration)
        {
            // SqlSugarRepository需要IConfiguration服务，请注入IConfiguration服务
            throw new NullReferenceException(
                "SqlSugarRepository requires IConfiguration service. Please inject the IHostEnvironment service.");
        }

        // 设置雪花Id的workerId，确保每个实例workerId都应不同
        var workerId = ushort.Parse(configuration["SnowId:WorkerId"] ?? snowIdWorkerId);
        YitIdHelper.SetIdGenerator(new IdGeneratorOptions {WorkerId = workerId});
    }

    /// <summary>
    /// SqlSugarClient的配置
    /// Client不能单例注入
    /// </summary>
    /// <param name="services"></param>
    /// <param name="hostEnvironment"></param>
    /// <exception cref="NullReferenceException"></exception>
    public static void SqlSugarClientConfigure(this IServiceCollection services, IHostEnvironment hostEnvironment = null)
    {
        var buildServiceProvider = services.BuildServiceProvider();
        // 获取分布式内存缓存服务，判断是否已经注入了分布式缓存的服务
        if (buildServiceProvider.GetService(typeof(IMemoryCache)) is not IMemoryCache memoryCache)
        {
            // SqlSugarRepository需要MemoryCache服务，请引用并且注入MemoryCache服务
            throw new NullReferenceException(
                "SqlSugarRepository requires MemoryCache service. Please reference and inject the MemoryCache service.");
        }

        // 放入上下文，方便使用
        SugarContext._memoryCache = memoryCache;
        if (hostEnvironment == null)
        {
            if (buildServiceProvider.GetService(typeof(IHostEnvironment)) is not IHostEnvironment hostEnvironmentService)
            {
                // SqlSugarRepository需要IHostEnvironment服务，请注入IHostEnvironment服务
                throw new NullReferenceException(
                    "SqlSugarRepository requires IHostEnvironment service. Please inject the IHostEnvironment service.");
            }

            SugarContext.HostEnvironment = hostEnvironmentService;
        }
        else
        {
            SugarContext.HostEnvironment = hostEnvironment;
        }

        if (buildServiceProvider.GetService(typeof(IConfiguration)) is not IConfiguration configuration)
        {
            // SqlSugarRepository需要IConfiguration服务，请注入IConfiguration服务
            throw new NullReferenceException(
                "SqlSugarRepository requires IConfiguration service. Please inject the IHostEnvironment service.");
        }

        // 读取Json文件配置
        var connectionStringsOptions = configuration.GetSection("ConnectionStrings").Get<ConnectionStringsOptions>();

        if (connectionStringsOptions == null)
        {
            // SqlSugar json配置文件为空，请配置json文件
            throw new SqlSugarException("SqlSugar json configuration file is empty, please configure the json file");
        }

        SugarContext.ConnectionStringsOptions = connectionStringsOptions;

        // 清除ModelType缓存
        EntityHelper.ClearCacheEntityType();

        // 得到连接字符串
        var connectionStr = DataBaseHelper.GetConnectionStr(new SysTenantDataBaseModel
        {
            ServiceIp = SugarContext.ConnectionStringsOptions.DefaultServiceIp,
            Port = SugarContext.ConnectionStringsOptions.DefaultPort,
            DbName = SugarContext.ConnectionStringsOptions.DefaultDbName,
            DbUser = SugarContext.ConnectionStringsOptions.DefaultDbUser,
            DbPwd = SugarContext.ConnectionStringsOptions.DefaultDbPwd,
            SugarSysDbType = SugarDbTypeEnum.Default.GetHashCode(),
            SugarDbTypeName = SugarDbTypeEnum.Default.GetType().GetMember(SugarDbTypeEnum.Default.ToString() ?? string.Empty)
                .FirstOrDefault()?.GetCustomAttribute<DescriptionAttribute>()?.Description,
            DbType = SugarContext.ConnectionStringsOptions.DefaultDbType
        });

        var connectConfig = new ConnectionConfig
        {
            ConfigId = SugarContext.ConnectionStringsOptions.DefaultConnectionId, // 此链接标志，用以后面切库使用
            ConnectionString = connectionStr, // 核心库连接字符串
            DbType = SugarContext.ConnectionStringsOptions.DefaultDbType,
            IsAutoCloseConnection = true, // 开启自动释放模式和EF原理一样我就不多解释了
            InitKeyType = InitKeyType.Attribute, // 从特性读取主键和自增列信息
            //InitKeyType = InitKeyType.SystemTable // 从数据库读取主键和自增列信息
            ConfigureExternalServices =
                DataBaseHelper.GetSugarExternalServices(SugarContext.ConnectionStringsOptions.DefaultDbType)
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

        // 判断是否需要初始化数据库
        if (!SugarContext.ConnectionStringsOptions.InitDataBase)
            return;

        var db = services.BuildServiceProvider().GetService(typeof(ISqlSugarClient)) as ISqlSugarClient;

        // ReSharper disable once PossibleNullReferenceException
        var _db = db.AsTenant().GetConnection(SugarContext.ConnectionStringsOptions.DefaultConnectionId);

        // 创建默认库
        _db.DbMaintenance.CreateDatabase();

        // 查询表是否存在
        if (_db.Ado.GetInt(
                $"SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_NAME = '{typeof(SysTenantDataBaseModel).GetSugarTableName()}'") >
            0)
            return;

        // 获取所有数据库Model
        var entityTypeList = EntityHelper.ReflexGetAllTEntityList();

        // 创建默认库的所有表
        _db.CodeFirst.InitTables(entityTypeList.Where(wh => wh.DbType == SugarDbTypeEnum.Default.GetHashCode())
            .Select(sl => sl.Type).ToArray());
    }
}