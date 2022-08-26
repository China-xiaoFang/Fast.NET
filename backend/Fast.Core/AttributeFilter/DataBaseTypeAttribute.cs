namespace Fast.Core.AttributeFilter;

/// <summary>
/// 系统数据库类型特性，区分是那个数据库，默认是Admin库
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class DataBaseTypeAttribute : Attribute
{
    public DataBaseTypeAttribute(SysDataBaseTypeEnum sysDataBaseType = SysDataBaseTypeEnum.Admin)
    {
        SysDataBaseType = sysDataBaseType;
    }

    /// <summary>
    /// 系统数据库类型
    /// </summary>
    public SysDataBaseTypeEnum SysDataBaseType { get; set; }
}