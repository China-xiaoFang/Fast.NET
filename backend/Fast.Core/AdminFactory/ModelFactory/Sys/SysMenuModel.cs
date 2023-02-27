using Fast.Core.AdminFactory.EnumFactory;
using Fast.SqlSugar.Tenant.AttributeFilter;
using Fast.SqlSugar.Tenant.BaseModel;

namespace Fast.Core.AdminFactory.ModelFactory.Sys;

/// <summary>
/// 系统菜单表Model类
/// </summary>
[SugarTable("Sys_Menu", "系统菜单表")]
[SugarDbType]
public class SysMenuModel : BaseEntity
{
    /// <summary>
    /// 编码
    /// </summary>
    [SugarColumn(ColumnDescription = "编码", ColumnDataType = "Nvarchar(100)", IsNullable = false)]
    public string Code { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [SugarColumn(ColumnDescription = "名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string Name { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    [SugarColumn(ColumnDescription = "标题", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string Title { get; set; }

    /// <summary>
    /// 父级Id
    /// </summary>
    [SugarColumn(ColumnDescription = "父级Id", IsNullable = false)]
    public long ParentId { get; set; }

    /// <summary>
    /// 模块Id
    /// </summary>
    [SugarColumn(ColumnDescription = "模块Id", IsNullable = false)]
    public long ModuleId { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    [SugarColumn(ColumnDescription = "类型", IsNullable = false)]
    public MenuTypeEnum Type { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [SugarColumn(ColumnDescription = "图标", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string Icon { get; set; }

    /// <summary>
    /// 路由地址
    /// </summary>
    [SugarColumn(ColumnDescription = "路由地址", ColumnDataType = "Nvarchar(100)", IsNullable = true)]
    public string Router { get; set; }

    /// <summary>
    /// 组件地址
    /// </summary>
    [SugarColumn(ColumnDescription = "组件地址", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string Component { get; set; }

    /// <summary>
    /// 内链/外链地址
    /// </summary>
    [SugarColumn(ColumnDescription = "内链/外链地址", ColumnDataType = "Nvarchar(500)", IsNullable = true)]
    public string Link { get; set; }

    /// <summary>
    /// 是否为系统菜单
    /// </summary>
    [SugarColumn(ColumnDescription = "是否为系统菜单", IsNullable = false)]
    public bool IsSystem { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序", IsNullable = false)]
    public int Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", IsNullable = false)]
    public CommonStatusEnum Status { get; set; }
}