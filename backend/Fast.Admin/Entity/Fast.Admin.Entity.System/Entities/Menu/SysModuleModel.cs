using Fast.Admin.Core.Enum.Common;
using Fast.Admin.Core.Enum.System;

namespace Fast.Core.AdminModel.Sys.Menu;

/// <summary>
/// 系统模块表Model类
/// </summary>
[SugarTable("Sys_Module", "系统模块表")]
[SugarDbType]
public class SysModuleModel : BaseEntity
{
    /// <summary>
    /// 模块名称
    /// </summary>
    [SugarColumn(ColumnDescription = "模块名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string ModuleName { get; set; }

    /// <summary>
    /// 颜色
    /// </summary>
    [SugarColumn(ColumnDescription = "颜色", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string Color { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [SugarColumn(ColumnDescription = "图标", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string Icon { get; set; }

    /// <summary>
    /// 查看类型
    /// </summary>
    [SugarColumn(ColumnDescription = "查看类型", ColumnDataType = "tinyint", IsNullable = false)]
    public ModuleViewTypeEnum ViewType { get; set; }

    /// <summary>
    /// 是否为系统模块
    /// </summary>
    [SugarColumn(ColumnDescription = "是否为系统模块", ColumnDataType = "tinyint", IsNullable = false)]
    public YesOrNotEnum IsSystem { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序", IsNullable = false)]
    public int Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", ColumnDataType = "tinyint", IsNullable = false)]
    public CommonStatusEnum Status { get; set; }
}