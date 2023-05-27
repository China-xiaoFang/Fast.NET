namespace Fast.Core.AdminService.Auth.Dto;

/// <summary>
/// 输入错误密码次数Dto
/// </summary>
internal class InputErrorPasswordDto
{
    /// <summary>
    /// 次数
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 冻结分钟
    /// </summary>
    public int? FreezeMinutes { get; set; }

    /// <summary>
    /// 解冻时间
    /// </summary>
    public DateTime? ThawingTime { get; set; }
}