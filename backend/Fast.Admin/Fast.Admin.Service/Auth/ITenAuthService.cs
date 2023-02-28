using Fast.Admin.Service.Auth.Dto;

namespace Fast.Admin.Service.Auth;

/// <summary>
/// 租户授权服务接口
/// </summary>
public interface ITenAuthService
{
    /// <summary>
    /// Web登录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task WebLogin(WebLoginInput input);
}