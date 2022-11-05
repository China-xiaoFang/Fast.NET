namespace Fast.Core.SqlSugar.BaseModel.Dto;

/// <summary>
/// 通用更新输入
/// </summary>
public class UpdateInputBase
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedTime { get; set; }
}