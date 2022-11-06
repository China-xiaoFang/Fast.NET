using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.SqlSugar.AttributeFilter;
using Fast.Core.SqlSugar.BaseModel;

namespace Fast.Core.AdminFactory.ModelFactory.Sys;

/// <summary>
/// 系统字典数据表Model类
/// </summary>
[SugarTable("Sys_Dict_Data", "系统字典数据表")]
[DataBaseType]
public class SysDictDataModel : BaseEntity
{
    /// <summary>
    /// 类型Id
    /// </summary>
    [SugarColumn(ColumnDescription = "类型Id")]
    public long TypeId { get; set; }

    /// <summary>
    /// 中文值
    /// </summary>
    [SugarColumn(ColumnDescription = "中文名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string ChValue { get; set; }

    /// <summary>
    /// 英文值
    /// </summary>
    [SugarColumn(ColumnDescription = "英文名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string EnValue { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    [SugarColumn(ColumnDescription = "编码", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string Code { get; set; }

    /// <summary>
    /// 顺序
    /// </summary>
    [SugarColumn(ColumnDescription = "顺序", IsNullable = false)]
    public int Sort { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string Remark { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", IsNullable = false)]
    public CommonStatusEnum Status { get; set; } = CommonStatusEnum.Enable;
}