namespace Fast.Core.AdminService.SysButton.Dto;

/// <summary>
/// 添加系统按钮输入
/// </summary>
public class AddSysButtonInput
{
    /// <summary>
    /// 按钮编码
    /// </summary>
    [Required(ErrorMessage = "按钮编码不能为空")]
    public string ButtonCode { get; set; }

    /// <summary>
    /// 按钮名称
    /// </summary>
    [Required(ErrorMessage = "按钮名称不能为空")]
    public string ButtonName { get; set; }

    /// <summary>
    /// 菜单Id
    /// </summary>
    [Required(ErrorMessage = "菜单Id不能为空")]
    public long MenuId { get; set; }

    /// <summary>
    /// 接口Id
    /// </summary>
    [Required(ErrorMessage = "接口Id不能为空")]
    public long ApiId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Required(ErrorMessage = "按钮排序不能为空")]
    public int Sort { get; set; }
}

/// <summary>
/// 更新系统按钮输入
/// </summary>
public class UpdateSysButtonInput : AddSysButtonInput
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [Required(ErrorMessage = "主键Id不能为空")]
    public long Id { get; set; }
}