namespace Fast.Core.SqlSugar.Internal.Dto;

/// <summary>
/// Sugar实体类型信息
/// </summary>
public class SugarEntityTypeInfo
{
    /// <summary>
    /// Sugar实体类型信息
    /// </summary>
    /// <param name="className">类名称</param>
    /// <param name="dbType">数据库类型</param>
    /// <param name="dbTypeName">数据库类型名称</param>
    /// <param name="isSplitTable">是否分表</param>
    /// <param name="type">实体类型</param>
    public SugarEntityTypeInfo(string className, int dbType, string dbTypeName, bool isSplitTable, Type type)
    {
        ClassName = className;
        DbType = dbType;
        DbTypeName = dbTypeName;
        IsSplitTable = isSplitTable;
        Type = type;
    }

    /// <summary>
    /// 类名称
    /// </summary>
    public string ClassName { get; set; }

    /// <summary>
    /// 数据库类型
    /// </summary>
    public int DbType { get; set; }

    /// <summary>
    /// 数据库类型名称
    /// </summary>
    public string DbTypeName { get; set; }

    /// <summary>
    /// 是否分表
    /// </summary>
    public bool IsSplitTable { get; set; }

    /// <summary>
    /// 实体类型
    /// </summary>
    public Type Type { get; set; }
}