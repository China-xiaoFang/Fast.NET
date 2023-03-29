using Fast.Admin.Model;
using Fast.Admin.Model.BaseModel;
using SqlSugar;

namespace Fast.Ocelot.Model.ModelFactory;

/// <summary>
/// 配置备份
///</summary>
[SugarTable("Ocelot_Setting_Bak", "网关配置备份表")]
[SugarDbType]
public class OcelotSettingBak : BaseEntity
{
    /// <summary>
    /// 备份时间
    ///</summary>
    [SugarColumn(ColumnDescription = "备份时间", ColumnDataType = "datetimeoffset", IsNullable = false)]
    public DateTime BakTime { get; set; }

    /// <summary>
    /// 备份内容
    ///</summary>
    [SugarColumn(ColumnDescription = "备份内容", ColumnDataType = "Nvarchar(200)", IsNullable = false)]
    public string BakJson { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", ColumnDataType = "Nvarchar(200)", IsNullable = false)]
    public string Remark { get; set; }

    /// <summary>
    /// ConsulKey
    /// </summary>
    [SugarColumn(ColumnDescription = "ConsulKey", ColumnDataType = "Nvarchar(200)", IsNullable = false)]
    public string ConsulKey { get; set; }

    /// <summary>
    /// ConsulDc
    /// </summary>
    [SugarColumn(ColumnDescription = "ConsulDc", ColumnDataType = "Nvarchar(200)", IsNullable = false)]
    public string ConsulDc { get; set; }
}