using Fast.NET.Core.AdminFactory.ModelFactory.Tenant;
using Fast.NET.Core.BaseFactory;
using Fast.NET.Core.BaseFactory.Const;
using Fast.NET.Core.SqlSugar.Extension;
using Fast.NET.Core.SqlSugar.Internal;
using static Furion.App;

namespace Fast.NET.Core.SqlSugar;

/// <summary>
/// SqlSugar配置
/// </summary>
public static class SqlSugarConfig
{
    /// <summary>
    /// 缓存配置
    /// </summary>
    public static readonly ICache _cache = GetService<ICache>();

    /// <summary>
    /// 反射获取所有的数据库Model
    /// </summary>
    /// <returns></returns>
    private static (string key, SysDataBaseTypeEnum type) ReflexGetAllTEntity(string name)
    {
        // 先从缓存获取
        var result = _cache.Get<List<(string key, SysDataBaseTypeEnum type)>>(CommonConst.CACHE_KEY_MODEL_DB_TYPE);

        if (result != null && result.Any())
        {
            var entityInfo = result.FirstOrDefault(f => f.key == name);
            if (entityInfo.key.IsEmpty())
            {
                throw Oops.Oh(ErrorCodeEnum.SugarModelError);
            }

            return entityInfo;
        }
        else
        {
            // 获取所有实现了接口的类的类型
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(sl => sl.GetTypes().Where(wh => wh.GetInterfaces().Contains(typeof(IDbEntity))));

            // 遍历获取到的类型集合
            // 获取数据库类型，如果没有则默认是Admin库
            result = types.Select(type => new ValueTuple<string, SysDataBaseTypeEnum>(type.Name,
                type.GetCustomAttribute<DataBaseTypeAttribute>(true)?.SysDataBaseType ?? SysDataBaseTypeEnum.Admin)).ToList();

            var entityInfo = result.FirstOrDefault(f => f.key == name);
            if (entityInfo.key.IsEmpty())
            {
                throw Oops.Oh(ErrorCodeEnum.SugarModelError);
            }

            return entityInfo;
        }
    }

    public static ISqlSugarClient LoadSqlSugar<TEntity>(this ISqlSugarClient db) where TEntity : class, new()
    {
        var _db = (SqlSugarClient) db;

        var dbType = ReflexGetAllTEntity(typeof(TEntity).Name);

        // 默认Db
        var defaultDb = _db.GetConnection(GlobalContext.ConnectionInfo.ConnectionId);

        switch (dbType.type)
        {
            case SysDataBaseTypeEnum.Admin:
                return defaultDb;
            case SysDataBaseTypeEnum.Tenant:
                // 获取租户Id
                var tenantId = GlobalContext.GetTenantId();

                // 这里每次都获取一下数据库信息，方便跨库连接
                GlobalContext.BusinessDbInfo = GetDbInfo(defaultDb, SysDataBaseTypeEnum.Tenant, tenantId);

                return GetSqlSugarClient(_db, GlobalContext.BusinessDbInfo);
            default:
                throw Oops.Oh(ErrorCodeEnum.SugarModelError);
        }
    }

    /// <summary>
    /// 初始化SqlSugar配置
    /// </summary>
    public static void InitSqlSugar(this IServiceCollection services)
    {
        // 程序入口Exe文件名称
        var appName = WebHostEnvironment.ApplicationName;

        // 清楚Model缓存
        _cache.Del($"{appName}{CommonConst.CACHE_KEY_MODEL_TYPE}");
        _cache.Del($"{appName}{CommonConst.CACHE_KEY_MODEL_DLL}");

        var connectConfig = new ConnectionConfig
        {
            ConfigId = GlobalContext.ConnectionInfo.ConnectionId, // 此链接标志，用以后面切库使用
            ConnectionString = GlobalContext.ConnectionInfo.Connection, // 核心库连接字符串
            DbType = GlobalContext.ConnectionInfo.DbType,
            //IsShardSameThread = true,
            IsAutoCloseConnection = true, // 开启自动释放模式和EF原理一样我就不多解释了
            InitKeyType = InitKeyType.Attribute, // 从特性读取主键和自增列信息
            //InitKeyType = InitKeyType.SystemTable // 从数据库读取主键和自增列信息
        };

        // Sugar Action
        void ConfigAction(ISqlSugarClient db)
        {
            SqlSugarProvider _db = db.AsTenant().GetConnection(connectConfig.ConfigId);

            // 过滤器
            LoadSugarFilter(_db);
        }

        services.AddSqlSugar(connectConfig, (Action<ISqlSugarClient>) ConfigAction);
    }

