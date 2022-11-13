using Fast.Core.AdminFactory.EnumFactory;

namespace Fast.Core.Operation.Config.Dto;

/// <summary>
/// 得到配置类型输入
/// </summary>
public class GetConfigInput
{
    /// <summary>
    /// 编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 配置类型
    /// 默认系统配置
    /// </summary>
    public SysConfigTypeEnum ConfigType { get; set; } = SysConfigTypeEnum.System;
}