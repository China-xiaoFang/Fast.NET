namespace Fast.Core.AdminFactory.ModelFactory.Basis;

/// <summary>
/// 系统组织架构表Model类
/// </summary>
[SugarTable("Sys_Org", "系统组织架构表")]
[DataBaseType(SysDataBaseTypeEnum.Tenant)]
public class SysOrgModel : BaseEntity
{
    /// <summary>
    /// 父级Id
    /// </summary>
    [SugarColumn(ColumnDescription = "父级Id", IsNullable = false)]
    public long ParentId { get; set; }

    /// <summary>
    /// 父级Id集合
    /// </summary>
    [SugarColumn(ColumnDescription = "父级Id集合", IsNullable = false, IsJson = true)]
    public List<long> ParentIds { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [SugarColumn(ColumnDescription = "名称", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string Name { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    [SugarColumn(ColumnDescription = "编码", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string Code { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    [SugarColumn(ColumnDescription = "联系人", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string Contacts { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    [SugarColumn(ColumnDescription = "电话", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string Tel { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序", IsNullable = false)]
    public int Sort { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string Remark { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", IsNullable = false)]
    public CommonStatusEnum Status { get; set; } = CommonStatusEnum.Enable;
}