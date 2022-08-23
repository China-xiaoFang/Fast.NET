namespace Fast.NET.Core.AdminFactory.EnumFactory;

/// <summary>
/// 公共状态
/// </summary>
public enum CommonStatusEnum
{
    /// <summary>
    /// 正常
    /// </summary>
    [Description("正常")]
    Enable = 0,

    /// <summary>
    /// 停用
    /// </summary>
    [Description("停用")]
    Disable = 1,

    /// <summary>
    /// 删除
    /// </summary>
    [Description("删除")]
    Delete = 2
}