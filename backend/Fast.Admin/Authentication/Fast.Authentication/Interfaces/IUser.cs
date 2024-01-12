using Fast.Authentication.Infrastructures;
using Fast.NET.Core.Enum;

// ReSharper disable once CheckNamespace
namespace Fast.Authentication;

/// <summary>
/// 授权用户信息
/// 作用域注册，保证当前请求管道中是唯一的，并且只会加载一次
/// </summary>
public interface IUser
{
    /// <summary>
    /// 当前租户Id
    /// </summary>
    long TenantId { get; }

    /// <summary>
    /// 当前用户Id
    /// </summary>
    long UserId { get; }

    /// <summary>
    /// 当前用户账号
    /// </summary>
    string UserAccount { get; }

    /// <summary>
    /// 当前用户工号
    /// </summary>
    string UserJobNum { get; }

    /// <summary>
    /// 当前用户名称
    /// </summary>
    string UserName { get; }

    /// <summary>
    /// 是否超级管理员
    /// </summary>
    bool IsSuperAdmin { get; }

    /// <summary>
    /// 是否系统管理员
    /// </summary>
    bool IsSystemAdmin { get; }

    /// <summary>
    /// 是否租户管理员
    /// </summary>
    bool IsTenantAdmin { get; }

    /// <summary>
    /// App 运行环境
    /// </summary>
    AppEnvironmentEnum AppEnvironment { get; }

    /// <summary>
    /// 统一登录
    /// </summary>
    /// <param name="authUserInfo"></param>
    /// <returns></returns>
    Task LoginAsync(AuthUserInfo authUserInfo);

    /// <summary>
    /// 统一退出登录
    /// </summary>
    /// <returns></returns>
    Task LogoutAsync();
}