namespace Fast.Core.AdminModel.Sys.Api;

/// <summary>
/// 系统接口分组信息表Model类
/// </summary>
[SugarTable("Sys_Api_Group_Info", "系统接口分组信息表")]
[SugarDbType]
public class SysApiGroupInfoModel : BaseEntity
{
    /// <summary>
    /// 接口分组名称
    /// </summary>
    [SugarColumn(ColumnDescription = "接口分组名称", ColumnDataType = "Nvarchar(100)", IsNullable = false)]
    public string ApiGroupName { get; set; }

    /// <summary>
    /// 接口父级分组名称
    /// </summary>
    [SugarColumn(ColumnDescription = "接口父级分组名称", ColumnDataType = "Nvarchar(100)", IsNullable = false)]
    public string ApiParentGroupName { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序", IsNullable = false)]
    public int Sort { get; set; }
}