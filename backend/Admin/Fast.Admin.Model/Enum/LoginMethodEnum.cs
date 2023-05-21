using System.ComponentModel;
using Fast.Iaas.Internal;

namespace Fast.Admin.Model.Enum;

/// <summary>
/// 登录方式枚举
/// </summary>
[FastEnum("登录方式枚举")]
public enum LoginMethodEnum
{
    /// <summary>
    /// 账号
    /// </summary>
    [Description("账号")]
    Account = 1,

    /// <summary>
    /// 工号
    /// </summary>
    [Description("工号")]
    JobNum = 2,

    /// <summary>
    /// 手机号
    /// </summary>
    [Description("手机号")]
    Phone = 3,

    /// <summary>
    /// 邮箱
    /// </summary>
    [Description("邮箱")]
    Email = 4
}