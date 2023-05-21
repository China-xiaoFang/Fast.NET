namespace Fast.Core.Operation.Config.Dto;

/// <summary>
/// 配置信息
/// </summary>
public class ConfigInfo
{
    /// <summary>
    /// 编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 中文名称
    /// </summary>
    public string ChName { get; set; }

    /// <summary>
    /// 英文名称
    /// </summary>
    public string EnName { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }
}