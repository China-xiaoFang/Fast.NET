namespace Fast.Core.AdminFactory.ModelFactory.Basis;

/// <summary>
/// 系统用户数据范围表Model类
/// </summary>
[SugarTable("Sys_User_Data_Scope", "系统用户数据范围表")]
[DataBaseType(SysDataBaseTypeEnum.Tenant)]
public class SysUserDataScopeModel : IDbEntity
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
    public long SysOrgId { get; set; }
}