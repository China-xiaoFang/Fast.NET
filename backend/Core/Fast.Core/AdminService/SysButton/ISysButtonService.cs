using Fast.Core.AdminService.SysButton.Dto;

namespace Fast.Core.AdminService.SysButton;

/// <summary>
/// 系统按钮服务接口
/// </summary>
public interface ISysButtonService
{
    /// <summary>
    /// 根据菜单Id查询系统按钮
    /// </summary>
    /// <param name="menuId"></param>
    /// <returns></returns>
    Task<List<SysButtonOutput>> QuerySysButtonListByMenuId(long menuId);

    /// <summary>
    /// 添加系统按钮
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task AddSysButton(AddSysButtonInput input);

    /// <summary>
    /// 更新系统按钮
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task UpdateSysButton(UpdateSysButtonInput input);

    /// <summary>
    /// 删除系统按钮
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteSysButton(long id);
}