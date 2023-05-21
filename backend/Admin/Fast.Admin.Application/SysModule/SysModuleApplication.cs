using Fast.Admin.Service.SysModule;
using Fast.Admin.Service.SysModule.Dto;
using Fast.Core.Const;
using Fast.Core.Restful.Internal;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Application.SysModule;

/// <summary>
/// 系统模块接口
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Web, Name = "SysModule", Order = 100)]
public class SysModuleApplication : IDynamicApiController
{
    private readonly ISysModuleService _sysModuleService;

    public SysModuleApplication(ISysModuleService sysModuleService)
    {
        _sysModuleService = sysModuleService;
    }

    /// <summary>
    /// 分页查询模块信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page", "分页查询模块信息", HttpRequestActionEnum.Page)]
    public async Task<PageResult<SysModuleOutput>> QuerySysModulePageList([FromQuery] QuerySysModuleInput input)
    {
        return await _sysModuleService.QuerySysModulePageList(input);
    }

    /// <summary>
    /// 查询系统模块选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet("sysModuleSelector", "查询系统模块选择器", HttpRequestActionEnum.Page)]
    public async Task<List<SysModuleOutput>> QuerySysModuleSelector()
    {
        return await _sysModuleService.QuerySysModuleSelector();
    }

    /// <summary>
    /// 添加系统模块
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add", "添加系统模块", HttpRequestActionEnum.Add)]
    public async Task AddModule(AddModuleInput input)
    {
        await _sysModuleService.AddModule(input);
    }

    /// <summary>
    /// 更新系统模块
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPut("update", "更新系统模块", HttpRequestActionEnum.Update)]
    public async Task UpdateModule(UpdateModuleInput input)
    {
        await _sysModuleService.UpdateModule(input);
    }
}