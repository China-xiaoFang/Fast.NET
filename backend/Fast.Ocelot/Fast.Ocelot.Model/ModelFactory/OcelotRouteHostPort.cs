using Fast.Core.SqlSugar.AttributeFilter;
using Fast.Core.SqlSugar.BaseModel;
using SqlSugar;

namespace Fast.Ocelot.Model.ModelFactory;

/// <summary>
/// 路由地址配置表
///</summary>
[SugarTable("Ocelot_Route_Host_Port", "网关路由地址配置表")]
[DataBaseType]
public class OcelotRouteHostPort : BaseEntity
{
    /// <summary>
    /// 主机
    ///</summary>
    [SugarColumn(ColumnDescription = "主机", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string Host { get; set; }

    /// <summary>
    /// 端口
    ///</summary>
    [SugarColumn(ColumnDescription = "端口", IsNullable = false)]
    public int Port { get; set; }

    /// <summary>
    /// 路由Id 
    ///</summary>
    [SugarColumn(ColumnDescription = "路由Id", IsNullable = false)]
    public long RouteId { get; set; }
}