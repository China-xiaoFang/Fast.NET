namespace Fast.Core.FriendlyException.Extensions.Options;

/// <summary>
/// AddInject 友好异常配置选项
/// </summary>
public sealed class FriendlyExceptionOptions
{
    /// <summary>
    /// 是否启用全局友好异常
    /// </summary>
    public bool GlobalEnabled { get; set; } = true;
}