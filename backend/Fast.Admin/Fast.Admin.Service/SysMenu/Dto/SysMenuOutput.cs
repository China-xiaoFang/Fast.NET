using System.Collections;
using Fast.Admin.Service.SysButton.Dto;

namespace Fast.Admin.Service.SysMenu.Dto;

public class QuerySysMenuTreeBaseOutput : BaseOutput, ITreeNode
{
    /// <summary>
    /// 获取节点id
    /// </summary>
    /// <returns></returns>
    public long GetId()
    {
        return Id;
    }

    /// <summary>
    /// 获取节点父id
    /// </summary>
    /// <returns></returns>
    public long GetPid()
    {
        return ParentId;
    }

    /// <summary>
    /// 设置Children
    /// </summary>
    /// <param name="children"></param>
    public void SetChildren(IList children)
    {
        Children = (List<QuerySysMenuTreeBaseOutput>) children;
    }

    /// <summary>
    /// 编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 父级Id
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 模块Id
    /// </summary>
    public long ModuleId { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public MenuTypeEnum Type { get; set; }

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
    /// 是否为系统菜单
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

    /// <summary>
    /// 按钮集合
    /// </summary>
    public List<SysButtonOutput> ButtonList { get; set; }

    /// <summary>
    /// 子节点
    /// </summary>
    public List<QuerySysMenuTreeBaseOutput> Children { get; set; }
}