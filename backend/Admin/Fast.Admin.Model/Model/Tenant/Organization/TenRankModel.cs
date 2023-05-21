using Fast.Admin.Model.BaseModel;
using Fast.Admin.Model.Enum;
using SqlSugar;

namespace Fast.Admin.Model.Model.Tenant.Organization;

/// <summary>
/// 租户职级表Model类
/// </summary>
[SugarTable("Ten_Rank", "租户职级表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenRankModel : BaseEntity
{
    /// <summary>
    /// 职级名称
    /// </summary>
    [SugarColumn(ColumnDescription = "职级名称", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string RankName { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序", IsNullable = false)]
    public int Sort { get; set; }
}