namespace Fast.Iaas.BaseModel.Interface;

/// <summary>
/// IsDeleted 软删除 接口定义类
/// </summary>
public interface IBaseDeleted : IDbEntity
{
    /// <summary>
    /// 软删除
    /// </summary>
    bool IsDeleted { get; set; }
}