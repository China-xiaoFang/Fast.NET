namespace Fast.Core.AdminFactory.ModelFactory.Basis;

/// <summary>
/// 系统用户角色表Model类
/// </summary>
[SugarTable("Sys_User_Role", "系统用户角色表")]
[DataBaseType(SysDataBaseTypeEnum.Tenant)]
public class SysUserRoleModel : IDbEntity
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