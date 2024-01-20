// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Fast.Admin.Core.Enum.Common;
using Fast.Admin.Core.Enum.Db;

namespace Fast.Admin.Entity.Tenant.Organization;

/// <summary>
/// <see cref="TenOrgModel"/> 租户组织架构表Model类
/// </summary>
[SugarTable("Ten_Org", "租户组织架构表")]
[SugarDbType(FastDbTypeEnum.SysAdminCore)]
public class TenOrgModel : BaseEntity
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
    /// 组织名称
    /// </summary>
    [SugarColumn(ColumnDescription = "组织名称", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string OrgName { get; set; }

    /// <summary>
    /// 组织编码
    /// </summary>
    [SugarColumn(ColumnDescription = "组织编码", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string OrgCode { get; set; }

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