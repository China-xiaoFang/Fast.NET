using Fast.Core.ConfigurableOptions.Options;

namespace Fast.Core.DataValidation.Options;

/// <summary>
/// 验证消息配置选项
/// </summary>
public sealed class ValidationTypeMessageSettingsOptions : IConfigurableOptions
{
    /// <summary>
    /// 验证消息配置表
    /// </summary>
    public object[][] Definitions { get; set; }
}