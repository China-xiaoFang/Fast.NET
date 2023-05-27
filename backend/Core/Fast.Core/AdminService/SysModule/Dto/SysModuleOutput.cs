using Fast.Core.AdminEnum;
using Fast.Iaas.BaseModel.Dto;

namespace Fast.Core.AdminService.SysModule.Dto;

/// <summary>
/// 系统模块输出
/// </summary>
public class SysModuleOutput : BaseOutput
{
    /// <summary>
    /// 名称
    /// </summary>
    public string ModuleName { get; set; }

    /// <summary>
    /// 颜色
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 查看类型
    /// </summary>
    public ModuleViewTypeEnum ViewType { get; set; }

    /// <summary>
    /// 是否为系统模块
    /// </summary>
    public YesOrNotEnum IsSystem { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public CommonStatusEnum Status { get; set; }
}