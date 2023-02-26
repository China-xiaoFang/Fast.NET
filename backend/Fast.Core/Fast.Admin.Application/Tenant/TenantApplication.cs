using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.AdminFactory.ServiceFactory.Tenant;
using Fast.Core.AdminFactory.ServiceFactory.Tenant.Dto;
using Fast.Core.Const;
using Fast.Core.Internal.Restful.Internal;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Application.Tenant;

/// <summary>
/// 租户接口
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Web, Name = "Tenant", Order = 100)]
public class TenantApplication : IDynamicApiController
{
    private readonly ISysTenantService _sysTenantService;

    public TenantApplication(ISysTenantService sysTenantService)
    {
        _sysTenantService = sysTenantService;
    }

    /// <summary>
    /// Web站点初始化
    /// </summary>
    /// <returns></returns>
    [HttpGet("webSiteInit", "Web站点初始化", HttpRequestActionEnum.Query), AllowAnonymous, DisableOpLog]
    public async Task<WebSiteInitOutput> WebSiteInit()
    {
        return await _sysTenantService.WebSiteInit();
    }

    /// <summary>
    /// 分页查询租户信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("sysTenant/page", "分页查询租户信息", HttpRequestActionEnum.Query)]
    public async Task<PageResult<TenantOutput>> QueryTenantPageList([FromQuery] QueryTenantInput input)
    {
        return await _sysTenantService.QueryTenantPageList(input);
    }

    /// <summary>
    /// 添加租户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("sysTenant/add", "添加租户", HttpRequestActionEnum.Add)]
    public async Task AddTenant(AddTenantInput input)
    {
        await _sysTenantService.AddTenant(input);
    }

    /// <summary>
    /// 初始化租户信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("sysTenant/initTenantInfo", "初始化租户信息", HttpRequestActionEnum.Add)]
    public async Task InitTenantInfo(InitTenantInfoInput input)
    {
        await _sysTenantService.InitTenantInfo(input);
    }
}