using Fast.Iaas.Attributes;
using Fast.Iaas.BaseModel;

namespace Fast.Admin.Model.Model.Tenant.Organization;

/// <summary>
/// 租户角色表Model类
/// </summary>
[SugarTable("Ten_Role", "租户角色表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenRoleModel : BaseEntity
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [SugarColumn(ColumnDescription = "角色名称", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string RoleName { get; set; }

    /// <summary>
    /// 角色编码
    /// </summary>
    [SugarColumn(ColumnDescription = "角色编码", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string RoleCode { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序", IsNullable = false)]
    public int Sort { get; set; }

    /// <summary>
    /// 数据范围类型
    /// </summary>
    [SugarColumn(ColumnDescription = "数据范围类型", ColumnDataType = "tinyint", IsNullable = false)]
    public DataScopeTypeEnum DataScopeType { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string Remark { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", ColumnDataType = "tinyint", IsNullable = false)]
    public CommonStatusEnum Status { get; set; } = CommonStatusEnum.Enable;

    /// <summary>
    /// 角色类型
    /// </summary>
    [SugarColumn(ColumnDescription = "角色类型", ColumnDataType = "tinyint", IsNullable = false)]
    public RoleTypeEnum RoleType { get; set; }
}