using Fast.Admin.Model.BaseModel.Interface;
using Fast.Admin.Model.Enum;
using SqlSugar;

namespace Fast.Admin.Model.Model.Tenant.Auth.DataScope;

/// <summary>
/// 租户角色数据范围表Model类
/// </summary>
[SugarTable("Ten_Role_Data_Scope", "租户角色数据范围表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenRoleDataScopeModel : IDbEntity
{
    /// <summary>
    /// 角色Id
    /// </summary>
    [SugarColumn(ColumnDescription = "角色Id", IsNullable = false)]
    public long SysRoleId { get; set; }

    /// <summary>
    /// 机构Id
    /// </summary>
    [SugarColumn(ColumnDescription = "机构Id", IsNullable = false)]
    public long SysOrgId { get; set; }
}