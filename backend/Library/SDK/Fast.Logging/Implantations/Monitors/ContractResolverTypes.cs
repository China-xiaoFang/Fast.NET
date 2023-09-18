namespace Fast.Logging.Implantations.Monitors;

/// <summary>
/// LoggingMonitor 序列化属性命名规则选项
/// </summary>
public enum ContractResolverTypes
{
    /// <summary>
    /// CamelCase 小驼峰
    /// </summary>
    /// <remarks>默认值</remarks>
    CamelCase = 0,

    /// <summary>
    /// 保持原样
    /// </summary>
    Default = 1
}