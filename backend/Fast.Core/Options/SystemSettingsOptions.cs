namespace Fast.Core.Options;

/// <summary>
/// 系统配置
/// </summary>
public class SystemSettingsOptions
{
    /// <summary>
    /// 最大请求Body Size
    /// </summary>
    public long MaxRequestBodySize { get; set; }

    /// <summary>
    /// 初始化数据库
    /// </summary>
    public bool InitDataBase { get; set; }
}