namespace Fast.Core.Const;

/// <summary>
/// 配置常量类
/// </summary>
public class ConfigConst
{
    public class System
    {
        /// <summary>
        /// AES 加密解密
        /// </summary>
        public const string AESCrypto = "Sys_Crypto";
    }

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
    /// 百度翻译
    /// </summary>
    public class BaiduTranslator
    {
        /// <summary>
        /// 百度翻译开发者AppId
        /// </summary>
        public const string AppId = "Sys_BaiduTranslatorAppId";

        /// <summary>
        /// 百度翻译开发者密钥
        /// </summary>
        public const string SecretKey = "Sys_BaiduTranslatorSecretKey";

        /// <summary>
        /// 百度翻译请求Url
        /// </summary>
        public const string Url = "Sys_BaiduTranslatorUrl";
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

        /// <summary>
        /// 是否为演示环境
        /// </summary>
        public const string DemoEnvironment = "Ten_DemoEnvironment";

        /// <summary>
        /// 演示环境，请求禁用
        /// </summary>
        public const string DemoEnvironmentRequestDisabled = "Ten_DemoEnvironmentRequestDisabled";
    }
}