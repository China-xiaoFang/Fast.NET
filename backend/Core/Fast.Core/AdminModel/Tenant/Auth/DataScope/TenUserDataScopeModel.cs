using Fast.Iaas.Attributes;
using Fast.Iaas.BaseModel.Interface;

namespace Fast.Core.AdminModel.Tenant.Auth.DataScope;

/// <summary>
/// 租户用户数据范围表Model类
/// </summary>
[SugarTable("Ten_User_Data_Scope", "租户用户数据范围表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenUserDataScopeModel : IDbEntity
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