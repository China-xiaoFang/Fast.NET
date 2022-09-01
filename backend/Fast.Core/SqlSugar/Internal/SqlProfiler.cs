using DbType = System.Data.DbType;

namespace Fast.Core.SqlSugar.Internal;

/// <summary>
/// SqlSugar 打印SQL语句参数格式化帮助类
/// 【使用方式】：在需要打印SQL语句的地方，如 Startup，将
/// App.PrintToMiniProfiler("SqlSugar1", "Info", sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
/// 替换为
/// App.PrintToMiniProfiler("SqlSugar", "Info", SqlProfiler.ParameterFormat(sql, pars));
/// </summary>
public class SqlProfiler
{
    /// <summary>
    /// 格式化参数拼接成完整的SQL语句
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="pars"></param>
    /// <returns></returns>
    public static string ParameterFormat(string sql, SugarParameter[] pars)
    {
        //应逆向替换，否则由于 SqlSugar 多个表的过滤器问题导致替换不完整  如 @TenantId1  @TenantId10
        for (var i = pars.Length - 1; i >= 0; i--)
        {
            sql = pars[i].DbType switch
            {
                DbType.String or DbType.DateTime or DbType.Date or DbType.Time or DbType.DateTime2 or DbType.DateTimeOffset
                    or DbType.Guid or DbType.VarNumeric or DbType.AnsiStringFixedLength or DbType.AnsiString
                    or DbType.StringFixedLength => sql.Replace(pars[i].ParameterName, "'" + pars[i].Value + "'"),
                DbType.Boolean when pars[i].Value.IsEmpty() => sql.Replace(pars[i].ParameterName, "NULL"),
                DbType.Boolean => sql.Replace(pars[i].ParameterName, Convert.ToBoolean(pars[i].Value) ? "1" : "0"),
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
    public static string ParameterFormat(string sql, object pars)
    {
        var param = (SugarParameter[]) pars;
        return ParameterFormat(sql, param);
    }
}