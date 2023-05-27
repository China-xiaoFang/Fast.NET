using Fast.Admin.Service.SysButton;
using Fast.Admin.Service.SysButton.Dto;
using Fast.Core.Const;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Application.SysButton;

/// <summary>
/// 系统按钮接口
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Web, Name = "SysButton", Order = 100)]
public class SysButtonApplication : IDynamicApiController
{
    private readonly ISysButtonService _sysButtonService;

    public SysButtonApplication(ISysButtonService sysButtonService)
    {
        _sysButtonService = sysButtonService;
    }

    /// <summary>
    /// 根据菜单Id查询系统按钮
    /// </summary>
    /// <param name="menuId"></param>
    /// <returns></returns>
    [HttpGet("page", "根据菜单Id查询系统按钮", HttpRequestActionEnum.Page)]
    public async Task<List<SysButtonOutput>> QuerySysButtonListByMenuId(long menuId)
    {
        return await _sysButtonService.QuerySysButtonListByMenuId(menuId);
    }

    /// <summary>
    /// 添加系统按钮
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add", "添加系统按钮", HttpRequestActionEnum.Add)]
    public async Task AddSysButton(AddSysButtonInput input)
    {
        await _sysButtonService.AddSysButton(input);
    }

    /// <summary>
    /// 更新系统按钮
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task UpdateSysButton(UpdateSysButtonInput input)
    {
        await _sysButtonService.UpdateSysButton(input);
    }

    /// <summary>
    /// 删除系统按钮
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("delete", "删除系统按钮", HttpRequestActionEnum.Delete)]
    public async Task DeleteSysButton(long id)
    {
        await _sysButtonService.DeleteSysButton(id);
    }
}