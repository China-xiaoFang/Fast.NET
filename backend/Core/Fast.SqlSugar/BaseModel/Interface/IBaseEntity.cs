namespace Fast.SqlSugar.BaseModel.Interface;

/// <summary>
/// BaseEntity 接口定义类
/// </summary>
public interface IBaseEntity : IDbEntity
{
    /// <summary>
    /// 创建时间
    /// </summary>
    DateTime? CreatedTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    DateTime? UpdatedTime { get; set; }

    /// <summary>
    /// 创建者Id
    /// </summary>
    long? CreatedUserId { get; set; }

    /// <summary>
    /// 创建者名称
    /// </summary>
    string CreatedUserName { get; set; }

    /// <summary>
    /// 更新者Id
    /// </summary>
    long? UpdatedUserId { get; set; }

    /// <summary>
    /// 更新者名称
    /// </summary>
    string UpdatedUserName { get; set; }

    /// <summary>
    /// 更新版本控制字段
    /// </summary>
    long UpdatedVersion { get; set; }

    /// <summary>
    /// 软删除
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// 更新信息列
    /// </summary>
    /// <returns></returns>
    string[] UpdateColumn();

    /// <summary>
    /// 假删除的列，包含更新信息
    /// </summary>
    /// <returns></returns>
    string[] FalseDeleteColumn();
}