using Fast.Admin.Core.Enum.Common;
using Fast.Admin.Core.Enum.System;

namespace Fast.Core.AdminModel.Sys;

/// <summary>
/// 系统应用本地化表Model类
/// </summary>
[SugarTable("Sys_App_Localization", "系统应用本地化表")]
[SugarDbType]
public class SysAppLocalizationModel : BaseEntity
{
    /// <summary>
    /// 中文
    /// </summary>
    [SugarColumn(ColumnDescription = "名称", ColumnDataType = "Nvarchar(MAX)", IsNullable = false)]
    public string Chinese { get; set; }

    /// <summary>
    /// 英文
    /// </summary>
    [SugarColumn(ColumnDescription = "英文", ColumnDataType = "Nvarchar(MAX)", IsNullable = false)]
    public string English { get; set; }

    /// <summary>
    /// 翻译来源
    /// </summary>
    [SugarColumn(ColumnDescription = "翻译来源", ColumnDataType = "tinyint", IsNullable = false)]
    public TranslationSourceEnum TranslationSource { get; set; }

    /// <summary>
    /// 是否为系统配置
    /// </summary>
    [SugarColumn(ColumnDescription = "是否为系统配置", ColumnDataType = "tinyint", IsNullable = false)]
    public YesOrNotEnum IsSystem { get; set; }
}