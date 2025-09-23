// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using System.Text;
using SqlSugar;
using Yitter.IdGenerator;

// ReSharper disable once CheckNamespace
namespace Fast.SqlSugar;

/// <summary>
/// <see cref="SugarEntityFilter"/> Sugar 实体过滤器
/// </summary>
[SuppressSniffer]
public static class SugarEntityFilter
{
    /// <summary>
    /// 加载 Sugar Aop
    /// </summary>
    /// <param name="isDevelopment"><see cref="bool"/> 是否为开发环境</param>
    /// <param name="_db"><see cref="ISqlSugarClient"/></param>
    /// <param name="sugarSqlExecMaxSeconds"><see cref="int"/> Sql最大执行秒数</param>
    /// <param name="diffLog"><see cref="bool"/> 是否启用差异日志</param>
    /// <param name="disableAop"><see cref="bool"/> 是否禁用Aop</param>
    /// <param name="sqlSugarEntityHandler"><see cref="ISqlSugarEntityHandler"/> Sugar实体处理 程序</param>
    public static void LoadSugarAop(bool isDevelopment, ISqlSugarClient _db, int? sugarSqlExecMaxSeconds = null,
        bool diffLog = false, bool disableAop = true, ISqlSugarEntityHandler sqlSugarEntityHandler = null)
    {
        sugarSqlExecMaxSeconds ??= SqlSugarContext.ConnectionSettings?.SugarSqlExecMaxSeconds!.Value;
        _db.Aop.OnLogExecuted = (rawSql, pars) =>
        {
            if (isDevelopment)
            {
                //// 如果是系统表则不输出，避免安全起见
                //if (rawSql.Contains("information_schema", StringComparison.OrdinalIgnoreCase))
                //    return;

                var handleSql = UtilMethods.GetSqlString(_db.CurrentConnectionConfig.DbType, rawSql, pars);

                var logSb = new StringBuilder();
                logSb.Append("\u001b[40m\u001b[90m");
                logSb.Append("fsql");
                logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                logSb.Append(": ");
                logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                logSb.Append(Environment.NewLine);
                logSb.Append("\u001b[40m\u001b[90m");
                logSb.Append("      ");
                logSb.Append($"Time: {_db.Ado.SqlExecutionTime}");
                logSb.Append(Environment.NewLine);
                logSb.Append("      ");
                logSb.Append(handleSql);
                logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                Console.WriteLine(logSb.ToString());
            }

            if (!disableAop && sqlSugarEntityHandler != null)
            {
                var handleSql = UtilMethods.GetSqlString(_db.CurrentConnectionConfig.DbType, rawSql, pars);

                // 执行Sql处理
                try
                {
                    sqlSugarEntityHandler.ExecuteAsync(rawSql, pars, _db.Ado.SqlExecutionTime, handleSql)
                        .GetAwaiter()
                        .GetResult();
                }
                catch (Exception ex)
                {
                    var logSb = new StringBuilder();
                    logSb.Append("\u001b[41m\u001b[30m");
                    logSb.Append("fsql");
                    logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                    logSb.Append(": ");
                    logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                    logSb.Append(Environment.NewLine);
                    logSb.Append("\u001b[41m\u001b[30m");
                    logSb.Append("      ");
                    logSb.Append("Exec [Fast.SqlSugar.ISqlSugarEntityHandler].[ExecuteAsync] method error.");
                    logSb.Append(Environment.NewLine);
                    logSb.Append(ex);
                    logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                    Console.WriteLine(logSb.ToString());
                }
            }

            // 执行时间判断
            if (_db.Ado.SqlExecutionTime.TotalSeconds > sugarSqlExecMaxSeconds)
            {
                var handleSql = UtilMethods.GetSqlString(_db.CurrentConnectionConfig.DbType, rawSql, pars);

                // 代码CS文件名称
                var fileName = _db.Ado.SqlStackTrace.FirstFileName;
                // 代码行数
                var fileLine = _db.Ado.SqlStackTrace.FirstLine;
                // 方法名称
                var firstMethodName = _db.Ado.SqlStackTrace.FirstMethodName;
                // 消息
                var message =
                    $"Sql执行时间超过 {sugarSqlExecMaxSeconds} 秒，建议优化。{Environment.NewLine}FileName：{fileName}{Environment.NewLine}FileLine：{fileLine}{Environment.NewLine}FirstMethodName：{firstMethodName}{Environment.NewLine}Sql：{handleSql}{Environment.NewLine}SqlExecutionTime：{_db.Ado.SqlExecutionTime}";

                // 控制台输出
                var logSb = new StringBuilder();
                logSb.Append("\u001b[40m\u001b[1m\u001b[33m");
                logSb.Append("fsql");
                logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                logSb.Append(": ");
                logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                logSb.Append(Environment.NewLine);
                logSb.Append("\u001b[40m\u001b[1m\u001b[33m");
                logSb.Append("      ");
                logSb.Append(message);
                logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                Console.WriteLine(logSb.ToString());

                if (!disableAop && sqlSugarEntityHandler != null)
                {
                    // 执行Sql超时处理
                    try
                    {
                        sqlSugarEntityHandler.ExecuteTimeoutAsync(fileName, fileLine, firstMethodName, rawSql, pars,
                                _db.Ado.SqlExecutionTime, handleSql, message)
                            .GetAwaiter()
                            .GetResult();
                    }
                    catch (Exception ex)
                    {
                        var _logSb = new StringBuilder();
                        _logSb.Append("\u001b[41m\u001b[30m");
                        _logSb.Append("fsql");
                        _logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                        _logSb.Append(": ");
                        _logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                        _logSb.Append(Environment.NewLine);
                        _logSb.Append("\u001b[41m\u001b[30m");
                        logSb.Append("      ");
                        _logSb.Append("Exec [Fast.SqlSugar.ISqlSugarEntityHandler].[ExecuteTimeoutAsync] method error.");
                        _logSb.Append(Environment.NewLine);
                        _logSb.Append(ex);
                        _logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                        Console.WriteLine(_logSb.ToString());
                    }
                }
            }
        };

        // 判断是否启用差异日志
        if (diffLog)
        {
            _db.Aop.OnDiffLogEvent = diff =>
            {
                if (!disableAop && sqlSugarEntityHandler != null)
                {
                    // 差异日志
                    if ((diff.AfterData != null && diff.AfterData.Any()) || (diff.BeforeData != null && diff.BeforeData.Any()))
                    {
                        var handleSql = UtilMethods.GetSqlString(_db.CurrentConnectionConfig.DbType, diff.Sql, diff.Parameters);

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

                        // 执行Sql差异处理
                        try
                        {
                            sqlSugarEntityHandler.ExecuteDiffLogAsync(diff.DiffType, tableName, tableDescription,
                                    diff.BusinessData, diff.BeforeData?.Select(sl => sl.Columns)
                                        .ToList(), diff.AfterData?.Select(sl => sl.Columns)
                                        .ToList(), diff.Sql, diff.Parameters, diff.Time, handleSql)
                                .GetAwaiter()
                                .GetResult();
                        }
                        catch (Exception ex)
                        {
                            var logSb = new StringBuilder();
                            logSb.Append("\u001b[41m\u001b[30m");
                            logSb.Append("fsql");
                            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                            logSb.Append(": ");
                            logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                            logSb.Append(Environment.NewLine);
                            logSb.Append("\u001b[41m\u001b[30m");
                            logSb.Append("      ");
                            logSb.Append("Exec [Fast.SqlSugar.ISqlSugarEntityHandler].[ExecuteDiffLogAsync] method error.");
                            logSb.Append(Environment.NewLine);
                            logSb.Append(ex);
                            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                            Console.WriteLine(logSb.ToString());
                        }
                    }
                }
            };
        }

        _db.Aop.OnError = exp =>
        {
            var param = (SugarParameter[]) exp.Parametres;

            var handleSql = UtilMethods.GetSqlString(_db.CurrentConnectionConfig.DbType, exp.Sql, param);

            // 代码CS文件名称
            var fileName = _db.Ado.SqlStackTrace.FirstFileName;
            // 代码行数
            var fileLine = _db.Ado.SqlStackTrace.FirstLine;
            // 方法名称
            var firstMethodName = _db.Ado.SqlStackTrace.FirstMethodName;

            if (isDevelopment)
            {
                var logSb = new StringBuilder();
                logSb.Append("\u001b[41m\u001b[30m");
                logSb.Append("fsql");
                logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                logSb.Append(": ");
                logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                logSb.Append(Environment.NewLine);
                logSb.Append("\u001b[41m\u001b[30m");
                logSb.Append("      ");
                logSb.Append(
                    $"Sql 执行异常{Environment.NewLine}FileName：{fileName}{Environment.NewLine}FileLine：{fileLine}{Environment.NewLine}FirstMethodName：{firstMethodName}{Environment.NewLine}Sql：{handleSql}");
                logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                Console.WriteLine(logSb.ToString());
            }

            if (!disableAop && sqlSugarEntityHandler != null)
            {
                // 执行Sql错误处理
                try
                {
                    sqlSugarEntityHandler.ExecuteErrorAsync(fileName, fileLine, firstMethodName, exp.Sql, param, handleSql, exp)
                        .GetAwaiter()
                        .GetResult();
                }
                catch (Exception ex)
                {
                    var logSb = new StringBuilder();
                    logSb.Append("\u001b[41m\u001b[30m");
                    logSb.Append("fsql");
                    logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                    logSb.Append(": ");
                    logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                    logSb.Append(Environment.NewLine);
                    logSb.Append("\u001b[41m\u001b[30m");
                    logSb.Append("      ");
                    logSb.Append("Exec [Fast.SqlSugar.ISqlSugarEntityHandler].[ExecuteErrorAsync] method error.");
                    logSb.Append(Environment.NewLine);
                    logSb.Append(ex);
                    logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                    Console.WriteLine(logSb.ToString());
                }
            }
        };

        // Model基类处理
        //_db.Aop.DataExecuting = (oldValue, entityInfo) =>
        _db.Aop.DataExecuting = (_, entityInfo) =>
        {
            switch (entityInfo.OperationType)
            {
                // 新增操作
                case DataFilterType.InsertByObject:
                    // 主键（long）赋值雪花Id，这里一条记录只会匹配一次
                    if (entityInfo.EntityColumnInfo.IsPrimarykey
                        && entityInfo.EntityColumnInfo.PropertyInfo.PropertyType == typeof(long))
                    {
                        if (SqlSugarContext.EntityValueCheck(nameof(IPrimaryKeyEntity<long>.Id), new List<dynamic> {null, 0},
                                entityInfo))
                        {
                            entityInfo.SetValue(YitIdHelper.NextId());
                        }
                    }

                    // 更新版本控制字段
                    SqlSugarContext.SetEntityValue(nameof(IUpdateVersion.RowVersion), new List<dynamic> {null, 0}, 1,
                        ref entityInfo);

                    // 创建时间
                    SqlSugarContext.SetEntityValue(nameof(IBaseEntity.CreatedTime), new List<dynamic> {null}, DateTime.Now,
                        ref entityInfo);

                    // 其余字段判断
                    if (sqlSugarEntityHandler != null)
                    {
                        // 部门Id
                        SqlSugarContext.SetEntityValue(nameof(IBaseEntity.DepartmentId), new List<dynamic> {null, 0},
                            sqlSugarEntityHandler.AssignDepartmentId(), ref entityInfo);

                        // 部门名称
                        SqlSugarContext.SetEntityValue(nameof(IBaseEntity.DepartmentName), new List<dynamic> {null, ""},
                            sqlSugarEntityHandler.AssignDepartmentName(), ref entityInfo);

                        // 创建者Id
                        SqlSugarContext.SetEntityValue(nameof(IBaseEntity.CreatedUserId), new List<dynamic> {null, 0},
                            sqlSugarEntityHandler.AssignUserId(), ref entityInfo);

                        // 创建者名称
                        SqlSugarContext.SetEntityValue(nameof(IBaseEntity.CreatedUserName), new List<dynamic> {null, ""},
                            sqlSugarEntityHandler.AssignUserName(), ref entityInfo);

                        // 租户Id
                        SqlSugarContext.SetEntityValue(nameof(IBaseTEntity.TenantId), new List<dynamic> {null, 0},
                            sqlSugarEntityHandler.AssignTenantId() ?? 0, ref entityInfo);
                    }

                    break;
                // 更新操作
                case DataFilterType.UpdateByObject:
                    // 更新时间
                    SqlSugarContext.SetEntityValue(nameof(IBaseEntity.UpdatedTime), null, DateTime.Now, ref entityInfo);

                    // 其余字段判断
                    if (sqlSugarEntityHandler != null)
                    {
                        // 更新者Id
                        SqlSugarContext.SetEntityValue(nameof(IBaseEntity.UpdatedUserId), new List<dynamic> {null, 0},
                            sqlSugarEntityHandler.AssignUserId(), ref entityInfo);

                        // 更新者名称
                        SqlSugarContext.SetEntityValue(nameof(IBaseEntity.UpdatedUserName), new List<dynamic> {null, ""},
                            sqlSugarEntityHandler.AssignUserName(), ref entityInfo);
                    }

                    break;
                case DataFilterType.DeleteByObject:
                    break;
            }
        };
    }

    /// <summary>
    /// 加载Sugar过滤器
    /// </summary>
    /// <param name="_db"><see cref="ISqlSugarClient"/></param>
    /// <param name="sqlSugarEntityHandler"><see cref="ISqlSugarEntityHandler"/> Sugar实体处理 程序</param>
    public static void LoadSugarFilter(ISqlSugarClient _db, ISqlSugarEntityHandler sqlSugarEntityHandler)
    {
        if (sqlSugarEntityHandler != null)
        {
            // 配置多租户全局过滤器
            if (!sqlSugarEntityHandler.IsSuperAdmin())
            {
                _db.QueryFilter.AddTableFilter<IBaseTEntity>(it => it.TenantId == (sqlSugarEntityHandler.AssignTenantId() ?? 0));
            }
        }

        // 配置软删除全局过滤器
        _db.QueryFilter.AddTableFilter<IDeletedEntity>(it => it.IsDeleted == false);
    }
}