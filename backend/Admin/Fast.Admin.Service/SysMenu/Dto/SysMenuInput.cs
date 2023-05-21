using Fast.Admin.Model.Enum;

namespace Fast.Admin.Service.SysMenu.Dto;

/// <summary>
/// 查询系统菜单树形输入
/// </summary>
public class QuerySysMenuTreeInput
{
    /// <summary>
    /// 模块Id
    /// </summary>
    public long? ModuleId { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string MenuName { get; set; }

    /// <summary>
    /// 是否为系统菜单
    /// </summary>
    public YesOrNotEnum? IsSystem { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public CommonStatusEnum Status { get; set; } = CommonStatusEnum.Enable;
}

/// <summary>
/// 添加系统菜单输入
/// </summary>
public class AddSysMenuInput
{
    /// <summary>
    /// 编码
    /// </summary>
    [Required(ErrorMessage = "编码不能为空")]
    public string MenuCode { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [Required(ErrorMessage = "名称不能为空")]
    public string MenuName { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string MenuTitle { get; set; }

    /// <summary>
    /// 父级Id
    /// </summary>
    [Required(ErrorMessage = "父级Id不能为空")]
    public long ParentId { get; set; }

    /// <summary>
    /// 模块Id
    /// </summary>
    [Required(ErrorMessage = "模块Id不能为空")]
    public long ModuleId { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    [Required(ErrorMessage = "类型不能为空")]
    public MenuTypeEnum MenuType { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 路由地址
    /// </summary>
    public string Router { get; set; }

    /// <summary>
    /// 组件地址
    /// </summary>
    public string Component { get; set; }

    /// <summary>
    /// 内链/外链地址
    /// </summary>
    public string Link { get; set; }

    /// <summary>
    /// 是否显示
    /// </summary>
    [Required(ErrorMessage = "是否显示不能为空")]
    public YesOrNotEnum Visible { get; set; }

    /// <summary>
    /// 是否为系统菜单
    /// </summary>
    [Required(ErrorMessage = "是否为系统菜单不能为空")]
    public YesOrNotEnum IsSystem { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Required(ErrorMessage = "排序不能为空")]
    public int Sort { get; set; }
}

/// <summary>
/// 更新系统菜单输入
/// </summary>
public class UpdateSysMenuInput : AddSysMenuInput
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