    /// <summary>
    /// 得到DbInfo
    /// </summary>
    /// <param name="_db"></param>
    /// <param name="dbType"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    public static SysTenantDataBaseModel GetDbInfo(SqlSugarProvider _db, SysDataBaseTypeEnum dbType, long tenantId)
    {
        // 数据库信息缓存
        var dbInfoList = _cache.Get<List<SysTenantDataBaseModel>>($"{CommonConst.CACHE_KEY_TENANT_DB_INFO}{tenantId}");
        if (dbInfoList == null || !dbInfoList.Any())
        {
            dbInfoList = _db.Queryable<SysTenantDataBaseModel>().Where(wh => wh.TenantId == tenantId).ToList();
            _cache.Set($"{CommonConst.CACHE_KEY_TENANT_DB_INFO}{tenantId}", dbInfoList);
        }

        var db = dbInfoList.FirstOrDefault(f => f.DbType == dbType);
        if (db == null)
        {
            throw Oops.Oh("租户数据库配置异常！");
        }

        return db;
    }

    /// <summary>
    /// 得到SqlSugar客户端
    /// </summary>
    /// <param name="_db"></param>
    /// <param name="dbInfo"></param>
    /// <returns></returns>
    private static ISqlSugarClient GetSqlSugarClient(SqlSugarClient _db, SysTenantDataBaseModel dbInfo)
    {
        var connectionId = $"{dbInfo.DbType}_{dbInfo.TenantId}";

        if (_db.IsAnyConnection(connectionId))
            return _db.GetConnection(connectionId);

        AddDataBase(_db, connectionId, dbInfo);
        return _db.GetConnection(connectionId);
    }

    /// <summary>
    /// 添加数据库
    /// </summary>
    /// <param name="db"></param>
    /// <param name="connectionId"></param>
    /// <param name="dbInfo"></param>
    public static void AddDataBase(SqlSugarClient db, string connectionId, SysTenantDataBaseModel dbInfo)
    {
        var connectionStr = "";

        // TODO:这里要根据数据库的类型来拼接链接字符串，目前暂时支持Sql Server
        switch (dbInfo.DbType)
        {
            case DbType.MySql:
                break;
            case DbType.SqlServer:
                connectionStr =
                    $"Data Source={dbInfo.ServiceIp}; Initial Catalog={dbInfo.DbName}; User ID={dbInfo.DbUser};Password={dbInfo.DbPwd};MultipleActiveResultSets=true;max pool size=100;";
                break;
            case DbType.Sqlite:
                break;
            case DbType.Oracle:
                break;
            case DbType.PostgreSQL:
                break;
            case DbType.Dm:
                break;
            case DbType.Kdbndp:
                break;
            case DbType.Oscar:
                break;
            case DbType.MySqlConnector:
                break;
            case DbType.Access:
                break;
            case DbType.OpenGauss:
                break;
            case DbType.Custom:
                break;
            default:
                throw Oops.Oh("数据库Type 配置异常！");
        }

        db.AddConnection(new ConnectionConfig
        {
            ConfigId = connectionId, // 此链接标志，用以后面切库使用
            ConnectionString = connectionStr, // 租户库连接字符串
            DbType = dbInfo.DbType,
            //IsShardSameThread = true,
            IsAutoCloseConnection = true, // 开启自动释放模式和EF原理一样我就不多解释了
            InitKeyType = InitKeyType.Attribute, // 从特性读取主键和自增列信息
            //InitKeyType = InitKeyType.SystemTable // 从数据库读取主键和自增列信息
        });

        var _db = db.GetConnection(connectionId);

        // 过滤器
        LoadSugarFilter(_db);
    }

    /// <summary>
    /// 日志Model集合
    /// </summary>
    private static readonly List<string> _logModelList = new List<string>
    {
        //nameof(SysLogVisModel),
        //nameof(SysLogExModel),
        //nameof(SysLogOpModel),
        //nameof(DingDingSendLogModel),
        //nameof(SysScheduledServerOpLogModel)
    };

