using Fast.SqlSugar.Attributes;

namespace Fast.Core.AdminModel.Sys;

/// <summary>
/// 租户App授权信息表Model类
/// </summary>
[SugarTable("Sys_Tenant_App_Info", "租户App授权信息表")]
[SugarDbType]
public class SysTenantAppInfoModel : BaseTEntity
{
    /// <summary>
    /// App类型
    /// </summary>
    [SugarColumn(ColumnDescription = "App类型", ColumnDataType = "tinyint", IsNullable = false)]
    public AppTypeEnum AppType { get; set; }

    /// <summary>
    /// AppKey
    /// </summary>
    [SugarColumn(ColumnDescription = "AppKey", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string AppKey { get; set; }

    /// <summary>
    /// 授权开始时间
    /// </summary>
    [SugarColumn(ColumnDescription = "授权开始时间", ColumnDataType = "datetimeoffset", IsNullable = false)]
    public DateTime AuthStartTime { get; set; }

    /// <summary>
    /// 授权结束时间
    /// </summary>
    [SugarColumn(ColumnDescription = "授权结束时间", ColumnDataType = "datetimeoffset", IsNullable = false)]
    public DateTime AuthEndTime { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string Remark { get; set; }
}