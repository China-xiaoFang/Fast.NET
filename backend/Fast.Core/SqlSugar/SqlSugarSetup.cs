using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Configuration;
using SystemDbType = System.Data.DbType;

namespace Fast.Core.SqlSugar;

/// <summary>
/// SqlSugar设置/配置/入口
/// </summary>
public static class SqlSugarSetup
{
    /// <summary>
    /// 缓存 Entity 类型
    /// </summary>
    private static List<(string className, SysDataBaseTypeEnum dbType, Type type)> _cacheEntityTypeList { get; set; }

    /// <summary>
    /// 数据库配置
    /// </summary>
    private static readonly ConnectionStringsOptions connectionStringsOptions =
        Configuration.GetSection("ConnectionStrings").Get<ConnectionStringsOptions>();

    /// <summary>
    /// SqlSugarClient的配置
    /// Client不能单例注入
    /// </summary>
    public static void SqlSugarClientConfigure(this IServiceCollection services)
    {
        // 清除ModelType缓存
        _cacheEntityTypeList = null;

        // 得到连接字符串
        var connectionStr = DataBaseHelper.GetConnectionStr(new SysTenantDataBaseModel
        {
            ServiceIp = connectionStringsOptions.DefaultServiceIp,
            Port = connectionStringsOptions.DefaultPort,
            DbName = connectionStringsOptions.DefaultDbName,
            DbUser = connectionStringsOptions.DefaultDbUser,
            DbPwd = connectionStringsOptions.DefaultDbPwd,
            SysDbType = SysDataBaseTypeEnum.Admin,
            DbType = connectionStringsOptions.DefaultDbType
        });

        var connectConfig = new ConnectionConfig
        {
            ConfigId = connectionStringsOptions.DefaultConnectionId, // 此链接标志，用以后面切库使用
            ConnectionString = connectionStr, // 核心库连接字符串
            DbType = connectionStringsOptions.DefaultDbType,
            IsAutoCloseConnection = true, // 开启自动释放模式和EF原理一样我就不多解释了
            InitKeyType = InitKeyType.Attribute, // 从特性读取主键和自增列信息
            //InitKeyType = InitKeyType.SystemTable // 从数据库读取主键和自增列信息
            ConfigureExternalServices = DataBaseHelper.GetSugarExternalServices(connectionStringsOptions.DefaultDbType)
        };

        // 注册 SqlSugarClient
        services.AddScoped<ISqlSugarClient>(_ =>
        {
            var sqlSugarClient = new SqlSugarClient(connectConfig);
            // 过滤器
            EntityFilter.LoadSugarFilter(sqlSugarClient);

            return sqlSugarClient;
        });

        // 注册非泛型仓储
        services.AddScoped<ISqlSugarRepository, SqlSugarRepository>();

        // 注册 SqlSugar 
        services.AddScoped(typeof(ISqlSugarRepository<>), typeof(SqlSugarRepository<>));
    }

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

        var dbType = EntityHelper.ReflexGetAllTEntity(typeof(TEntity).Name);

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
                GlobalContext.TenantDbInfo =
                    SqlSugarClientHelper.GetDbInfo(defaultDb, SysDataBaseTypeEnum.Tenant, tenantId.ParseToLong());

