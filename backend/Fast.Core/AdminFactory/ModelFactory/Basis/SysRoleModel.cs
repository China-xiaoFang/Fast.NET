namespace Fast.Core.AdminFactory.ModelFactory.Basis;

/// <summary>
/// 系统角色表Model类
/// </summary>
[SugarTable("Sys_Role", "系统角色表")]
[DataBaseType(SysDataBaseTypeEnum.Tenant)]
public class SysRoleModel : BaseEntity
{
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
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序", IsNullable = false)]
    public int Sort { get; set; }

    /// <summary>
    /// 数据范围类型
    /// </summary>
    [SugarColumn(ColumnDescription = "数据范围类型", IsNullable = false)]
    public DataScopeTypeEnum DataScopeType { get; set; }

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

    /// <summary>
    /// 角色类型
    /// </summary>
    [SugarColumn(ColumnDescription = "角色类型", IsNullable = false)]
    public RoleTypeEnum RoleType { get; set; }
}