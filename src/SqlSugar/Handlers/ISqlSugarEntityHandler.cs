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

using Microsoft.AspNetCore.Http;
using SqlSugar;

// ReSharper disable once CheckNamespace
namespace Fast.SqlSugar;

/// <summary>
/// <see cref="ISqlSugarEntityHandler"/> Sugar实体处理
/// </summary>
/// <remarks>
/// <para>不能在构造函数中注入 <see cref="ISqlSugarClient"/> 否则会出现循环引用的问题</para>
/// <para>不能在构造函数中注入 <see cref="IHttpContextAccessor"/> 或 <see cref="HttpContext"/> 否则会出现并发线程释放的问题</para>
/// </remarks>
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
    /// <param name="businessData"><see cref="object"/> 业务数据</param>
    /// <param name="beforeColumnList"><see cref="string"/> 执行前列信息</param>
    /// <param name="afterColumnList"><see cref="string"/> 执行后列信息</param>
    /// <param name="rawSql"><see cref="string"/> 原始Sql语句</param>
    /// <param name="parameters"><see cref="SugarParameter"/> Sql参数</param>
    /// <param name="executeTime"><see cref="TimeSpan"/> 执行时间</param>
    /// <param name="handlerSql"><see cref="string"/> 参数化处理后的Sql语句</param>
    /// <returns></returns>
    Task ExecuteDiffLogAsync(DiffType diffType, string tableName, string tableDescription, object businessData,
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
    /// <param name="exception"><see cref="SqlSugarException"/> 异常信息</param>
    /// <returns></returns>
    Task ExecuteErrorAsync(string fileName, int fileLine, string methodName, string rawSql, SugarParameter[] parameters,
        string handlerSql, SqlSugarException exception);

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