                return SqlSugarClientHelper.GetSqlSugarClient(_db, GlobalContext.TenantDbInfo);
            default:
                throw Oops.Oh(ErrorCode.SugarModelError);
        }
    }

    /// <summary>
    /// SqlSugarClient帮助类
    /// </summary>
    private static class SqlSugarClientHelper
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
            var _cache = GetService<ICache>();
            // 数据库信息缓存
            var dbInfoList = _cache.Get<List<SysTenantDataBaseModel>>($"{CommonConst.CACHE_KEY_TENANT_DB_INFO}{tenantId}");
            if (dbInfoList == null || !dbInfoList.Any())
            {
                dbInfoList = _db.Queryable<SysTenantDataBaseModel>().Where(wh => wh.TenantId == tenantId).Filter(null, true)
                    .ToList();
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
            EntityFilter.LoadSugarFilter(_db);

            return _db;
        }
    }

    /// <summary>
    /// 实体类帮助类
    /// </summary>
    public static class EntityHelper
    {
        /// <summary>
        /// 反射获取所有的数据库Model Type
        /// </summary>
        /// <returns></returns>
        public static List<(string className, SysDataBaseTypeEnum dbType, Type type)> ReflexGetAllTEntityList()
        {
            var excludeBaseTypes = new List<Type>
            {
                typeof(IBaseEntity),
                typeof(IBaseLogEntity),
                typeof(IBaseTenant),
                typeof(AutoIncrementEntity),
                typeof(BaseEntity),
                typeof(BaseLogEntity),
                typeof(BaseTenant),
                typeof(BaseTEntity),
                typeof(PrimaryKeyEntity),
            };

            // 先从缓存获取
            var entityTypeList = _cacheEntityTypeList;
            if (entityTypeList != null && entityTypeList.Any())
                return entityTypeList;

            // 获取所有实现了接口的类的类型
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(sl => sl.GetTypes().Where(wh => wh.GetInterfaces().Contains(typeof(IDbEntity))))
                // 排除BaseEntity
                .Where(wh => !excludeBaseTypes.Contains(wh));

            // 遍历获取到的类型集合
            // 获取数据库类型，如果没有则默认是Admin库
            entityTypeList = types.Select(type => new ValueTuple<string, SysDataBaseTypeEnum, Type>(type.Name,
                    type.GetCustomAttribute<DataBaseTypeAttribute>(true)?.SysDataBaseType ?? SysDataBaseTypeEnum.Admin, type))
                .ToList();
            // 放入缓存
            _cacheEntityTypeList = entityTypeList;

            return entityTypeList;
        }

        /// <summary>
        /// 反射获取所有的数据库Model
        /// </summary>
        /// <returns></returns>
        public static (string key, SysDataBaseTypeEnum dbType, Type type) ReflexGetAllTEntity(string name)
        {
            var entityType = ReflexGetAllTEntityList().FirstOrDefault(f => f.className == name);
            if (entityType.className.IsEmpty())
            {
                throw Oops.Oh(ErrorCode.SugarModelError);
            }

            return entityType;
        }
    }

    /// <summary>
    /// 实体类过滤器
    /// </summary>
    private static class EntityFilter
    {
        /// <summary>
        /// 加载过滤器
        /// </summary>
        /// <param name="_db"></param>
        public static void LoadSugarFilter(ISqlSugarClient _db)
        {
            // 执行超时时间 60秒
            _db.Ado.CommandTimeOut = 60;

            if (HostEnvironment.IsDevelopment())
            {
                _db.Aop.OnLogExecuted = (sql, pars) =>
                {
                    if (sql.StartsWith("SELECT"))
                    {
                        // 如果是系统表则不输出，避免安全起见
                        if (sql.Contains("information_schema.TABLES"))
                            return;
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                    }

                    if (sql.StartsWith("UPDATE") || sql.StartsWith("INSERT"))
                    {
                        // 如果是两个Log表，则不输出
                        if (sql.Contains("[Sys_Log_Op]") || sql.Contains("[Sys_Log_Ex]"))
                            return;
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    }

                    if (sql.StartsWith("DELETE"))
                        Console.ForegroundColor = ConsoleColor.Blue;

                    PrintToMiniProfiler("SqlSugar", "Info", ParameterFormat(sql, pars));
                    Console.WriteLine($"\r\n\r\n{ParameterFormat(sql, pars)}\r\nTime：{_db.Ado.SqlExecutionTime}");
                };

                _db.Aop.OnError = exp =>
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\r\n\r\n错误 Sql语句：{ParameterFormat(exp.Sql, exp.Parametres)}");
                };
            }

            // Model基类处理
            _db.Aop.DataExecuting = (oldValue, entityInfo) =>
            {
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (entityInfo.OperationType)
                {
                    // 新增操作
                    case DataFilterType.InsertByObject:
                        // 主键（long）赋值雪花Id
                        if (entityInfo.EntityColumnInfo.IsPrimarykey &&
                            entityInfo.EntityColumnInfo.PropertyInfo.PropertyType == typeof(long))
                        {
                            if (EntityValueCheck(ClaimConst.ID_FIELD, new List<dynamic> {null, 0}, entityInfo))
                                entityInfo.SetValue(YitIdHelper.NextId());
                        }

                        // 创建时间
                        SetEntityValue(ClaimConst.CREATEDTIME_FIELD, new List<dynamic> {null}, DateTime.Now, ref entityInfo);

                        // 判断是否存在用户信息
                        if (User != null)
                        {
                            // 租户Id
                            SetEntityValue(ClaimConst.TENANTID_FIELD, new List<dynamic> {null, 0}, GlobalContext.TenantId,
                                ref entityInfo);

                            // 创建者Id
                            SetEntityValue(ClaimConst.CREATEDUSERID_FIELD, new List<dynamic> {null, 0}, GlobalContext.UserId,
                                ref entityInfo);

                            // 创建者名称
                            SetEntityValue(ClaimConst.CREATEDUSERNAME_FIELD, new List<dynamic> {null, ""}, GlobalContext.UserName,
                                ref entityInfo);
                        }

                        break;
                    // 更新操作
                    case DataFilterType.UpdateByObject:
                        // 更新时间
                        SetEntityValue(ClaimConst.UPDATEDTIME_FIELD, new List<dynamic> {null}, DateTime.Now, ref entityInfo);

                        // 更新者Id
                        SetEntityValue(ClaimConst.UPDATEDUSERID_FIELD, new List<dynamic> {null, 0}, GlobalContext.UserId,
                            ref entityInfo);

                        // 更新者名称
                        SetEntityValue(ClaimConst.UPDATEDUSERNAME_FIELD, new List<dynamic> {null, ""}, GlobalContext.UserName,
                            ref entityInfo);
                        break;
                }
            };

            // 这里可以根据字段判断也可以根据接口判断，如果是租户Id，建议根据接口判断，如果是IsDeleted，建议根据名称判断，方便别的表也可以实现
            foreach (var (_, _, type) in EntityHelper.ReflexGetAllTEntityList())
            {
                // 配置多租户全局过滤器
                // 判断实体是否继承了租户基类接口
                if (type.GetInterfaces().Contains(typeof(IBaseTenant)))
                {
                    // 构建动态Lambda
                    var lambda = DynamicExpressionParser.ParseLambda(new[] {Expression.Parameter(type, "it")}, typeof(bool),
                        $"{nameof(BaseTenant.TenantId)} == @0 OR @1 ", GlobalContext.GetTenantId(false),
                        GlobalContext.IsSuperAdmin);
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
                        $"{nameof(BaseTEntity.IsDeleted)} == @0", false);
                    // 将Lambda传入过滤器
                    _db.QueryFilter.Add(new TableFilterItem<object>(type, lambda) {IsJoinQuery = true});
                }
            }
        }

        /// <summary>
        /// 设置Entity Value
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="emptyList"></param>
        /// <param name="setValue"></param>
        /// <param name="entityInfo"></param>
        /// <param name="propertyName"></param>
        private static void SetEntityValue(string fieldName, ICollection<dynamic> emptyList, dynamic setValue,
            ref DataFilterModel entityInfo, string propertyName = null)
        {
            // 属性名称如果为Null，则默认为字段名称
            propertyName ??= fieldName;

            // 判断属性名称是否等于传入的字段名称
            if (entityInfo.PropertyName != fieldName)
                return;
            if (EntityValueCheck(propertyName, emptyList, entityInfo))
                entityInfo.SetValue(setValue);
        }

        /// <summary>
        /// Entity Value 检测
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="emptyList"></param>
        /// <param name="entityInfo"></param>
        /// <returns></returns>
        private static bool EntityValueCheck(string propertyName, ICollection<dynamic> emptyList, DataFilterModel entityInfo)
        {
            try
            {
                // 转换为动态类型
                var dynamicEntityInfo = (dynamic) entityInfo.EntityValue;
                var value = propertyName switch
                {
                    ClaimConst.ID_FIELD => dynamicEntityInfo.Id,
                    ClaimConst.CLAINM_TENANTID => dynamicEntityInfo.TenantId,
                    ClaimConst.CREATEDTIME_FIELD => dynamicEntityInfo.CreatedTime,
                    ClaimConst.CREATEDUSERID_FIELD => dynamicEntityInfo.CreatedUserId,
                    ClaimConst.CREATEDUSERNAME_FIELD => dynamicEntityInfo.CreatedUserName,
                    ClaimConst.UPDATEDTIME_FIELD => dynamicEntityInfo.UpdatedTime,
                    ClaimConst.UPDATEDUSERID_FIELD => dynamicEntityInfo.UpdatedUserId,
                    ClaimConst.UPDATEDUSERNAME_FIELD => dynamicEntityInfo.UpdatedUserName,
                    _ => throw new NotImplementedException(),
                };
                return emptyList.Any(empty => empty == value);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 格式化参数拼接成完整的SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        private static string ParameterFormat(string sql, IReadOnlyList<SugarParameter> pars)
        {
            //应逆向替换，否则由于 SqlSugar 多个表的过滤器问题导致替换不完整  如 @TenantId1  @TenantId10
            for (var i = pars.Count - 1; i >= 0; i--)
            {
                sql = pars[i].DbType switch
                {
                    SystemDbType.String or SystemDbType.DateTime or SystemDbType.Date or SystemDbType.Time
                        or SystemDbType.DateTime2 or SystemDbType.DateTimeOffset or SystemDbType.Guid or SystemDbType.VarNumeric
                        or SystemDbType.AnsiStringFixedLength or SystemDbType.AnsiString
                        or SystemDbType.StringFixedLength => sql.Replace(pars[i].ParameterName, "'" + pars[i].Value + "'"),
                    SystemDbType.Boolean when pars[i].Value.IsEmpty() => sql.Replace(pars[i].ParameterName, "NULL"),
                    SystemDbType.Boolean => sql.Replace(pars[i].ParameterName, Convert.ToBoolean(pars[i].Value) ? "1" : "0"),
                    _ => sql.Replace(pars[i].ParameterName, pars[i].Value?.ToString())
                };
            }

            return sql;
        }

        /// <summary>
        /// 格式化参数拼接成完整的SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        private static string ParameterFormat(string sql, object pars)
        {
            var param = (SugarParameter[]) pars;
            return ParameterFormat(sql, param);
        }
    }

    /// <summary>
    /// 数据库帮助类
    /// 各种类型数据库的兼容
    /// </summary>
    private static class DataBaseHelper
    {
        /// <summary>
        /// 得到数据库连接字符串
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public static string GetConnectionStr(SysTenantDataBaseModel dbInfo)
        {
            var connectionStr = "";
            // 目前暂时支持Sql Server 和 MySql
            switch (dbInfo.DbType)
            {
                case DbType.MySql:
                    connectionStr =
                        $"Data Source={dbInfo.ServiceIp};Database={dbInfo.DbName};User ID={dbInfo.DbUser};Password={dbInfo.DbPwd};pooling=true;port={dbInfo.Port};sslmode=none;CharSet=utf8;Convert Zero Datetime=True;Allow Zero Datetime=True;";
                    break;
                case DbType.SqlServer:
                    connectionStr =
                        $"Data Source={dbInfo.ServiceIp};Initial Catalog={dbInfo.DbName};User ID={dbInfo.DbUser};Password={dbInfo.DbPwd};MultipleActiveResultSets=true;max pool size=100;";
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
                case DbType.QuestDB:
                case DbType.HG:
                case DbType.ClickHouse:
                default:
                    throw Oops.Oh(ErrorCode.DbTypeError);
            }

            return connectionStr;
        }

        /// <summary>
        /// 目前暂时支持Sql Server 和 MySql
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static ConfigureExternalServices GetSugarExternalServices(DbType dbType)
        {
            var externalServices = new ConfigureExternalServices
            {
                EntityNameService = (type, entityInfo) =>
                {
                    // 全局开启创建表按照字段排序，避免重复代码
                    entityInfo.IsCreateTableFiledSort = true;

                    // Table Name 配置，如果使用SqlSugar的规范，其实这里是不会走的
                    var tableAttribute = type.GetCustomAttribute<TableAttribute>();
                    if (tableAttribute != null)
                    {
                        entityInfo.DbTableName = tableAttribute.Name;
                    }
                },
                EntityService = (propertyInfo, columnInfo) =>
                {
                    // 主键配置，如果使用SqlSugar的规范，其实这里是不会走的
                    var keyAttribute = propertyInfo.GetCustomAttribute<KeyAttribute>();
                    if (keyAttribute != null)
                    {
                        columnInfo.IsPrimarykey = true;
                    }

                    // 列名配置，如果使用SqlSugar的规范，其实这里是不会走的
                    var columnAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>();
                    if (columnAttribute != null)
                    {
                        columnInfo.DbColumnName = columnAttribute.Name;
                    }

                    // 可空类型配置
                    if (propertyInfo.PropertyType.IsGenericType &&
                        propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        columnInfo.IsNullable = true;
                    }

                    // 这里的所有数据库类型，默认是根据SqlServer配置的
                    var columnDbType = columnInfo.DataType?.ToUpper();
                    if (columnDbType == null)
                        return;
                    // String
                    if (columnDbType.StartsWith("NVARCHAR"))
                    {
                        var length = columnDbType.Substring("NVARCHAR(".Length, columnDbType.Length - "NVARCHAR(".Length - 1);
                        SetDbTypeNvarchar(dbType, length, ref columnInfo);
                    }

                    // DateTime
                    if (columnDbType == "DATETIMEOFFSET")
                    {
                        SetDbTypeDateTime(dbType, ref columnInfo);
                    }
                }
            };
            return externalServices;
        }

        /// <summary>
        /// 设置Nvarchar类型
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="length"></param>
        /// <param name="columnInfo"></param>
        private static void SetDbTypeNvarchar(DbType dbType, string length, ref EntityColumnInfo columnInfo)
        {
            switch (dbType)
            {
                case DbType.MySql:
                    columnInfo.DataType = length == "MAX" ? "longtext" : $"varchar({length})";
                    break;
                case DbType.SqlServer:
                    columnInfo.DataType = $"Nvarchar({length})";
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
                case DbType.QuestDB:
                    break;
                case DbType.HG:
                    break;
                case DbType.ClickHouse:
                    break;
                case DbType.Custom:
                    break;
                default:
                    throw Oops.Oh(ErrorCode.DbTypeError);
            }
        }

        /// <summary>
        /// 设置DateTime类型
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="columnInfo"></param>
        private static void SetDbTypeDateTime(DbType dbType, ref EntityColumnInfo columnInfo)
        {
            switch (dbType)
            {
                case DbType.MySql:
                    columnInfo.DataType = "datetime";
                    break;
                case DbType.SqlServer:
                    columnInfo.DataType = "datetimeoffset";
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
                case DbType.QuestDB:
                    break;
                case DbType.HG:
                    break;
                case DbType.ClickHouse:
                    break;
                case DbType.Custom:
                    break;
                default:
                    throw Oops.Oh(ErrorCode.DbTypeError);
            }
        }
    }
}

