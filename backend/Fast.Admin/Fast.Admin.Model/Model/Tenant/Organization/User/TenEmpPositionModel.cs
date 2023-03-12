using Fast.SqlSugar.Tenant.AttributeFilter;
using Fast.SqlSugar.Tenant.BaseModel.Interface;
using Fast.SqlSugar.Tenant.Internal.Enum;
using SqlSugar;

namespace Fast.Admin.Model.Model.Tenant.Organization.User;

/// <summary>
/// 租户员工职位表Model类
/// </summary>
[SugarTable("Ten_Emp_Position", "租户员工职位表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenEmpPositionModel : IDbEntity
{
    /// <summary>
    /// 系统用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "职位Id", IsNullable = false)]
    public long SysUserId { get; set; }

    /// <summary>
    /// 机构Id
    /// </summary>
    [SugarColumn(ColumnDescription = "机构Id", IsNullable = false)]
    public long OrgId { get; set; }

    /// <summary>
    /// 职位Id
    /// </summary>
    [SugarColumn(ColumnDescription = "职位Id", IsNullable = false)]
    public long PositionId { get; set; }

    /// <summary>
    /// 主管系统用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "职位Id", IsNullable = true)]
    public long? LeaderSysUserId { get; set; }
}