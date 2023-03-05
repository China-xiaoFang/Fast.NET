using Fast.Admin.Service.SysMenu;
using Fast.Admin.Service.SysMenu.Dto;
using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.Const;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Application.SysMenu;

/// <summary>
/// 系统按钮接口
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Web, Name = "SysMenu", Order = 100)]
public class SysMenuApplication : IDynamicApiController
{
    private readonly ISysMenuService _sysMenuService;

    public SysMenuApplication(ISysMenuService sysMenuService)
    {
        _sysMenuService = sysMenuService;
    }

    /// <summary>
    /// 查询系统菜单树形
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("tree", "查询系统菜单树形", HttpRequestActionEnum.Query)]
    public async Task<List<SysMenuTreeOutput>> QuerySysMenuTree([FromQuery] QuerySysMenuTreeInput input)
    {
        return await _sysMenuService.QuerySysMenuTree(input);
    }

    /// <summary>
    /// 添加系统菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add", "添加系统菜单", HttpRequestActionEnum.Add)]
    public async Task AddSysMenu(AddSysMenuInput input)
    {
        await _sysMenuService.AddSysMenu(input);
    }

    /// <summary>
    /// 更新系统菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPut("update", "更新系统菜单", HttpRequestActionEnum.Update)]
    public async Task UpdateSysMenu(UpdateSysMenuInput input)
    {
        await _sysMenuService.UpdateSysMenu(input);
    }
}