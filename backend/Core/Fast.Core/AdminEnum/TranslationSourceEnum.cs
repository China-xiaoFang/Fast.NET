using System.ComponentModel;
using Fast.Iaas.Attributes;

namespace Fast.Core.AdminEnum;

/// <summary>
/// 应用本地化配置来源
/// </summary>
[FastEnum("应用本地化配置来源")]
public enum TranslationSourceEnum
{
    /// <summary>
    /// 自定义
    /// </summary>
    [Description("自定义")]
    Custom = 1,

    /// <summary>
    /// 百度翻译
    /// </summary>
    [Description("百度翻译")]
    BaiduTranslate = 2,
}