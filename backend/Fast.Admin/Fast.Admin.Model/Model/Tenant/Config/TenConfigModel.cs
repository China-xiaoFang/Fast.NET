using Fast.SqlSugar.Tenant.AttributeFilter;
using Fast.SqlSugar.Tenant.BaseModel;
using Fast.SqlSugar.Tenant.Internal.Enum;
using SqlSugar;

namespace Fast.Admin.Model.Model.Tenant.Config;

/// <summary>
/// 租户配置表Model类
/// </summary>
[SugarTable("Ten_Config", "租户配置表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenConfigModel : BaseEntity
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
    /// 值
    /// </summary>
    [SugarColumn(ColumnDescription = "值", ColumnDataType = "Nvarchar(200)", IsNullable = false)]
    public string Value { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string Remark { get; set; }
}