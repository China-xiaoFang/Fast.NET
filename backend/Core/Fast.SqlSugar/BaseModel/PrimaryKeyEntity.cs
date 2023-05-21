namespace Fast.Admin.Model.BaseModel;

/// <summary>
/// 主键实体基类
/// </summary>
public abstract class PrimaryKeyEntity : IPrimaryKeyEntity<long>, IDbEntity
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [SugarColumn(ColumnDescription = "Id主键", IsPrimaryKey = true)]
    // 注意是在这里定义你的公共实体
    public virtual long Id { get; set; }
}