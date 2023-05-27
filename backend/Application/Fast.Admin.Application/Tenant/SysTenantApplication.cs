using Fast.Core.AdminService.Tenant;
using Fast.Core.AdminService.Tenant.Dto;
using Fast.Core.Restful.Internal;
using Fast.Iaas.Attributes;
using Fast.Iaas.Const;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Application.Tenant;

/// <summary>
/// 租户接口
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Web, Name = "Tenant", Order = 100)]
public class SysTenantApplication : IDynamicApiController
{
    private readonly ISysTenantService _sysTenantService;

    public SysTenantApplication(ISysTenantService sysTenantService)
    {
        _sysTenantService = sysTenantService;
    }

    /// <summary>
    /// Web站点初始化
    /// </summary>
    /// <returns></returns>
    [HttpGet("/webSiteInit", "Web站点初始化", HttpRequestActionEnum.Page), AllowAnonymous, DisableOpLog]
    public async Task<WebSiteInitOutput> WebSiteInit()
    {
        return await _sysTenantService.WebSiteInit();
    }

    /// <summary>
    /// 分页查询租户信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page", "分页查询租户信息", HttpRequestActionEnum.Page)]
    public async Task<PageResult<TenantOutput>> QueryTenantPageList([FromQuery] QueryTenantInput input)
    {
        return await _sysTenantService.QueryTenantPageList(input);
    }

    /// <summary>
    /// 添加租户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add", "添加租户", HttpRequestActionEnum.Add)]
    public async Task AddTenant(AddTenantInput input)
    {
        await _sysTenantService.AddTenant(input);
    }

    /// <summary>
    /// 初始化租户信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("initTenantInfo", "初始化租户信息", HttpRequestActionEnum.Add)]
    public async Task InitTenantInfo(InitTenantInfoInput input)
    {
        await _sysTenantService.InitTenantInfo(input);
    }
}