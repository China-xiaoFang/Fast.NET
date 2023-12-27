namespace Fast.Core.AdminModel.Sys.Dic;

/// <summary>
/// 系统字典类型表Model类
/// </summary>
[SugarTable("Sys_Dict_Type", "系统字典类型表")]
[SugarDbType]
public class SysDictTypeModel : BaseEntity
{
    /// <summary>
    /// 编码
    /// </summary>
    [SugarColumn(ColumnDescription = "编码", ColumnDataType = "Nvarchar(50)", IsNullable = false,
        UniqueGroupNameList = new[] {nameof(Code)})]
    public string Code { get; set; }

    /// <summary>
    /// 中文名称
    /// </summary>
    [SugarColumn(ColumnDescription = "中文名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string ChName { get; set; }

    /// <summary>
    /// 英文名称
    /// </summary>
    [SugarColumn(ColumnDescription = "英文名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string EnName { get; set; }

    /// <summary>
    /// 级别
    /// </summary>
    [SugarColumn(ColumnDescription = "级别", ColumnDataType = "tinyint", IsNullable = false)]
    public SysLevelEnum Level { get; set; }

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
    /// 数据集合
    /// </summary>
    [Navigate(NavigateType.OneToMany, nameof(SysDictDataModel.TypeId))]
    public virtual List<SysDictDataModel> DataList { get; set; }
}