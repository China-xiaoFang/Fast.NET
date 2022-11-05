using Fast.Core.SqlSugar.BaseModel.Interface;

namespace Fast.Core.SqlSugar.BaseModel;

/// <summary>
/// 租户基类
/// </summary>
public class BaseTenant : IBaseTenant
{
    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id", CreateTableFieldSort = 998)]
    public virtual long? TenantId { get; set; }
}