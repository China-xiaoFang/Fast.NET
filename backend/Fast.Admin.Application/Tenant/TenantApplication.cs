using Fast.Core.AdminFactory.ServiceFactory.Tenant;
using Fast.Core.AdminFactory.ServiceFactory.Tenant.Dto;
using Fast.Core.Restful.Internal;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Application.Tenant;

/// <summary>
/// 租户接口
/// </summary>
[ApiDescriptionSettings(Name = "Tenant", Order = 100)]
public class TenantApplication : IDynamicApiController
{
    private readonly ISysTenantService _sysTenantService;

    public TenantApplication(ISysTenantService sysTenantService)
    {
        _sysTenantService = sysTenantService;
    }

    /// <summary>
    /// 分页查询租户信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("sysTenant/page")]
    public async Task<PageResult<TenantOutput>> QueryTenantPageList([FromQuery] QueryTenantInput input)
    {
        return await _sysTenantService.QueryTenantPageList(input);
    }

    /// <summary>
    /// 添加租户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("sysTenant/add")]
    public async Task AddTenant(AddTenantInput input)
    {
        await _sysTenantService.AddTenant(input);
    }

    /// <summary>
    /// 初始化租户信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("sysTenant/initTenantInfo")]
    public async Task InitTenantInfo(InitTenantInfoInput input)
    {
        await _sysTenantService.InitTenantInfo(input);
    }
}