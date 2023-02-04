using Fast.Core.AdminFactory.ServiceFactory.Auth.Dto;

namespace Fast.Core.AdminFactory.ServiceFactory.Auth;

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