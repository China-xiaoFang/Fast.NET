using Fast.Admin.Model.Enum;

namespace Fast.Admin.Service.Auth.Dto;

/// <summary>
/// Web登录输入
/// </summary>
public class WebLoginInput
{
    /// <summary>
    /// 账号/邮箱/手机号码
    /// </summary>
    /// <example>superAdmin</example>
    [Required(ErrorMessage = "账号/邮箱/手机号码不能为空"), MinLength(3, ErrorMessage = "账号/邮箱/手机号码不能少于3位字符")]
    public string Account { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码不能为空"), MinLength(6, ErrorMessage = "密码不能少于6位字符")]
    public string Password { get; set; }

    /// <summary>
    /// 登录方式
    /// </summary>
    [Required(ErrorMessage = "登录方式不能为空")]
    public LoginMethodEnum LoginMethod { get; set; } = LoginMethodEnum.Account;
}