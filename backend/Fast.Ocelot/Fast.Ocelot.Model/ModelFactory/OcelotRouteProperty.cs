using Fast.Admin.Model;
using Fast.Admin.Model.BaseModel;
using SqlSugar;

namespace Fast.Ocelot.Model.ModelFactory;

/// <summary>
/// 路由属性表
///</summary>
[SugarTable("Ocelot_Route_Property", "网关路由属性表")]
[SugarDbType]
public class OcelotRouteProperty : BaseEntity
{
    /// <summary>
    /// 
    ///</summary>
    [SugarColumn(ColumnDescription = "Key", ColumnDataType = "Nvarchar(200)", IsNullable = false)]
    public string Key { get; set; }

    /// <summary>
    /// 
    ///</summary>
    [SugarColumn(ColumnDescription = "Value", ColumnDataType = "Nvarchar(200)", IsNullable = false)]
    public string Value { get; set; }

    /// <summary>
    /// 路由Id 
    ///</summary>
    [SugarColumn(ColumnDescription = "路由Id", IsNullable = false)]
    public long RouteId { get; set; }

    /// <summary>
    /// 
    ///</summary>
    [SugarColumn(ColumnDescription = "Type", IsNullable = false)]
    public int Type { get; set; }
}