using Fast.Core.AdminEnum;
using Fast.Core.AdminModel.Tenant;
using Fast.Core.RabbitMQ.EventSubscriber;
using Fast.Iaas.BaseModel.Interface;
using Furion.EventBus;
using Furion.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yitter.IdGenerator;
using SystemDbType = System.Data.DbType;

namespace Fast.Core.SqlSugar.Filter;

/// <summary>
/// Sugar实体类过滤器
/// </summary>
static class SugarEntityFilter
{
    /// <summary>
    /// 日志白名单表集合
    /// </summary>
    private static List<string> LogWhiteList =>
        new()
        {
            "Sys_Log_Op",
            "Sys_Log_Ex",
            "Sys_Log_Diff",
            "Sys_Log_Scheduler_Job",
            "Sys_Log_Vis"
        };

    /// <summary>
    /// 加载过滤器
    /// </summary>
    /// <param name="_db"></param>
    /// <param name="commandTimeOut">超时时间</param>
    /// <param name="sugarSqlExecMaxSeconds">SqlSugar Sql执行最大秒数，如果超过记录警告日志</param>
    /// <param name="diffLog">差异日志</param>
    public static void LoadSugarFilter(ISqlSugarClient _db, int commandTimeOut, double sugarSqlExecMaxSeconds, bool diffLog)
    {
        // 执行超时时间
        _db.Ado.CommandTimeOut = commandTimeOut;

        _db.Aop.OnLogExecuted = (sql, pars) =>
        {
            if (App.HostEnvironment.IsDevelopment())
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
                    if (LogWhiteList.Any(sql.Contains))
                    {
                        return;
                    }

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                }

                if (sql.StartsWith("DELETE"))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }

