using Fast.Iaas.Attributes;
using Fast.Iaas.BaseModel.Interface;
using SqlSugar;

namespace Fast.Admin.Model.Model.Tenant.Organization.User;

/// <summary>
/// 租户用户角色表Model类
/// </summary>
[SugarTable("Ten_User_Role", "租户用户角色表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenUserRoleModel : IDbEntity
{
    /// <summary>
    /// 用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "用户Id", IsNullable = false)]
    public long SysUserId { get; set; }

    /// <summary>
    /// 角色Id
    /// </summary>
    [SugarColumn(ColumnDescription = "角色Id", IsNullable = false)]
    public long SysRoleId { get; set; }
}