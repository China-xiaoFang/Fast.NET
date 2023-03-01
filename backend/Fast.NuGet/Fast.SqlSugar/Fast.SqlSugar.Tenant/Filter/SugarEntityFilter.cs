using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Fast.SqlSugar.Tenant.BaseModel;
using Fast.SqlSugar.Tenant.BaseModel.Interface;
using Fast.SqlSugar.Tenant.Const;
using Fast.SqlSugar.Tenant.Helper;
using Microsoft.Extensions.Hosting;
using SqlSugar;
using Yitter.IdGenerator;
using SystemDbType = System.Data.DbType;

namespace Fast.SqlSugar.Tenant.Filter;

/// <summary>
/// Sugar实体类过滤器
/// </summary>
static class SugarEntityFilter
{
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
            if (SugarContext.HostEnvironment.IsDevelopment())
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
                SugarContext.Log.Warning(message);
            }
        };

        _db.Aop.OnDiffLogEvent = diff =>
        {
            if (diffLog)
            {
                var executeSql = ParameterFormat(diff.Sql, diff.Parameters);

                // 差异日志
                SugarContext.Log.DiffLog(diff.BusinessData.ToString(), diff.AfterData, diff.BeforeData, executeSql, diff.DiffType,
                    DateTime.Now);
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
            SugarContext.Log.Error(message);
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
                        if (EntityValueCheck(SugarFieldConst.Id, new List<dynamic> {null, 0}, entityInfo))
                            entityInfo.SetValue(YitIdHelper.NextId());
                    }

                    // 创建时间
                    SetEntityValue(SugarFieldConst.CreatedTime, new List<dynamic> {null}, DateTime.Now, ref entityInfo);

                    // 租户Id
                    SetEntityValue(SugarFieldConst.TenantId, new List<dynamic> {null, 0}, SugarContext.TenantId, ref entityInfo);

                    // 创建者Id
                    SetEntityValue(SugarFieldConst.CreatedUserId, new List<dynamic> {null, 0}, SugarContext.UserId,
                        ref entityInfo);

                    // 创建者名称
                    SetEntityValue(SugarFieldConst.CreatedUserName, new List<dynamic> {null, ""}, SugarContext.UserName,
                        ref entityInfo);

                    // 更新版本控制字段
                    SetEntityValue(SugarFieldConst.UpdatedVersion, new List<dynamic> {null, 0}, 1, ref entityInfo);
                    break;
                // 更新操作
                case DataFilterType.UpdateByObject:
                    // 更新时间
                    SetEntityValue(SugarFieldConst.UpdatedTime, null, DateTime.Now, ref entityInfo);

                    // 更新者Id
                    SetEntityValue(SugarFieldConst.UpdatedUserId, null, SugarContext.UserId, ref entityInfo);

                    // 更新者名称
                    SetEntityValue(SugarFieldConst.UpdatedUserName, null, SugarContext.UserName, ref entityInfo);
                    break;
            }
        };

        // 这里可以根据字段判断也可以根据接口判断，如果是租户Id，建议根据接口判断，如果是IsDeleted，建议根据名称判断，方便别的表也可以实现
        foreach (var entityType in EntityHelper.ReflexGetAllTEntityList())
        {
            // 配置多租户全局过滤器
            // 判断实体是否继承了租户基类接口
            if (entityType.Type.GetInterfaces().Contains(typeof(IBaseTenant)))
            {
                // 构建动态Lambda
                var lambda = DynamicExpressionParser.ParseLambda(new[] {Expression.Parameter(entityType.Type, "it")},
                    typeof(bool), $"{nameof(BaseTenant.TenantId)} == @0 OR @1 ", SugarContext.GetTenantId(false),
                    SugarContext.IsSuperAdmin);
                // 将Lambda传入过滤器
                _db.QueryFilter.Add(new TableFilterItem<object>(entityType.Type, lambda));
            }

            // 配置加删除全局过滤器
            // 判断实体是否继承了基类
            // ReSharper disable once InvertIf
            if (!string.IsNullOrEmpty(entityType.Type.GetProperty(SugarFieldConst.IsDeleted)?.ToString()))
            {
                // 构建动态Lambda
                var lambda = DynamicExpressionParser.ParseLambda(new[] {Expression.Parameter(entityType.Type, "it")},
                    typeof(bool), $"{nameof(BaseTEntity.IsDeleted)} == @0", false);
                // 将Lambda传入过滤器
                _db.QueryFilter.Add(new TableFilterItem<object>(entityType.Type, lambda) {IsJoinQuery = true});
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
                SugarFieldConst.Id => dynamicEntityInfo.Id,
                SugarFieldConst.TenantId => dynamicEntityInfo.TenantId,
                SugarFieldConst.CreatedTime => dynamicEntityInfo.CreatedTime,
                SugarFieldConst.CreatedUserId => dynamicEntityInfo.CreatedUserId,
                SugarFieldConst.CreatedUserName => dynamicEntityInfo.CreatedUserName,
                SugarFieldConst.UpdatedTime => dynamicEntityInfo.UpdatedTime,
                SugarFieldConst.UpdatedUserId => dynamicEntityInfo.UpdatedUserId,
                SugarFieldConst.UpdatedUserName => dynamicEntityInfo.UpdatedUserName,
                _ => throw new NotImplementedException(),
            };
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