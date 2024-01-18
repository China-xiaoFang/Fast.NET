using Fast.Admin.Core.Enum.Common;

namespace Fast.Core.AdminModel.Sys.Dic;

/// <summary>
/// 系统字典数据表Model类
/// </summary>
[SugarTable("Sys_Dict_Data", "系统字典数据表")]
[SugarDbType]
public class SysDictDataModel : BaseEntity
{
    /// <summary>
    /// 类型Id
    /// </summary>
    [SugarColumn(ColumnDescription = "类型Id")]
    public long TypeId { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [SugarColumn(ColumnDescription = "名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string Name { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    [SugarColumn(ColumnDescription = "值", ColumnDataType = "Nvarchar(200)", IsNullable = false)]
    public string Value { get; set; }

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
    [SugarColumn(ColumnDescription = "状态", ColumnDataType = "tinyint", IsNullable = false)]
    public CommonStatusEnum Status { get; set; } = CommonStatusEnum.Enable;

    /// <summary>
    /// 字典类型
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(TypeId))]
    public virtual SysDictTypeModel DictType { get; set; }
}