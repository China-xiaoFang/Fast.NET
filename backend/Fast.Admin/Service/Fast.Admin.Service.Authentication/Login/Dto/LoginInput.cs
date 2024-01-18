// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using System.ComponentModel.DataAnnotations;
using Fast.Admin.Core.Enum.Login;

namespace Fast.Admin.Service.Authentication.Login.Dto;

/// <summary>
/// <see cref="LoginInput"/> 登录输入
/// </summary>
public class LoginInput
{
    /// <summary>
    /// 账号/邮箱/手机号码
    /// </summary>
    /// <example>superAdmin</example>
    [StringRequired(ErrorMessage = "账号/邮箱/手机号码不能为空"), MinLength(3, ErrorMessage = "账号/邮箱/手机号码不能少于3位字符")]
    public string Account { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [StringRequired(ErrorMessage = "密码不能为空"), MinLength(6, ErrorMessage = "密码不能少于6位字符")]
    public string Password { get; set; }

    /// <summary>
    /// 登录方式
    /// </summary>
    [EnumRequired(ErrorMessage = "登录方式不能为空")]
    public LoginMethodEnum LoginMethod { get; set; } = LoginMethodEnum.Account;
}