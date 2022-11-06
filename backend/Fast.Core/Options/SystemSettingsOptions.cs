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
    /// 最大请求Body Size
    /// </summary>
    public long MaxRequestBodySize { get; set; }

    /// <summary>
    /// 初始化数据库
    /// </summary>
    public bool InitDataBase { get; set; }

    /// <summary>
    /// 同步枚举字典
    /// </summary>
    public bool SyncEnumDict { get; set; }

    /// <summary>
    /// 系统环境
    /// </summary>
    public EnvironmentEnum Environment { get; set; }

    /// <summary>
    /// 演示环境请求禁止前缀
    /// </summary>
    public List<HttpRequestPrefixEnum> DemoEnvReqDisable { get; set; }
}