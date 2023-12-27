// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Fast.SqlSugar.Handlers;
using Fast.SqlSugar.IBaseEntities;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.SqlSugar.Filters;

/// <summary>
/// <see cref="SugarEntityFilter"/> Sugar实体过滤器
/// </summary>
internal static class SugarEntityFilter
{
    /// <summary>
    /// 加载 Sugar Aop
    /// </summary>
    /// <param name="_db"></param>
    /// <param name="sugarSqlExecMaxSeconds"><see cref="double"/> Sql最大执行秒数</param>
    /// <param name="diffLog"><see cref="bool"/> 是否启用差异日志</param>
    /// <param name="sqlSugarEntityHandler"><see cref="ISqlSugarEntityHandler"/> Sugar实体处理</param>
    internal static void LoadSugarAop(ISqlSugarClient _db, double sugarSqlExecMaxSeconds, bool diffLog,
        ISqlSugarEntityHandler sqlSugarEntityHandler)
    {
        _db.Aop.OnLogExecuted = (rawSql, pars) =>
        {
            var handleSql = SqlSugarContext.ParameterFormat(rawSql, pars);

#if DEBUG
            if (rawSql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                // 如果是系统表则不输出，避免安全起见
                if (rawSql.Contains("information_schema.TABLES", StringComparison.OrdinalIgnoreCase))
                    return;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }

            if (rawSql.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase) ||
                rawSql.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
            }

            if (rawSql.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }

            Console.WriteLine($"\r\n\r\n{handleSql}\r\nTime：{_db.Ado.SqlExecutionTime}");
#endif

            if (sqlSugarEntityHandler != null)
            {
                // 执行Sql处理
                Task.Run(async () =>
                {
                    await sqlSugarEntityHandler.ExecuteAsync(rawSql, pars, _db.Ado.SqlExecutionTime, handleSql);
                }).Wait();
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
                    $"Sql执行时间超过 {sugarSqlExecMaxSeconds} 秒，建议优化。\r\nFileName：{fileName}\r\nFileLine：{fileLine}\r\nFirstMethodName：{firstMethodName}\r\nSql：{handleSql}\r\nSqlExecutionTime：{_db.Ado.SqlExecutionTime}";

                // 控制台输出
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\r\n\r\n{message}");

                if (sqlSugarEntityHandler != null)
                {
                    // 执行Sql超时处理
                    Task.Run(async () =>
                    {
                        await sqlSugarEntityHandler.ExecuteTimeoutAsync(fileName, fileLine, firstMethodName, rawSql, pars,
                            _db.Ado.SqlExecutionTime, handleSql, message);
                    }).Wait();
                }
            }
        };

        // 判断是否启用差异日志
        if (diffLog)
        {
            _db.Aop.OnDiffLogEvent = diff =>
            {
                if (sqlSugarEntityHandler != null)
                {
                    // 差异日志
                    if ((diff.AfterData != null && diff.AfterData.Any()) || (diff.BeforeData != null && diff.BeforeData.Any()))
                    {
                        var handleSql = SqlSugarContext.ParameterFormat(diff.Sql, diff.Parameters);

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
                        Task.Run(async () =>
                        {
                            await sqlSugarEntityHandler.ExecuteDiffLogAsync(diff.DiffType, diff.BusinessData.ToString(),
                                tableName, tableDescription, diff.BeforeData?.Select(sl => sl.Columns).ToList(),
                                diff.AfterData?.Select(sl => sl.Columns).ToList(), diff.Sql, diff.Parameters, diff.Time,
                                handleSql);
                        }).Wait();
                    }
                }
            };
        }

        _db.Aop.OnError = exp =>
        {
            var param = (SugarParameter[]) exp.Parametres;

            var handleSql = SqlSugarContext.ParameterFormat(exp.Sql, param);

            // 代码CS文件名称
            var fileName = _db.Ado.SqlStackTrace.FirstFileName;
            // 代码行数
            var fileLine = _db.Ado.SqlStackTrace.FirstLine;
            // 方法名称
            var firstMethodName = _db.Ado.SqlStackTrace.FirstMethodName;
            // 消息
            var message =
                $"Sql 执行异常\r\nFileName：{fileName}\r\nFileLine：{fileLine}\r\nFirstMethodName：{firstMethodName}\r\nSql：{handleSql}";

#if DEBUG
            // 控制台输出
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\r\n\r\n{message}");
#endif

            if (sqlSugarEntityHandler != null)
            {
                // 执行Sql错误处理
                Task.Run(async () =>
                {
                    await sqlSugarEntityHandler.ExecuteErrorAsync(fileName, fileLine, firstMethodName, exp.Sql, param,
                        handleSql, message);
                }).Wait();
            }
        };

        // Model基类处理
        _db.Aop.DataExecuting = (oldValue, entityInfo) =>
        {
            switch (entityInfo.OperationType)
            {
                // 新增操作
                case DataFilterType.InsertByObject:
                    // 主键（long）赋值雪花Id，这里一条记录只会匹配一次
                    if (entityInfo.EntityColumnInfo.IsPrimarykey &&
                        entityInfo.EntityColumnInfo.PropertyInfo.PropertyType == typeof(long))
                    {
                        if (SqlSugarContext.EntityValueCheck(nameof(IPrimaryKeyEntity<long>.Id), new List<dynamic> {null, 0},
                                entityInfo))
                        {
                            entityInfo.SetValue(YitIdHelper.NextId());
                        }
                    }

                    // 创建时间
                    SqlSugarContext.SetEntityValue(nameof(IBaseEntity.CreatedTime), new List<dynamic> {null}, DateTime.Now,
                        ref entityInfo);

                    // 更新版本控制字段
                    SqlSugarContext.SetEntityValue(nameof(IBaseEntity.UpdatedVersion), new List<dynamic> {null, 0}, 1,
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
    /// <param name="_db"></param>
    /// <param name="sqlSugarEntityHandler"><see cref="ISqlSugarEntityHandler"/> Sugar实体处理</param>
    internal static void LoadSugarFilter(ISqlSugarClient _db, ISqlSugarEntityHandler sqlSugarEntityHandler)
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
        _db.QueryFilter.AddTableFilter<IBaseDeletedEntity>(it => it.IsDeleted == false);
    }
}