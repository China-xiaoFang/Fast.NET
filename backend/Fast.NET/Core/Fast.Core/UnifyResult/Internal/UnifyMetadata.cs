namespace Fast.Core.UnifyResult.Internal;

/// <summary>
/// 规范化元数据
/// </summary>
internal sealed class UnifyMetadata
{
    /// <summary>
    /// 提供器名称
    /// </summary>
    public string ProviderName { get; set; }

    /// <summary>
    /// 提供器类型
    /// </summary>
    public Type ProviderType { get; set; }

    /// <summary>
    /// 统一的结果类型
    /// </summary>
    public Type ResultType { get; set; }
}