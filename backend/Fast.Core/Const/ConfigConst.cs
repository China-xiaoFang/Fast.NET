namespace Fast.Core.Const;

/// <summary>
/// 配置常量类
/// </summary>
public class ConfigConst
{
    /// <summary>
    /// 版权信息
    /// </summary>
    public class Copyright
    {
        /// <summary>
        /// 备案编号
        /// </summary>
        public const string ICPCode = "Sys_ICPCode";

        /// <summary>
        /// 备案Url
        /// </summary>
        public const string ICPUrl = "Sys_ICPUrl";

        /// <summary>
        /// 公安备案编号
        /// </summary>
        public const string PublicCode = "Sys_PublicCode";

        /// <summary>
        /// 公安备案Url
        /// </summary>
        public const string PublicUrl = "Sys_PublicUrl";

        /// <summary>
        /// 版权信息
        /// </summary>
        public const string CRCode = "Sys_CRCode";

        /// <summary>
        /// 版权Url
        /// </summary>
        public const string CRUrl = "Sys_CRUrl";
    }

    /// <summary>
    /// 租户配置
    /// </summary>
    public class Tenant
    {
        /// <summary>
        /// 系统名称
        /// </summary>
        public const string WebName = "Ten_WebName";

        /// <summary>
        /// 系统Logo
        /// </summary>
        public const string WebLogo = "Ten_WebLogo";

        /// <summary>
        /// 系统描述
        /// </summary>
        public const string WebDescribe = "Ten_WebDescribe";

        /// <summary>
        /// 验证码开关
        /// </summary>
        public const string VerCodeSwitch = "Ten_VerCodeSwitch";

        /// <summary>
        /// Token过期时间
        /// </summary>
        public const string TokenExpiredTime = "Ten_TokenExpiredTime";

        /// <summary>
        /// 刷新Token过期时间
        /// </summary>
        public const string RefreshTokenExpiredTime = "Ten_RefreshTokenExpiredTime";
    }
}