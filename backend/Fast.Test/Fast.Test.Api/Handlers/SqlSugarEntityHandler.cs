using Fast.SqlSugar.Attributes;
using Fast.SqlSugar.Handlers;
using Fast.SqlSugar.Options;
using Fast.Test.Api.Entities;
using SqlSugar;

namespace Fast.Test.Api.Handlers;

public class SqlSugarEntityHandler : ISqlSugarEntityHandler
{
    /// <summary>
    /// 根据实体类型获取连接字符串
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="sqlSugarClient"><see cref="ISqlSugarClient"/> 默认库SqlSugar客户端</param>
    /// <param name="sugarDbType">实体类头部的 <see cref="SugarDbTypeAttribute"/> 特性，如果不存在可能为空</param>
    /// <returns></returns>
    public async Task<ConnectionSettingsOptions> GetConnectionSettings<TEntity>(ISqlSugarClient sqlSugarClient, SugarDbTypeAttribute sugarDbType)
    {
        if (typeof(TEntity) == typeof(Entity1))
        {
            return new ConnectionSettingsOptions
            {
                ConnectionId = "123",
                ServiceIp = "192.168.1.22",
                Port = "1433",
                DbName = "Gejia.Gateway1",
                DbUser = "sa",
                DbPwd = "dev.2018",
                DbType = DbType.SqlServer,
                CommandTimeOut = 60,
                SugarSqlExecMaxSeconds = 30,
                DiffLog = true,
            };
        }

        return null;
    }

    /// <summary>
    /// 执行Sql
    /// </summary>
    /// <param name="sql"><see cref="string"/> 未处理的Sql语句</param>
    /// <param name="parameters"><see cref="SugarParameter"/> Sql参数</param>
    /// <param name="executionTime"><see cref="TimeSpan"/> 执行时间</param>
    /// <param name="handlerSql"><see cref="string"/> 参数化处理后的Sql语句</param>
    /// <returns></returns>
    public async Task ExecuteAsync(string sql, SugarParameter[] parameters, TimeSpan executionTime, string handlerSql)
    {
    }

    /// <summary>
    /// 执行Sql超时
    /// </summary>
    /// <param name="fileName"><see cref="string"/> 文件名称</param>
    /// <param name="fileLine"><see cref="int"/> 文件行数</param>
    /// <param name="methodName"><see cref="string"/> 方法名称</param>
    /// <param name="sql"><see cref="string"/> 未处理的Sql语句</param>
    /// <param name="parameters"><see cref="SugarParameter"/> Sql参数</param>
    /// <param name="executionTime"><see cref="TimeSpan"/> 执行时间</param>
    /// <param name="handlerSql"><see cref="string"/> 参数化处理后的Sql语句</param>
    /// <param name="message"><see cref="string"/></param>
    /// <returns></returns>
    public async Task ExecuteTimeoutAsync(string fileName, int fileLine, string methodName, string sql,
        SugarParameter[] parameters, TimeSpan executionTime, string handlerSql, string message)
    {
    }

    /// <summary>
    /// 执行Sql差异
    /// </summary>
    /// <param name="diffType"><see cref="DiffType"/> 差异类型</param>
    /// <param name="diffDescription"><see cref="string"/> 差异描述</param>
    /// <param name="tableName"><see cref="string"/> 表名称</param>
    /// <param name="tableDescription"><see cref="string"/> 表描述</param>
    /// <param name="beforeColumnList"><see cref="string"/> 执行前列信息</param>
    /// <param name="afterColumnList"><see cref="string"/> 执行后列信息</param>
    /// <param name="sql"><see cref="string"/> 未处理的Sql语句</param>
    /// <param name="parameters"><see cref="SugarParameter"/> Sql参数</param>
    /// <param name="executionTime"><see cref="TimeSpan"/> 执行时间</param>
    /// <param name="handlerSql"><see cref="string"/> 参数化处理后的Sql语句</param>
    /// <returns></returns>
    public async Task ExecuteDiffLogAsync(DiffType diffType, string diffDescription, string tableName, string tableDescription,
        List<List<DiffLogColumnInfo>> beforeColumnList, List<List<DiffLogColumnInfo>> afterColumnList, string sql,
        SugarParameter[] parameters, TimeSpan? executionTime, string handlerSql)
    {
    }

    /// <summary>
    /// 执行Sql错误
    /// </summary>
    /// <param name="fileName"><see cref="string"/> 文件名称</param>
    /// <param name="fileLine"><see cref="int"/> 文件行数</param>
    /// <param name="methodName"><see cref="string"/> 方法名称</param>
    /// <param name="sql"><see cref="string"/> 未处理的Sql语句</param>
    /// <param name="parameters"><see cref="SugarParameter"/> Sql参数</param>
    /// <param name="handlerSql"><see cref="string"/> 参数化处理后的Sql语句</param>
    /// <param name="message"><see cref="string"/></param>
    /// <returns></returns>
    public async Task ExecuteErrorAsync(string fileName, int fileLine, string methodName, string sql, SugarParameter[] parameters,
        string handlerSql, string message)
    {
    }

    /// <summary>
    /// 指派租户Id
    /// </summary>
    /// <returns></returns>
    public long? AssignTenantId()
    {
        return null;
    }

    /// <summary>
    /// 指派部门Id
    /// </summary>
    /// <returns></returns>
    public long? AssignDepartmentId()
    {
        return null;
    }

    /// <summary>
    /// 指派部门名称
    /// </summary>
    /// <returns></returns>
    public string AssignDepartmentName()
    {
        return null;
    }

    /// <summary>
    /// 指派用户Id
    /// </summary>
    /// <returns></returns>
    public long? AssignUserId()
    {
        return null;
    }

    /// <summary>
    /// 指派用户名称
    /// </summary>
    /// <returns></returns>
    public string AssignUserName()
    {
        return null;
    }

    /// <summary>
    /// 是否为超级管理员
    /// </summary>
    /// <returns></returns>
    public bool IsSuperAdmin()
    {
        return false;
    }

    /// <summary>
    /// 是否为管理员
    /// </summary>
    /// <returns></returns>
    public bool IsAdmin()
    {
        return false;
    }
}