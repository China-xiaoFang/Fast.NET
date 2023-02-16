using Fast.Core.AdminFactory.ServiceFactory.Auth;
using Fast.Core.AdminFactory.ServiceFactory.Auth.Dto;
using Fast.Core.Const;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Application.Auth;

/// <summary>
/// 租户授权接口
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Web, Name = "Auth", Order = 100)]
public class AuthApplication : IDynamicApiController
{
    private readonly ITenAuthService _tenAuthService;

    public AuthApplication(ITenAuthService tenAuthService)
    {
        _tenAuthService = tenAuthService;
    }

    /// <summary>
    /// Web登录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("webLogin", "Web登录"), AllowAnonymous, DisableOpLog]
    public async Task WebLogin(WebLoginInput input)
    {
        await _tenAuthService.WebLogin(input);
    }

    /// <summary>
    /// 测试Get请求参数加密
    /// </summary>
    /// <param name="account"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    [HttpGet("testGet1", "Web登录"), AllowAnonymous, DisableOpLog]
    public async Task TestGet1(string account, string password)
    {
        await _tenAuthService.WebLogin(new WebLoginInput {Account = account, Password = password});
    }

    /// <summary>
    /// 测试Get请求参数加密
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("testGet2", "Web登录"), AllowAnonymous, DisableOpLog]
    public async Task TestGet2([FromQuery] WebLoginInput input)
    {
        await _tenAuthService.WebLogin(input);
    }
}