namespace Fast.SqlSugar.Enum;

/// <summary>
/// 系统Db类型枚举
/// </summary>
public enum SysDataBaseTypeEnum
{
    /// <summary>
    /// Admin库
    /// </summary>
    [Description("Admin库")]
    Admin = 1,

    /// <summary>
    /// 租户库
    /// </summary>
    [Description("租户库")]
    Tenant = 2
}