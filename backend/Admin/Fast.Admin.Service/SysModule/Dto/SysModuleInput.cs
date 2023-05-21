using Fast.Admin.Model.Enum;
using Fast.SqlSugar.BaseModel.Dto;

namespace Fast.Admin.Service.SysModule.Dto;

/// <summary>
/// 查询模块输入
/// </summary>
public class QuerySysModuleInput : PageInputBase
{
    /// <summary>
    /// 名称
    /// </summary>
    public string ModuleName { get; set; }

    /// <summary>
    /// 查看类型
    /// </summary>
    public ModuleViewTypeEnum? ViewType { get; set; }

    /// <summary>
    /// 是否为系统模块
    /// </summary>
    public YesOrNotEnum? IsSystem { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public CommonStatusEnum? Status { get; set; }
}

/// <summary>
/// 添加模块信息
/// </summary>
public class AddModuleInput
{
    /// <summary>
    /// 名称
    /// </summary>
    [Required(ErrorMessage = "名称不能为空")]
    public string ModuleName { get; set; }

    /// <summary>
    /// 颜色
    /// </summary>
    [Required(ErrorMessage = "颜色不能为空")]
    public string Color { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [Required(ErrorMessage = "图标不能为空")]
    public string Icon { get; set; }

    /// <summary>
    /// 查看类型
    /// </summary>
    [Required(ErrorMessage = "查看类型不能为空")]
    public ModuleViewTypeEnum ViewType { get; set; }

    /// <summary>
    /// 是否为系统模块
    /// </summary>
    [Required(ErrorMessage = "是否为系统模块不能为空")]
    public YesOrNotEnum IsSystem { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Required(ErrorMessage = "排序不能为空")]
    public int Sort { get; set; }
}

/// <summary>
/// 更新模块信息
/// </summary>
public class UpdateModuleInput : AddModuleInput
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [Required(ErrorMessage = "主键Id不能为空")]
    public long Id { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [Required(ErrorMessage = "状态不能为空")]
    public CommonStatusEnum Status { get; set; }
}