namespace Fast.Iaas.Const;

/// <summary>
/// 正则表达式字符串
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
}