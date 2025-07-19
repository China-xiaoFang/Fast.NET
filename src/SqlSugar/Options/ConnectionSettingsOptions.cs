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

using SqlSugar;

// ReSharper disable once CheckNamespace
namespace Fast.SqlSugar;

/// <summary>
/// <see cref="ConnectionSettingsOptions"/> 连接字符串配置
/// </summary>
[SuppressSniffer]
public class ConnectionSettingsOptions : DbConnectionInfo
{
    /// <summary>
    /// SqlSugarClient 连接Id
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public string ConnectionId { get; set; }

    /// <summary>
    /// 数据库类型，用于区分使用的是那个类型的数据库
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库类型")]
    public DbType? DbType { get; set; }

    /// <summary>
    /// 超时时间，单位秒
    /// </summary>
    [SugarColumn(ColumnDescription = "超时时间，单位秒")]
    public int? CommandTimeOut { get; set; }

    /// <summary>
    /// SqlSugar Sql执行最大秒数，如果超过记录警告日志
    /// </summary>
    [SugarColumn(ColumnDescription = "SqlSugar Sql执行最大秒数，如果超过记录警告日志")]
    public int? SugarSqlExecMaxSeconds { get; set; }

    /// <summary>
    /// 差异日志
    /// </summary>
    [SugarColumn(ColumnDescription = "差异日志")]
    public bool? DiffLog { get; set; }

    /// <summary>
    /// 禁用 SqlSugar 的 Aop
    /// </summary>
    /// <remarks>
    /// 如果是通过 <see cref="ISqlSugarEntityHandler"/> 进行保存日志到数据库中
    /// <para>必须要将相关 AOP 中涉及到的日志表，单独进行分库设置，并且禁用 AOP</para>
    /// <para>或通过 new <see cref="SqlSugarClient"/>() 的方式进行保存。不然会导致死循环的问题</para>
    /// </remarks>
    [SugarColumn(ColumnDescription = "差异日志")]
    public bool? DisableAop { get; set; }

    /// <summary>
    /// 从库信息
    /// </summary>
    /// <remarks>一般默认库或者主库不建议设置从库</remarks>
    [SugarColumn(IsIgnore = true)]
    public virtual List<SlaveConnectionInfo> SlaveConnectionList { get; set; }

    /// <summary>
    /// 后期配置
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public override void PostConfigure()
    {
        base.PostConfigure();
        DbType ??= global::SqlSugar.DbType.SqlServer;
        CommandTimeOut ??= 60;
        SugarSqlExecMaxSeconds ??= 30;
        DiffLog ??= false;
        DisableAop ??= true;
    }
}