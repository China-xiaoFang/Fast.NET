using Fast.Core.User;

namespace Fast.Core;

/// <summary>
/// 用户通用上下文
/// </summary>
public class UserContext
{
    private static IUser User => App.GetService<IUser>();

    /// <summary>
    /// 当前租户Id
    /// </summary>
    public static long TenantId => User.TenantId;

    /// <summary>
    /// 当前用户Id
    /// </summary>
    public static long UserId => User.UserId;

    /// <summary>
    /// 当前用户账号
    /// </summary>
    public static string UserAccount => User.UserAccount;

    /// <summary>
    /// 当前用户工号
    /// </summary>
    public static string UserJobNum => User.UserJobNum;

    /// <summary>
    /// 当前用户名称
    /// </summary>
    public static string UserName => User.UserName;

    /// <summary>
    /// 是否超级管理员
    /// </summary>
    public static bool IsSuperAdmin => User.IsSuperAdmin;

    /// <summary>
    /// 是否系统管理员
    /// </summary>
    public static bool IsSystemAdmin => User.IsSystemAdmin;

    /// <summary>
    /// 是否租户管理员
    /// </summary>
    public static bool IsTenantAdmin => User.IsTenantAdmin;
}