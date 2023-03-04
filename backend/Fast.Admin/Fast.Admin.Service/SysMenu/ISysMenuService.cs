using Fast.Admin.Service.SysMenu.Dto;

namespace Fast.Admin.Service.SysMenu;

/// <summary>
/// 系统菜单服务接口
/// </summary>
public interface ISysMenuService
{
    /// <summary>
    /// 查询系统菜单树形
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<List<QuerySysMenuTreeBaseOutput>> QuerySysMenuTree(QuerySysMenuTreeInput input);

    /// <summary>
    /// 添加系统菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task AddSysMenu(AddSysMenuInput input);

    /// <summary>
    /// 更新系统菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task UpdateSysMenu(UpdateSysMenuInput input);
}