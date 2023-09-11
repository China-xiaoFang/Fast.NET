using Fast.Iaas.BaseModel.Interface;
using SqlSugar;

namespace Fast.Iaas.BaseModel{

/// <summary>
/// 租户实体基类
/// </summary>
public class BaseTEntity : BaseEntity, IBaseTenant
{
    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id", IsNullable = true, CreateTableFieldSort = 997)]
    public virtual long TenantId { get; set; }
}}