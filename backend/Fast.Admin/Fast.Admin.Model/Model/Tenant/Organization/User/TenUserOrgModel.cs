using Fast.Admin.Model.BaseModel.Interface;
using Fast.Admin.Model.Enum;
using SqlSugar;

namespace Fast.Admin.Model.Model.Tenant.Organization.User;

/// <summary>
/// 租户用户机构表Model类
/// </summary>
[SugarTable("Ten_User_Org", "租户用户机构表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenUserOrgModel : IDbEntity
{
    /// <summary>
    /// 用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "用户Id", IsNullable = false)]
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