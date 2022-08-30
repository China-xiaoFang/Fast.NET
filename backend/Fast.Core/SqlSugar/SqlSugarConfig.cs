namespace Fast.Core.SqlSugar;

/// <summary>
/// SqlSugar配置
/// </summary>
public static class SqlSugarConfig
{
    /// <summary>
    /// 缓存配置
    /// </summary>
    private static readonly ICache _cache = GetService<ICache>();

    /// <summary>
    /// 数据库配置
    /// </summary>
    private static readonly ConnectionStringsOptions connectionStringsOptions = GetOptions<ConnectionStringsOptions>();

    /// <summary>
    /// LoadSqlSugar，支持传入租户Id，获取租户Id的DbClient
    /// 默认是当前登录用户的租户Id
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="db"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    public static ISqlSugarClient LoadSqlSugar<TEntity>(this ISqlSugarClient db, long? tenantId = null)
        where TEntity : class, new()
    {
        var _db = (SqlSugarClient) db;

        var dbType = ReflexGetAllTEntity(typeof(TEntity).Name);

        // 默认Db
        var defaultDb = _db.GetConnection(connectionStringsOptions.DefaultConnectionId);

        switch (dbType.dbType)
        {
            case SysDataBaseTypeEnum.Admin:
                return defaultDb;
            case SysDataBaseTypeEnum.Tenant:
                if (tenantId.IsNullOrZero())
                {
                    // 获取租户Id
                    tenantId = GlobalContext.GetTenantId();
                }

                // 这里每次都获取一下数据库信息，方便跨库连接
                GlobalContext.TenantDbInfo = GetDbInfo(defaultDb, SysDataBaseTypeEnum.Tenant, tenantId.ParseToLong());

                return GetSqlSugarClient(_db, GlobalContext.TenantDbInfo);
            default:
                throw Oops.Oh(ErrorCode.SugarModelError);
        }
    }

    /// <summary>
    /// 初始化SqlSugar配置
    /// </summary>
    public static void InitSqlSugar(this IServiceCollection services)
    {
        // 程序入口Exe文件名称
        var appName = WebHostEnvironment.ApplicationName;

        // 清除ModelType缓存
        _cache.Del($"{appName}{CommonConst.CACHE_KEY_MODEL_DB_TYPE}");

        var connectConfig = new ConnectionConfig
        {
            ConfigId = connectionStringsOptions.DefaultConnectionId, // 此链接标志，用以后面切库使用
            ConnectionString = connectionStringsOptions.DefaultConnection, // 核心库连接字符串
            //DbType = (DbType) Enum.Parse(typeof(DbType), connectionStringsOptions.DefaultDbType).ParseToInt(),
            DbType = connectionStringsOptions.DefaultDbType,
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
    /// <param name="_db"></param>
    /// <param name="dbInfo"></param>
    /// <returns></returns>
    private static ISqlSugarClient GetSqlSugarClient(SqlSugarClient _db, SysTenantDataBaseModel dbInfo)
    {
        var connectionId = $"{dbInfo.SysDbType}_{dbInfo.TenantId}";

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
                throw Oops.Oh(ErrorCode.DbTypeError);
        }

        db.AddConnection(new ConnectionConfig
        {
            ConfigId = connectionId, // 此链接标志，用以后面切库使用
            ConnectionString = connectionStr, // 租户库连接字符串
            DbType = dbInfo.DbType,
            IsAutoCloseConnection = true, // 开启自动释放模式和EF原理一样我就不多解释了
            InitKeyType = InitKeyType.Attribute, // 从特性读取主键和自增列信息
            //InitKeyType = InitKeyType.SystemTable // 从数据库读取主键和自增列信息
        });

        var _db = db.GetConnection(connectionId);

        // 过滤器
        LoadSugarFilter(_db);
    }

    /// <summary>
    /// 反射获取所有的数据库Model Type
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<(string className, SysDataBaseTypeEnum dbType, Type type)> ReflexGetAllTEntityList()
    {
        // 程序入口Exe文件名称
        var appName = WebHostEnvironment.ApplicationName;

        // 先从缓存获取
        var entityType =
            _cache.Get<List<(string className, SysDataBaseTypeEnum dbType, Type type)>>(
                $"{appName}{CommonConst.CACHE_KEY_MODEL_DB_TYPE}");
        if (entityType != null && entityType.Any())
            return entityType;

        // 获取所有实现了接口的类的类型
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(sl => sl.GetTypes().Where(wh => wh.GetInterfaces().Contains(typeof(IDbEntity))));

        // 遍历获取到的类型集合
        // 获取数据库类型，如果没有则默认是Admin库
        entityType = types.Select(type => new ValueTuple<string, SysDataBaseTypeEnum, Type>(type.Name,
            type.GetCustomAttribute<DataBaseTypeAttribute>(true)?.SysDataBaseType ?? SysDataBaseTypeEnum.Admin, type)).ToList();
        // 放入缓存
        _cache.Set($"{appName}{CommonConst.CACHE_KEY_MODEL_DB_TYPE}", entityType);

        return entityType;
    }

    /// <summary>
    /// 反射获取所有的数据库Model
    /// </summary>
    /// <returns></returns>
    private static (string key, SysDataBaseTypeEnum dbType, Type type) ReflexGetAllTEntity(string name)
    {
        var entityInfo = ReflexGetAllTEntityList().FirstOrDefault(f => f.className == name);
        if (entityInfo.className.IsEmpty())
        {
            throw Oops.Oh(ErrorCode.SugarModelError);
        }

        return entityInfo;
    }

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
                    if (sql.Contains("[Sys_Log_Op]") || sql.Contains("[Sys_Log_Ex]"))
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

        // 这里可以根据字段判断也可以根据接口判断，如果是租户Id，建议根据接口判断，如果是IsDeleted，建议根据名称判断，方便别的表也可以实现
        foreach (var (_, _, type) in ReflexGetAllTEntityList())
        {
            // 配置多租户全局过滤器
            // 判断实体是否继承了租户基类接口
            if (type.GetInterfaces().Contains(typeof(IBaseTenant)))
            {
                // 构建动态Lambda
                var lambda = DynamicExpressionParser.ParseLambda(new[] {Expression.Parameter(type, "it")}, typeof(bool),
                    $"{nameof(BaseTenant.TenantId)} ==  @0 OR @1 ", GlobalContext.GetTenantId(false), GlobalContext.IsSuperAdmin);
                // 将Lambda传入过滤器
                _db.QueryFilter.Add(new TableFilterItem<object>(type, lambda));
            }

            // 配置加删除全局过滤器
            // 判断实体是否继承了基类
            // ReSharper disable once InvertIf
            if (!type.GetProperty(ClaimConst.DELETE_FIELD).IsEmpty())
            {
                // 构建动态Lambda
                var lambda = DynamicExpressionParser.ParseLambda(new[] {Expression.Parameter(type, "it")}, typeof(bool),
                    $"{nameof(BaseTEntity.IsDeleted)} ==  @0", false);
                // 将Lambda传入过滤器
                _db.QueryFilter.Add(new TableFilterItem<object>(type, lambda) {IsJoinQuery = true});
            }
        }
    }
}