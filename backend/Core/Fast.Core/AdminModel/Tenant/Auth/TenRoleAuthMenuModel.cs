using Fast.Iaas.Attributes;
using Fast.Iaas.BaseModel.Interface;

namespace Fast.Core.AdminModel.Tenant.Auth;

/// <summary>
/// 租户角色授权菜单表Model类
/// </summary>
[SugarTable("Ten_Role_Auth_Menu", "租户角色授权菜单表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenRoleAuthMenuModel : IDbEntity
{
    /// <summary>
    /// 角色Id
    /// </summary>
    [SugarColumn(ColumnDescription = "角色Id", IsNullable = false)]
    public long SysRoleId { get; set; }

    /// <summary>
    /// 菜单Id
    /// </summary>
    [SugarColumn(ColumnDescription = "菜单Id", IsNullable = false)]
    public long SysMenuId { get; set; }
}