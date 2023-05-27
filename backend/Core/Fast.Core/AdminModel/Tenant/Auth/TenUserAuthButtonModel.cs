using Fast.Iaas.Attributes;
using Fast.Iaas.BaseModel.Interface;

namespace Fast.Core.AdminModel.Tenant.Auth;

/// <summary>
/// 租户用户授权按钮表Model类
/// </summary>
[SugarTable("Ten_User_Auth_Button", "租户用户授权按钮表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenUserAuthButtonModel : IDbEntity
{
    /// <summary>
    /// 用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "用户Id", IsNullable = false)]
    public long SysUserId { get; set; }

    /// <summary>
    /// 按钮Id
    /// </summary>
    [SugarColumn(ColumnDescription = "按钮Id", IsNullable = false)]
    public long SysButtonId { get; set; }
}