    /// <summary>
    /// 加载过滤器
    /// </summary>
    /// <param name="_db"></param>
    private static void LoadSugarFilter(ISqlSugarClient _db)
    {
        // 执行超时时间 60秒
        _db.Ado.CommandTimeOut = 60;

        if (HostEnvironment.IsDevelopment())
        {
            _db.Aop.OnLogExecuted = (sql, pars) =>
            {
                if (sql.StartsWith("SELECT"))
                    Console.ForegroundColor = ConsoleColor.DarkGreen;

                if (sql.StartsWith("UPDATE") || sql.StartsWith("INSERT"))
                {
                    // 如果是两个Log表，则不输出
                    if (sql.Contains("[sys_log_op]") || sql.Contains("[sys_log_ex]"))
                        return;
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                }

                if (sql.StartsWith("DELETE"))
                    Console.ForegroundColor = ConsoleColor.Blue;

                PrintToMiniProfiler("SqlSugar", "Info", SqlProfiler.ParameterFormat(sql, pars));
                Console.WriteLine("\r\n\r\n" + SqlProfiler.ParameterFormat(sql, pars));
            };

            _db.Aop.OnError = exp =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\r\n\r\n错误 Sql语句：{SqlProfiler.ParameterFormat(exp.Sql, exp.Parametres)}");
            };
        }

        // 程序入口Exe文件名称
        var appName = WebHostEnvironment.ApplicationName;

        var modelTypeList = _cache.Get<List<Type>>($"{appName}{CommonConst.CACHE_KEY_MODEL_TYPE}");
        if (modelTypeList == null || !modelTypeList.Any())
        {
            var modelDllList = _cache.Get<List<string>>($"{appName}{CommonConst.CACHE_KEY_MODEL_DLL}");
            if (modelDllList == null || !modelDllList.Any())
            {
                modelDllList = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.Model.dll").ToList();
                // 排除 Zy.NET.Base.Model.dll
                modelDllList = modelDllList.Where(wh => !wh.EndsWith("Zy.NET.Base.Model.dll")).ToList();
                _cache.Set($"{appName}{CommonConst.CACHE_KEY_MODEL_DLL}", modelDllList);
            }

            modelTypeList = new List<Type>();

            foreach (var dll in modelDllList)
            {
                modelTypeList.AddRange(Assembly.LoadFrom(dll).GetTypes().Where(wh =>
                    wh.GetCustomAttributes(typeof(SugarTable), true).FirstOrDefault() != null));
            }

            _cache.Set($"{appName}{CommonConst.CACHE_KEY_MODEL_TYPE}", modelTypeList);
        }

        foreach (var type in modelTypeList)
        {
            // 配置多租户全局过滤器
            // 判断实体中是否包含TenantId属性
            if (!type.GetProperty(ClaimConst.TENANT_ID).IsEmpty())
            {
                // 判断如果Type为5张日志表，超管默认可以查看所有
                if (!(_logModelList.Contains(type.Name) && GlobalContext.IsSuperAdmin))
                {
                    // 构建动态Lambda
                    var lambda = DynamicExpressionParser.ParseLambda(new[] {Expression.Parameter(type, "it")}, typeof(bool),
                        $"{nameof(DBEntityTenant.TenantId)} ==  @0 OR ( @1 AND @2 )", GlobalContext.GetTenantId(false),
                        GlobalContext.IsSuperAdmin, GlobalContext.SystemSettings.SuperAdminViewAllData);
                    // 将Lambda传入过滤器
                    _db.QueryFilter.Add(new TableFilterItem<object>(type, lambda));
                }
            }

            // 配置加删除全局过滤器
            // 判断实体中是否存在IsDeleted
            // ReSharper disable once InvertIf
            if (!type.GetProperty(ClaimConst.DELETE_FIELD).IsEmpty())
            {
                // 构建动态Lambda
                var lambda = DynamicExpressionParser.ParseLambda(new[] {Expression.Parameter(type, "it")}, typeof(bool),
                    $"{nameof(DEntityBase.IsDeleted)} ==  @0", false);
                // 将Lambda传入过滤器
                _db.QueryFilter.Add(new TableFilterItem<object>(type, lambda) {IsJoinQuery = true});
            }
        }
    }
}