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

namespace Fast.Admin.Core.Constants;

/// <summary>
/// <see cref="RegexConst"/> 正则表达式常量
/// </summary>
public class RegexConst
{
    /// <summary>
    /// 中文、英文、数字包括下划线
    /// </summary>
    public const string ChEnNum_ = "/^[\u4E00-\u9FA5A-Za-z0-9_]+$/";

    /// <summary>
    /// 中文、英文、数字但不包括下划线等符号
    /// </summary>
    public const string ChEnNum = "^[\u4E00-\u9FA5A-Za-z0-9]+$";

    /// <summary>
    /// 中文
    /// </summary>
    public const string Chinese = "^[\u4e00-\u9fa5]{0,}$";

    /// <summary>
    /// Http地址判断
    /// </summary>
    public const string HttpUrl = "/(http):\\/\\/([\\w.]+\\/?)\\S*/";

    /// <summary>
    /// Https地址判断
    /// </summary>
    public const string HttpsUrl = "/(https):\\/\\/([\\w.]+\\/?)\\S*/";

    /// <summary>
    /// Http或者Https地址判断
    /// </summary>
    public const string HttpOrHttpsUrl = "/(http|https):\\/\\/([\\w.]+\\/?)\\S*/";

    /// <summary>
    /// 邮箱地址判断
    /// </summary>
    public const string EmailAddress =
        "^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";

    /// <summary>
    /// 手机号码判断
    /// </summary>
    public const string Mobile = @"^1[3456789]\d{9}$";
}