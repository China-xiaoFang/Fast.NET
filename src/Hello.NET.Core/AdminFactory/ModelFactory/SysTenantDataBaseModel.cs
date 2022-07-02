namespace Hello.NET.Core.AdminFactory.ModelFactory;

/// <summary>
/// 系统租户数据库Model类
/// </summary>
[SugarTable("sys_tenant_database")]
[Description("系统租户数据库表")]
public class SysTenantDataBaseModel : IDefaultDb
{
    /// <summary>
    /// 服务器Ip地址，可带端口
    /// </summary>
    [SugarColumn(ColumnDescription = "服务器Ip地址，可带端口", ColumnDataType = "Nvarchar(24)", IsNullable = false)]
    public string ServiceIp { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库名称", ColumnDataType = "Nvarchar(Max)", IsNullable = false)]
    public string DbName { get; set; }

    /// <summary>
    /// 数据库用户
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库用户", ColumnDataType = "Nvarchar(24)", IsNullable = false)]
    public string DbUser { get; set; }

    /// <summary>
    /// 数据库密码
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库密码", ColumnDataType = "Nvarchar(Max)", IsNullable = false)]
    public string DbPwd { get; set; }

    /// <summary>
    /// 数据库类型，用于区分是admin库还是业务库
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库类型", ColumnDataType = "tinyint", IsNullable = false)]
    public SysDataBaseTypeEnum DataBaseType { get; set; }

    /// <summary>
    /// 数据库类型，用于区分使用的是那个数据库
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库类型", ColumnDataType = "tinyint", IsNullable = false)]
    public DbType DbType { get; set; }

    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id", IsNullable = false)]
    public virtual long TenantId { get; set; }
}