using Fast.Admin.Model.BaseModel;
using Fast.Admin.Model.Enum;
using SqlSugar;

namespace Fast.Admin.Model.Model.Tenant.Organization;

/// <summary>
/// 租户职位表Model类
/// </summary>
[SugarTable("Ten_Position", "租户职位表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenPositionModel : BaseEntity
{
    /// <summary>
    /// 职位名称
    /// </summary>
    [SugarColumn(ColumnDescription = "职位名称", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string PositionName { get; set; }

    /// <summary>
    /// 机构Id
    /// </summary>
    [SugarColumn(ColumnDescription = "机构Id", IsNullable = false)]
    public long OrgId { get; set; }

    /// <summary>
    /// 职位分类
    /// </summary>
    [SugarColumn(ColumnDescription = "职位分类", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string PositionType { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序", IsNullable = false)]
    public int Sort { get; set; }
}