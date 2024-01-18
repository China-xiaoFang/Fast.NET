using Fast.Admin.Service.Authentication.Auth;
using Fast.Admin.Service.Authentication.Auth.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Application.Authentication.Auth;

/// <summary>
/// 鉴权
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Auth, Name = "auth", Order = 101)]
public class AuthApplication : IDynamicApplication
{
    private readonly IAuthService _authService;

    public AuthApplication(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// 获取登录用户信息
    /// </summary>
    /// <returns></returns>
    [HttpPost("/getLoginUserInfo")]
    public async Task<GetLoginUserInfoOutput> GetLoginUserInfo()
    {
        return await _authService.GetLoginUserInfo();
    }
}