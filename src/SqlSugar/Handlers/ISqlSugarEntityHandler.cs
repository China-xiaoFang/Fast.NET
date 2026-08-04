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

namespace Fast.SqlSugar;

/// <summary>
/// 定义 SqlSugar 实体审计、租户和 SQL 事件处理契约。
/// </summary>
/// <remarks>
/// <para>实现类型的构造函数不得注入 <see cref="ISqlSugarClient"/>，否则会形成循环依赖。</para>
/// <para>实现类型的构造函数不得注入 <see cref="IHttpContextAccessor"/> 或 <see cref="HttpContext"/>，
/// 否则处理器生命周期可能跨越请求边界并访问已释放的请求资源。</para>
/// </remarks>
public interface ISqlSugarEntityHandler
{
    /// <summary>
    /// 异步获取指定实体使用的数据库连接配置。
    /// </summary>
    /// <param name="sqlSugarClient">SqlSugar 客户端实例。</param>
    /// <param name="sugarDbType">实体声明的 <see cref="SugarDbTypeAttribute"/>；未声明时为 <see langword="null"/>。</param>
    /// <param name="entityType">当前实体的 <see cref="Type"/>。</param>
    /// <typeparam name="TEntity">实体类型。</typeparam>
    /// <returns>实体使用的数据库连接配置；返回 <see langword="null"/> 时使用全局默认配置。</returns>
    Task<ConnectionSettingsOptions> GetConnectionSettings<TEntity>(ISqlSugarClient sqlSugarClient,
        SugarDbTypeAttribute sugarDbType, Type entityType);

    /// <summary>
    /// 异步处理已执行的 SQL 语句。
    /// </summary>
    /// <param name="rawSql">原始 SQL 文本。</param>
    /// <param name="parameters">方法或查询使用的参数集合。</param>
    /// <param name="executeTime">操作耗时。</param>
    /// <param name="handlerSql">要检查或改写的 SQL 文本。</param>
    /// <returns>表示 SQL 处理操作的任务。</returns>
    Task ExecuteAsync(string rawSql, SugarParameter[] parameters, TimeSpan executeTime, string handlerSql);

    /// <summary>
    /// 异步处理执行超时的 SQL 语句。
    /// </summary>
    /// <param name="fileName">源文件名称。</param>
    /// <param name="fileLine">源代码行号。</param>
    /// <param name="methodName">目标方法名称。</param>
    /// <param name="rawSql">原始 SQL 文本。</param>
    /// <param name="parameters">方法或查询使用的参数集合。</param>
    /// <param name="executeTime">操作耗时。</param>
    /// <param name="handlerSql">要检查或改写的 SQL 文本。</param>
    /// <param name="message">要记录或返回的消息。</param>
    /// <returns>表示 SQL 超时处理操作的任务。</returns>
    Task ExecuteTimeoutAsync(string fileName, int fileLine, string methodName, string rawSql, SugarParameter[] parameters,
        TimeSpan executeTime, string handlerSql, string message);

    /// <summary>
    /// 异步记录 SQL 数据变更差异。
    /// </summary>
    /// <param name="diffType">数据变更类型。</param>
    /// <param name="tableName">数据库表名称。</param>
    /// <param name="tableDescription">数据库表说明。</param>
    /// <param name="businessData">响应中承载的业务数据。</param>
    /// <param name="beforeColumnList">变更前的列值集合。</param>
    /// <param name="afterColumnList">变更后的列值集合。</param>
    /// <param name="rawSql">原始 SQL 文本。</param>
    /// <param name="parameters">方法或查询使用的参数集合。</param>
    /// <param name="executeTime">操作耗时。</param>
    /// <param name="handlerSql">要检查或改写的 SQL 文本。</param>
    /// <returns>表示数据变更差异记录操作的任务。</returns>
    Task ExecuteDiffLogAsync(DiffType diffType, string tableName, string tableDescription, object businessData,
        List<List<DiffLogColumnInfo>> beforeColumnList, List<List<DiffLogColumnInfo>> afterColumnList, string rawSql,
        SugarParameter[] parameters, TimeSpan? executeTime, string handlerSql);

    /// <summary>
    /// 异步处理 SQL 执行异常。
    /// </summary>
    /// <param name="fileName">源文件名称。</param>
    /// <param name="fileLine">源代码行号。</param>
    /// <param name="methodName">目标方法名称。</param>
    /// <param name="rawSql">原始 SQL 文本。</param>
    /// <param name="parameters">方法或查询使用的参数集合。</param>
    /// <param name="handlerSql">要检查或改写的 SQL 文本。</param>
    /// <param name="exception">SQL 执行期间引发的 <see cref="SqlSugarException"/>。</param>
    /// <returns>表示 SQL 异常处理操作的任务。</returns>
    Task ExecuteErrorAsync(string fileName, int fileLine, string methodName, string rawSql, SugarParameter[] parameters,
        string handlerSql, SqlSugarException exception);

    /// <summary>
    /// 是否为超级管理员。
    /// </summary>
    /// <returns>满足条件时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
    bool IsSuperAdmin();

    /// <summary>
    /// 是否为管理员。
    /// </summary>
    /// <returns>满足条件时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
    bool IsAdmin();

    /// <summary>
    /// 获取新实体使用的租户 ID。
    /// </summary>
    /// <returns>租户 ID；无法确定时返回 <see langword="null"/>。</returns>
    long? AssignTenantId();

    /// <summary>
    /// 获取新实体使用的部门 ID。
    /// </summary>
    /// <returns>部门 ID；无法确定时返回 <see langword="null"/>。</returns>
    long? AssignDepartmentId();

    /// <summary>
    /// 获取新实体使用的部门名称。
    /// </summary>
    /// <returns>部门名称；无法确定时返回 <see langword="null"/>。</returns>
    string AssignDepartmentName();

    /// <summary>
    /// 获取新实体使用的用户 ID。
    /// </summary>
    /// <returns>用户 ID；无法确定时返回 <see langword="null"/>。</returns>
    long? AssignUserId();

    /// <summary>
    /// 获取新实体使用的用户名称。
    /// </summary>
    /// <returns>用户名称；无法确定时返回 <see langword="null"/>。</returns>
    string AssignUserName();
}
