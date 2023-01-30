using Fast.SqlSugar.Tenant.AttributeFilter;
using Fast.SqlSugar.Tenant.BaseModel;
using Fast.SqlSugar.Tenant.Internal.Enum;

namespace Fast.Core.AdminFactory.ModelFactory.Basis;

/// <summary>
/// 租户员工表Model类
/// </summary>
[SugarTable("Ten_Emp", "租户员工表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenEmpModel : PrimaryKeyEntity
{
    /// <summary>
    /// 工号
    /// </summary>
    [SugarColumn(ColumnDescription = "工号", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string JobNum { get; set; }

    /// <summary>
    /// 机构Id
    /// </summary>
    [SugarColumn(ColumnDescription = "机构Id", IsNullable = false)]
    public long OrgId { get; set; }
}