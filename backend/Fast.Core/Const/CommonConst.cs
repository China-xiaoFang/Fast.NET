namespace Fast.Core.Const;

public class CommonConst
{
    /// <summary>
    /// 默认密码
    /// </summary>
    public const string DEFAULT_PASSWORD = "123456";

    /// <summary>
    /// 默认管理员密码
    /// </summary>
    public const string DEFAULT_ADMIN_PASSWORD = "Fast888888";

    /// <summary>
    /// 默认头像地址
    /// </summary>
    public const string DEFAULT_Avatar_URL = "https://gitee.com/Net-18K/Fast.NET/raw/master/frontend/public/logn.png";

    /// <summary>
    /// 用户缓存
    /// </summary>
    public const string CACHE_KEY_USER = "user_";

    /// <summary>
    /// 菜单缓存
    /// </summary>
    public const string CACHE_KEY_MENU = "menu_";

    /// <summary>
    /// 权限缓存
    /// </summary>
    public const string CACHE_KEY_PERMISSION = "permission_";

    /// <summary>
    /// 数据范围缓存
    /// </summary>
    public const string CACHE_KEY_DATA_SCOPE = "datascope_";

    /// <summary>
    /// 用户数据权限范围缓存
    /// </summary>
    public const string CACHE_KEY_USER_DATA_SCOPE = "userDataScope_";

    /// <summary>
    /// 租户缓存
    /// </summary>
    public const string CACHE_KEY_TENANT_INFO = "tenantInfo";

    /// <summary>
    /// 所有权限缓存
    /// </summary>
    public const string CACHE_KEY_ALL_PERMISSION = "allPermission_";

    /// <summary>
    /// 在线用户缓存
    /// </summary>
    public const string CACHE_KEY_ONLINE_USER = "onlineUser";

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