/// <summary>
/// SqlSugar 拓展类
/// </summary>
public static class Extensions
{
    /// <summary>
    /// 获取SugarTable特性中的TableName
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetSugarTableName(this Type type)
    {
        var sugarTable = type.GetCustomAttribute<SugarTable>();
        if (sugarTable != null && !sugarTable.TableName.IsEmpty())
        {
            return sugarTable.TableName;
        }

        return type.Name;
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static SqlSugarPagedList<TEntity> ToPagedList<TEntity>(this ISugarQueryable<TEntity> queryable, int pageIndex,
        int pageSize)
    {
        var totalCount = 0;
        var items = queryable.ToPageList(pageIndex, pageSize, ref totalCount);
        var totalPages = (int) Math.Ceiling(totalCount / (double) pageSize);
        return new SqlSugarPagedList<TEntity>
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Items = items,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNextPages = pageIndex < totalPages,
            HasPrevPages = pageIndex - 1 > 0
        };
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static async Task<SqlSugarPagedList<TEntity>> ToPagedListAsync<TEntity>(this ISugarQueryable<TEntity> queryable,
        int pageIndex, int pageSize)
    {
        RefAsync<int> totalCount = 0;
        var items = await queryable.ToPageListAsync(pageIndex, pageSize, totalCount);
        var totalPages = (int) Math.Ceiling(totalCount / (double) pageSize);
        return new SqlSugarPagedList<TEntity>
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Items = items,
            TotalCount = (int) totalCount,
            TotalPages = totalPages,
            HasNextPages = pageIndex < totalPages,
            HasPrevPages = pageIndex - 1 > 0
        };
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static SqlSugarPagedList<TResult> ToPagedList<TEntity, TResult>(this ISugarQueryable<TEntity> queryable, int pageIndex,
        int pageSize, Expression<Func<TEntity, TResult>> expression)
    {
        var totalCount = 0;
        var items = queryable.ToPageList(pageIndex, pageSize, ref totalCount, expression);
        var totalPages = (int) Math.Ceiling(totalCount / (double) pageSize);
        return new SqlSugarPagedList<TResult>
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Items = items,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNextPages = pageIndex < totalPages,
            HasPrevPages = pageIndex - 1 > 0
        };
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static async Task<SqlSugarPagedList<TResult>> ToPagedListAsync<TEntity, TResult>(
        this ISugarQueryable<TEntity> queryable, int pageIndex, int pageSize, Expression<Func<TEntity, TResult>> expression)
    {
        RefAsync<int> totalCount = 0;
        var items = await queryable.ToPageListAsync(pageIndex, pageSize, totalCount, expression);
        var totalPages = (int) Math.Ceiling(totalCount / (double) pageSize);
        return new SqlSugarPagedList<TResult>
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Items = items,
            TotalCount = (int) totalCount,
            TotalPages = totalPages,
            HasNextPages = pageIndex < totalPages,
            HasPrevPages = pageIndex - 1 > 0
        };
    }
}

/// <summary>
/// SqlSugar 分页泛型集合
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class SqlSugarPagedList<TEntity>
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// 页容量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 总条数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// 当前页集合
    /// </summary>
    public IEnumerable<TEntity> Items { get; set; }

    /// <summary>
    /// 是否有上一页
    /// </summary>
    public bool HasPrevPages { get; set; }

    /// <summary>
    /// 是否有下一页
    /// </summary>
    public bool HasNextPages { get; set; }
}