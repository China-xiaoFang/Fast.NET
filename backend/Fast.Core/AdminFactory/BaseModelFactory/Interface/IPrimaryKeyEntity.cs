namespace Fast.Core.AdminFactory.BaseModelFactory.Interface;

/// <summary>
/// 主键接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPrimaryKeyEntity<T>
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [SugarColumn(ColumnDescription = "Id主键", IsPrimaryKey = true)]
    // 注意是在这里定义你的公共实体
    T Id { get; set; }
}