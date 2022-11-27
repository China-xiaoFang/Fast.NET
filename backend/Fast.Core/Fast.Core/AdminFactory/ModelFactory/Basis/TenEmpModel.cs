using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.SqlSugar.AttributeFilter;
using Fast.Core.SqlSugar.BaseModel;

namespace Fast.Core.AdminFactory.ModelFactory.Basis;

/// <summary>
/// 租户员工表Model类
/// </summary>
[SugarTable("Ten_Emp", "租户员工表")]
[DataBaseType(SysDataBaseTypeEnum.Tenant)]
public class TenEmpModel : PrimaryKeyEntity
{
    /// <summary>
    /// 工号
    /// </summary>
    [SugarColumn(ColumnDescription = "工号", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string JobNum { get; set; }

    /// <summary>
    /// 机构Id
    /// </summary>
    [SugarColumn(ColumnDescription = "机构Id", IsNullable = false)]
    public long OrgId { get; set; }

    /// <summary>
    /// 机构名称
    /// </summary>
    [SugarColumn(ColumnDescription = "机构名称", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string OrgName { get; set; }
}