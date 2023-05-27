using Fast.Iaas.BaseModel.Interface;
using SqlSugar;

namespace Fast.Iaas.BaseModel;

/// <summary>
/// 租户基类
/// </summary>
public class BaseTenant : IBaseTenant
{
    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id", IsNullable = true, CreateTableFieldSort = 997)]
    public virtual long TenantId { get; set; }
}