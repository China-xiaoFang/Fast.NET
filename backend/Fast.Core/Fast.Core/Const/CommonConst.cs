namespace Fast.Core.Const;

/// <summary>
/// 常量
/// </summary>
public class CommonConst
{
    /// <summary>
    /// 默认密码
    /// </summary>
    public const string DefaultPassword = "123456";

    /// <summary>
    /// 默认管理员密码
    /// </summary>
    public const string DefaultAdminPassword = "Fast888888";

    /// <summary>
    /// 默认头像地址
    /// </summary>
    public const string Default_Avatar_Url = "https://gitee.com/Net-18K/Fast.NET/raw/master/frontend/public/logo.png";

    /// <summary>
    /// 正则表达式字符串
    /// </summary>
    public class RegexStr
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
    }
}