namespace Fast.Core.AdminFactory.EnumFactory;

/// <summary>
/// 登陆类型
/// </summary>
public enum LoginTypeEnum
{
    /// <summary>
    /// 登陆
    /// </summary>
    [Description("登陆")]
    Login = 0,

    /// <summary>
    /// 登出
    /// </summary>
    [Description("登出")]
    Logout = 1,

    /// <summary>
    /// 注册
    /// </summary>
    [Description("注册")]
    Register = 2,

    /// <summary>
    /// 改密
    /// </summary>
    [Description("改密")]
    ChangePassword = 3,

    /// <summary>
    /// 三方授权登陆
    /// </summary>
    [Description("授权登陆")]
    AuthorizedLogin = 4
}