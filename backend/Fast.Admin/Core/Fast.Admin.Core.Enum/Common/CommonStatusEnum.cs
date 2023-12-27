namespace Fast.Admin.Core.Enum.Common;

/// <summary>
/// 公共状态枚举
/// </summary>
[FastEnum("公共状态枚举")]
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