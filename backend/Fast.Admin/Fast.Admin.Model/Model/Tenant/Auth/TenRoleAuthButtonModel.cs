using Fast.Admin.Model.BaseModel.Interface;
using Fast.Admin.Model.Enum;
using SqlSugar;

namespace Fast.Admin.Model.Model.Tenant.Auth;

/// <summary>
/// 租户角色授权按钮表Model类
/// </summary>
[SugarTable("Ten_Role_Auth_Button", "租户角色授权按钮表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenRoleAuthButtonModel : IDbEntity
{
    /// <summary>
    /// 角色Id
    /// </summary>
    [SugarColumn(ColumnDescription = "角色Id", IsNullable = false)]
    public long SysRoleId { get; set; }

    /// <summary>
    /// 按钮Id
    /// </summary>
    [SugarColumn(ColumnDescription = "按钮Id", IsNullable = false)]
    public long SysButtonId { get; set; }
}