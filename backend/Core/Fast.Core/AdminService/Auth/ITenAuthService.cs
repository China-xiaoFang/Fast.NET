using Fast.Core.AdminService.Auth.Dto;

namespace Fast.Core.AdminService.Auth;

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

    /// <summary>
    /// 获取登录用户信息
    /// </summary>
    /// <returns></returns>
    Task<GetLoginUserInfoOutput> GetLoginUserInfo();

    /// <summary>
    /// 获取登录菜单
    /// </summary>
    /// <returns></returns>
    Task<List<AntDesignRouterOutput>> GetLoginMenu();
}