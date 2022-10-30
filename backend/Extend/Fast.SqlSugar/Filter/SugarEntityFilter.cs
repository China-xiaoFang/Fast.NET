using SystemDbType = System.Data.DbType;

namespace Fast.SqlSugar.Filter;

/// <summary>
/// Sugar实体类过滤器
/// </summary>
static class SugarEntityFilter
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
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }

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
                        if (EntityValueCheck(SugarFieldConst.Id, new List<dynamic> {null, 0}, entityInfo))
                            entityInfo.SetValue(YitIdHelper.NextId());
                    }

                    // 创建时间
                    SetEntityValue(SugarFieldConst.CreatedTime, new List<dynamic> {null}, DateTime.Now, ref entityInfo);

                    // 判断是否存在用户信息
                    if (User != null)
                    {
                        // 租户Id
                        SetEntityValue(SugarFieldConst.TenantId, new List<dynamic> {null, 0}, SugarGlobalContext.TenantId,
                            ref entityInfo);

                        // 创建者Id
                        SetEntityValue(SugarFieldConst.CreatedUserId, new List<dynamic> {null, 0}, SugarGlobalContext.UserId,
                            ref entityInfo);

                        // 创建者名称
                        SetEntityValue(SugarFieldConst.CreatedUserName, new List<dynamic> {null, ""}, SugarGlobalContext.UserName,
                            ref entityInfo);
                    }

                    break;
                // 更新操作
                case DataFilterType.UpdateByObject:
                    // 更新时间
                    SetEntityValue(SugarFieldConst.UpdatedTime, null, DateTime.Now, ref entityInfo);

                    // 更新者Id
                    SetEntityValue(SugarFieldConst.UpdatedUserId, null, SugarGlobalContext.UserId, ref entityInfo);

                    // 更新者名称
                    SetEntityValue(SugarFieldConst.UpdatedUserName, null, SugarGlobalContext.UserName, ref entityInfo);
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
                    typeof(bool), $"{nameof(BaseTenant.TenantId)} == @0 OR @1 ", SugarGlobalContext.GetTenantId(false),
                    SugarGlobalContext.IsSuperAdmin);
                // 将Lambda传入过滤器
                _db.QueryFilter.Add(new TableFilterItem<object>(entityType.Type, lambda));
            }

            // 配置加删除全局过滤器
            // 判断实体是否继承了基类
            // ReSharper disable once InvertIf
            if (!entityType.Type.GetProperty(SugarFieldConst.IsDeleted).IsEmpty())
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