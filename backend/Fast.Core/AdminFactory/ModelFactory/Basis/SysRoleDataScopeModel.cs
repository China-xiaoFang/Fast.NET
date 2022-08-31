namespace Fast.Core.AdminFactory.ModelFactory.Basis;

/// <summary>
/// 系统角色数据范围表Model类
/// </summary>
[SugarTable("Sys_Role_Data_Scope", "系统角色数据范围表")]
[DataBaseType(SysDataBaseTypeEnum.Tenant)]
public class SysRoleDataScopeModel : IDbEntity
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