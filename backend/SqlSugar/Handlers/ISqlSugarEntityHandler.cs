// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
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

using Fast.SqlSugar.Attributes;
using Fast.SqlSugar.Options;
using SqlSugar;

namespace Fast.SqlSugar.Handlers;

/// <summary>
/// <see cref="ISqlSugarEntityHandler"/> Sugar实体处理
/// <remarks>不能在构造函数中注入 <see cref="ISqlSugarClient"/> 否则会出现循环引用的问题</remarks>
/// </summary>
public interface ISqlSugarEntityHandler
{
    /// <summary>
    /// 根据实体类型获取连接字符串
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="sqlSugarClient"><see cref="ISqlSugarClient"/> 默认库SqlSugar客户端</param>
    /// <param name="sugarDbType">实体类头部的 <see cref="SugarDbTypeAttribute"/> 特性，如果不存在可能为空</param>
    /// <param name="entityType"><see cref="Type"/> 实体类型</param>
    /// <returns></returns>
    Task<ConnectionSettingsOptions> GetConnectionSettings<TEntity>(ISqlSugarClient sqlSugarClient,
        SugarDbTypeAttribute sugarDbType, Type entityType);

    /// <summary>
    /// 执行Sql
    /// </summary>
    /// <param name="rawSql"><see cref="string"/> 原始Sql语句</param>
    /// <param name="parameters"><see cref="SugarParameter"/> Sql参数</param>
    /// <param name="executeTime"><see cref="TimeSpan"/> 执行时间</param>
    /// <param name="handlerSql"><see cref="string"/> 参数化处理后的Sql语句</param>
    /// <returns></returns>
    Task ExecuteAsync(string rawSql, SugarParameter[] parameters, TimeSpan executeTime, string handlerSql);

    /// <summary>
    /// 执行Sql超时
    /// </summary>
    /// <param name="fileName"><see cref="string"/> 文件名称</param>
    /// <param name="fileLine"><see cref="int"/> 文件行数</param>
    /// <param name="methodName"><see cref="string"/> 方法名称</param>
    /// <param name="rawSql"><see cref="string"/> 未处理的Sql语句</param>
    /// <param name="parameters"><see cref="SugarParameter"/> Sql参数</param>
    /// <param name="executeTime"><see cref="TimeSpan"/> 执行时间</param>
    /// <param name="handlerSql"><see cref="string"/> 参数化处理后的Sql语句</param>
    /// <param name="message"><see cref="string"/></param>
    /// <returns></returns>
    Task ExecuteTimeoutAsync(string fileName, int fileLine, string methodName, string rawSql, SugarParameter[] parameters,
        TimeSpan executeTime, string handlerSql, string message);

    /// <summary>
    /// 执行Sql差异
    /// </summary>
    /// <param name="diffType"><see cref="DiffType"/> 差异类型</param>
    /// <param name="tableName"><see cref="string"/> 表名称</param>
    /// <param name="tableDescription"><see cref="string"/> 表描述</param>
    /// <param name="diffDescription"><see cref="string"/> 差异描述</param>
    /// <param name="beforeColumnList"><see cref="string"/> 执行前列信息</param>
    /// <param name="afterColumnList"><see cref="string"/> 执行后列信息</param>
    /// <param name="rawSql"><see cref="string"/> 原始Sql语句</param>
    /// <param name="parameters"><see cref="SugarParameter"/> Sql参数</param>
    /// <param name="executeTime"><see cref="TimeSpan"/> 执行时间</param>
    /// <param name="handlerSql"><see cref="string"/> 参数化处理后的Sql语句</param>
    /// <returns></returns>
    Task ExecuteDiffLogAsync(DiffType diffType, string tableName, string tableDescription, string diffDescription,
        List<List<DiffLogColumnInfo>> beforeColumnList, List<List<DiffLogColumnInfo>> afterColumnList, string rawSql,
        SugarParameter[] parameters, TimeSpan? executeTime, string handlerSql);

    /// <summary>
    /// 执行Sql错误
    /// </summary>
    /// <param name="fileName"><see cref="string"/> 文件名称</param>
    /// <param name="fileLine"><see cref="int"/> 文件行数</param>
    /// <param name="methodName"><see cref="string"/> 方法名称</param>
    /// <param name="rawSql"><see cref="string"/> 原始Sql语句</param>
    /// <param name="parameters"><see cref="SugarParameter"/> Sql参数</param>
    /// <param name="handlerSql"><see cref="string"/> 参数化处理后的Sql语句</param>
    /// <param name="message"><see cref="string"/></param>
    /// <returns></returns>
    Task ExecuteErrorAsync(string fileName, int fileLine, string methodName, string rawSql, SugarParameter[] parameters,
        string handlerSql, string message);

    /// <summary>
    /// 指派租户Id
    /// </summary>
    /// <returns></returns>
    long? AssignTenantId();

    /// <summary>
    /// 指派部门Id
    /// </summary>
    /// <returns></returns>
    long? AssignDepartmentId();

    /// <summary>
    /// 指派部门名称
    /// </summary>
    /// <returns></returns>
    string AssignDepartmentName();

    /// <summary>
    /// 指派用户Id
    /// </summary>
    /// <returns></returns>
    long? AssignUserId();

    /// <summary>
    /// 指派用户名称
    /// </summary>
    /// <returns></returns>
    string AssignUserName();

    /// <summary>
    /// 是否为超级管理员
    /// </summary>
    /// <returns></returns>
    bool IsSuperAdmin();

    /// <summary>
    /// 是否为管理员
    /// </summary>
    /// <returns></returns>
    bool IsAdmin();
}