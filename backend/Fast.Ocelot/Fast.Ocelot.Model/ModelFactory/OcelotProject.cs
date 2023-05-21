using Fast.Admin.Model;
using Fast.Admin.Model.BaseModel;
using Fast.Admin.Model.Enum;
using SqlSugar;

namespace Fast.Ocelot.Model.ModelFactory;

/// <summary>
/// 网关项目表
/// </summary>
[SugarTable("Ocelot_Project", "网关项目表")]
[SugarDbType]
public class OcelotProject : BaseEntity
{
    /// <summary>
    /// 项目名称 
    ///</summary>
    [SugarColumn(ColumnDescription = "项目名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string ProjectName { get; set; }

    /// <summary>
    /// 顺序
    /// </summary>
    [SugarColumn(ColumnDescription = "顺序", IsNullable = false)]
    public int Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", IsNullable = false)]
    public CommonStatusEnum Status { get; set; } = CommonStatusEnum.Enable;
}