namespace Fast.SqlSugar.Dto;

/// <summary>
/// Sugar Entity 类型
/// </summary>
public class SugarEntityTypeDto
{
    /// <summary>
    /// Entity 类型
    /// </summary>
    /// <param name="className">Class 名称</param>
    /// <param name="dbType">Db 类型</param>
    /// <param name="type">Entity 类型</param>
    public SugarEntityTypeDto(string className, SysDataBaseTypeEnum dbType, Type type)
    {
        ClassName = className;
        DbType = dbType;
        Type = type;
    }

    /// <summary>
    /// Class 名称
    /// </summary>
    public string ClassName { get; set; }

    /// <summary>
    /// Db 类型
    /// </summary>
    public SysDataBaseTypeEnum DbType { get; set; }

    /// <summary>
    /// Entity 类型
    /// </summary>
    public Type Type { get; set; }
}