                Console.WriteLine($"\r\n\r\n{ParameterFormat(sql, pars)}\r\nTime：{_db.Ado.SqlExecutionTime}");
            }

            // 执行时间判断
            if (_db.Ado.SqlExecutionTime.TotalSeconds > sugarSqlExecMaxSeconds)
            {
                // 代码CS文件名称
                var fileName = _db.Ado.SqlStackTrace.FirstFileName;
                // 代码行数
                var fileLine = _db.Ado.SqlStackTrace.FirstLine;
                // 方法名称
                var firstMethodName = _db.Ado.SqlStackTrace.FirstMethodName;
                // 消息
                var message =
                    $"Sql执行时间超过 {sugarSqlExecMaxSeconds} 秒，建议优化。\r\nFileName：{fileName}\r\nFileLine：{fileLine}\r\nFirstMethodName：{firstMethodName}\r\nSql：{ParameterFormat(sql, pars)}\r\nSqlExecutionTime：{_db.Ado.SqlExecutionTime}";

                // 控制台输出
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\r\n\r\n{message}");

                // 写日志文件
                Log.Warning(message);
            }
        };

        _db.Aop.OnDiffLogEvent = diff =>
        {
            if (!diffLog)
                return;

            // 差异日志
            if ((diff.AfterData != null && diff.AfterData.Any()) || (diff.BeforeData != null && diff.BeforeData.Any()))
            {
                // 创建一个作用域
                Scoped.Create((_, scope) =>
                {
                    var serviceScope = scope.ServiceProvider;

                    // 获取事件总线服务
                    var _eventPublisher = serviceScope.GetService<IEventPublisher>();

                    var executeSql = ParameterFormat(diff.Sql, diff.Parameters);

                    DiffLogTableInfo firstData = null;
                    if (diff.AfterData != null && diff.AfterData.Any())
                    {
                        firstData = diff.AfterData.First();
                    }
                    else if (diff.BeforeData != null && diff.BeforeData.Any())
                    {
                        firstData = diff.BeforeData.First();
                    }

                    var tableName = firstData?.TableName;
                    var tableDescription = firstData?.TableDescription;

                    var diffLogType = diff.DiffType switch
                    {
                        DiffType.insert => DiffLogTypeEnum.Insert,
                        DiffType.update => DiffLogTypeEnum.Update,
                        DiffType.delete => DiffLogTypeEnum.Delete,
                        _ => DiffLogTypeEnum.None
                    };

                    var sysLogDiffModel = new SysLogDiffModel
                    {
                        Account = UserContext.UserAccount,
                        UserName = UserContext.UserName,
                        DiffDescription = diff.BusinessData.ToString(),
                        TableName = tableName,
                        TableDescription = tableDescription,
                        AfterColumnInfo = diff.AfterData?.Select(sl => sl.Columns).ToList(),
                        BeforeColumnInfo = diff.BeforeData?.Select(sl => sl.Columns).ToList(),
                        ExecuteSql = executeSql,
                        DiffType = diffLogType,
                        DiffTime = DateTime.Now
                    };
                    sysLogDiffModel.RecordCreate();

                    // 记录差异日志
                    _eventPublisher.PublishAsync(new FastChannelEventSource("Create:DiffLog", UserContext.TenantId,
                        sysLogDiffModel));
                });
            }
        };

        _db.Aop.OnError = exp =>
        {
            // 代码CS文件名称
            var fileName = _db.Ado.SqlStackTrace.FirstFileName;
            // 代码行数
            var fileLine = _db.Ado.SqlStackTrace.FirstLine;
            // 方法名称
            var firstMethodName = _db.Ado.SqlStackTrace.FirstMethodName;
            // 消息
            var message =
                $"Sql 执行异常\r\nFileName：{fileName}\r\nFileLine：{fileLine}\r\nFirstMethodName：{firstMethodName}\r\nSql：{ParameterFormat(exp.Sql, exp.Parametres)}";

            // 控制台输出
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\r\n\r\n{message}");

            // 写日志文件
            Log.Error(message);
        };


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
                        if (EntityValueCheck(nameof(IPrimaryKeyEntity<long>.Id), new List<dynamic> {null, 0}, entityInfo))
                            entityInfo.SetValue(YitIdHelper.NextId());
                    }

                    // 创建时间
                    SetEntityValue(nameof(IBaseEntity.CreatedTime), new List<dynamic> {null}, DateTime.Now, ref entityInfo);

                    // 租户Id
                    SetEntityValue(nameof(IBaseTenant.TenantId), new List<dynamic> {null, 0}, UserContext.TenantId,
                        ref entityInfo);

                    // 创建者Id
                    SetEntityValue(nameof(IBaseEntity.CreatedUserId), new List<dynamic> {null, 0}, UserContext.UserId,
                        ref entityInfo);

                    // 创建者名称
                    SetEntityValue(nameof(IBaseEntity.CreatedUserName), new List<dynamic> {null, ""}, UserContext.UserName,
                        ref entityInfo);

                    // 更新版本控制字段
                    SetEntityValue(nameof(IBaseEntity.UpdatedVersion), new List<dynamic> {null, 0}, 1, ref entityInfo);
                    break;
                // 更新操作
                case DataFilterType.UpdateByObject:
                    // 更新时间
                    SetEntityValue(nameof(IBaseEntity.UpdatedTime), null, DateTime.Now, ref entityInfo);

                    // 更新者Id
                    SetEntityValue(nameof(IBaseEntity.UpdatedUserId), null, UserContext.UserId, ref entityInfo);

                    // 更新者名称
                    SetEntityValue(nameof(IBaseEntity.UpdatedUserName), null, UserContext.UserName, ref entityInfo);
                    break;
            }
        };

        // 配置多租户全局过滤器
        if (!UserContext.IsSuperAdmin)
        {
            // TODO：这里可能由于SqlSugarBug问题，导致不能使用IF
            _db.QueryFilter.AddTableFilter<IBaseTenant>(it => it.TenantId == UserContext.TenantId);
        }

        // 配置加删除全局过滤器
        _db.QueryFilter.AddTableFilter<IBaseDeleted>(it => it.IsDeleted == false);
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
            dynamic value;
            switch (propertyName)
            {
                case nameof(IPrimaryKeyEntity<long>.Id):
                    value = dynamicEntityInfo.Id;
                    break;
                case nameof(IBaseTenant.TenantId):
                    value = dynamicEntityInfo.TenantId;
                    break;
                case nameof(IBaseEntity.CreatedTime):
                    value = dynamicEntityInfo.CreatedTime;
                    break;
                case nameof(IBaseEntity.CreatedUserId):
                    value = dynamicEntityInfo.CreatedUserId;
                    break;
                case nameof(IBaseEntity.CreatedUserName):
                    value = dynamicEntityInfo.CreatedUserName;
                    break;
                case nameof(IBaseEntity.UpdatedTime):
                    value = dynamicEntityInfo.UpdatedTime;
                    break;
                case nameof(IBaseEntity.UpdatedUserId):
                    value = dynamicEntityInfo.UpdatedUserId;
                    break;
                case nameof(IBaseEntity.UpdatedUserName):
                    value = dynamicEntityInfo.UpdatedUserName;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return emptyList == null || emptyList.Any(empty => empty == value);
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
                SystemDbType.String or SystemDbType.DateTime or SystemDbType.Date or SystemDbType.Time or SystemDbType.DateTime2
                    or SystemDbType.DateTimeOffset or SystemDbType.Guid or SystemDbType.VarNumeric
                    or SystemDbType.AnsiStringFixedLength or SystemDbType.AnsiString
                    or SystemDbType.StringFixedLength => sql.Replace(pars[i].ParameterName, "'" + pars[i].Value + "'"),
                SystemDbType.Boolean when string.IsNullOrEmpty(pars[i].Value.ToString()) => sql.Replace(pars[i].ParameterName,
                    "NULL"),
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