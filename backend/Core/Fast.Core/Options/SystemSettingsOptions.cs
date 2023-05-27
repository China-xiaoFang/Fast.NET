using Fast.Core.AdminEnum;
using Fast.Iaas.Internal;

namespace Fast.Core.Options;

/// <summary>
/// 系统配置
/// </summary>
public class SystemSettingsOptions
{
    /// <summary>
    /// 接口版本
    /// </summary>
    public string ApiVersion { get; set; }

    /// <summary>
    /// Web版本
    /// </summary>
    public string WebVersion { get; set; }

    /// <summary>
    /// 最大请求Body Size
    /// </summary>
    public long MaxRequestBodySize { get; set; }

    /// <summary>
    /// 初始化数据库
    /// </summary>
    public bool InitDataBase { get; set; }

    /// <summary>
    /// 同步接口信息
    /// </summary>
    public bool InitApiInfo { get; set; }

    /// <summary>
    /// 同步枚举字典
    /// </summary>
    public bool SyncEnumDict { get; set; }

    /// <summary>
    /// 同步应用本地化配置
    /// </summary>
    public bool SyncAppLocalization { get; set; }

    /// <summary>
    /// 系统环境
    /// </summary>
    public EnvironmentEnum Environment { get; set; }

    /// <summary>
    /// 演示环境请求禁止行为
    /// </summary>
    public List<HttpRequestActionEnum> DemoEnvReqDisable { get; set; }

    /// <summary>
    /// 多语言
    /// </summary>
    public bool AppLocalization { get; set; }

    /// <summary>
    /// 接口限流
    /// </summary>
    public bool RequestLimit { get; set; }

    /// <summary>
    /// 请求AES解密
    /// </summary>
    public bool RequestAESDecrypt { get; set; }

    /// <summary>
    /// 演示环境判断
    /// </summary>
    public bool DemoEnvironmentRequest { get; set; }
}