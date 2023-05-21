using Fast.SqlSugar.Attributes;
using Fast.SqlSugar.BaseModel;
using Fast.SqlSugar.Enum;

namespace Fast.Admin.Model.Model.Tenant.Organization;

/// <summary>
/// 租户组织架构表Model类
/// </summary>
[SugarTable("Ten_Org", "租户组织架构表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenOrgModel : BaseEntity
{
    /// <summary>
    /// 父级Id
    /// </summary>
    [SugarColumn(ColumnDescription = "父级Id", IsNullable = false)]
    public long ParentId { get; set; }

    /// <summary>
    /// 父级Id集合
    /// </summary>
    [SugarColumn(ColumnDescription = "父级Id集合", IsNullable = false, IsJson = true)]
    public List<long> ParentIds { get; set; }

    /// <summary>
    /// 组织名称
    /// </summary>
    [SugarColumn(ColumnDescription = "组织名称", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string OrgName { get; set; }

    /// <summary>
    /// 组织编码
    /// </summary>
    [SugarColumn(ColumnDescription = "组织编码", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string OrgCode { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    [SugarColumn(ColumnDescription = "联系人", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string Contacts { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    [SugarColumn(ColumnDescription = "电话", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string Tel { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序", IsNullable = false)]
    public int Sort { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string Remark { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", IsNullable = false)]
    public CommonStatusEnum Status { get; set; } = CommonStatusEnum.Enable;
}