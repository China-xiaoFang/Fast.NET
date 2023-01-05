using Fast.SqlSugar.Tenant.AttributeFilter;
using Fast.SqlSugar.Tenant.BaseModel;
using Fast.SqlSugar.Tenant.Internal.Enum;
using SqlSugar;

namespace Fast.SqlSugar.Tenant.SugarEntity;

/// <summary>
/// 系统租户数据库Model类
/// </summary>
[SugarTable("Sys_Tenant_Database", "系统租户数据库表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class SysTenantDataBaseModel : BaseTEntity
{
    /// <summary>
    /// 服务器Ip地址
    /// </summary>
    [SugarColumn(ColumnDescription = "服务器Ip地址", ColumnDataType = "Nvarchar(15)", IsNullable = false)]
    public string ServiceIp { get; set; }

    /// <summary>
    /// 端口号
    /// </summary>
    [SugarColumn(ColumnDescription = "端口号", ColumnDataType = "Nvarchar(5)", IsNullable = false)]
    public string Port { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string DbName { get; set; }

    /// <summary>
    /// 数据库用户
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库用户", ColumnDataType = "Nvarchar(10)", IsNullable = false)]
    public string DbUser { get; set; }

    /// <summary>
    /// 数据库密码
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库密码", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string DbPwd { get; set; }

    /// <summary>
    /// 系统数据库类型
    /// </summary>
    [SugarColumn(ColumnDescription = "系统数据库类型", ColumnDataType = "tinyint", IsNullable = false)]
    public int SugarSysDbType { get; set; }

    /// <summary>
    /// 系统数据库类型名称
    /// </summary>
    [SugarColumn(ColumnDescription = "系统数据库类型名称", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string SugarDbTypeName { get; set; }

    /// <summary>
    /// 数据库类型，用于区分使用的是那个类型的数据库
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库类型", ColumnDataType = "tinyint", IsNullable = false)]
    public DbType DbType { get; set; }

    /// <summary>
    /// SqlSugarClient 连接Id
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public string ConnectionId { get; set; }
}