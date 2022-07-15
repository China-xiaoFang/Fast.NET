namespace Fast.NET.Core.AdminFactory.EnumFactory;

/// <summary>
/// 系统Db类型枚举
/// </summary>
public enum SysDataBaseTypeEnum
{
    /// <summary>
    /// Admin库
    /// </summary>
    [Description("Admin库")]
    Default = 1,

    /// <summary>
    /// 业务库
    /// </summary>
    [Description("业务库")]
    Business = 2
}