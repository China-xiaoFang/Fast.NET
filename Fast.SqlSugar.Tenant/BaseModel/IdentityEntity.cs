using Fast.SqlSugar.Tenant.BaseModel.Interface;
using SqlSugar;

namespace Fast.SqlSugar.Tenant.BaseModel;

/// <summary>
/// 递增主键实体基类
/// </summary>
public abstract class IdentityEntity : IPrimaryKeyEntity<int>, IDbEntity
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [SugarColumn(IsIdentity = true, ColumnDescription = "Id主键", IsPrimaryKey = true)] //通过特性设置主键和自增列 
    // 注意是在这里定义你的公共实体
    public virtual int Id { get; set; }
}