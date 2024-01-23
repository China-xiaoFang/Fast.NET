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

using Fast.Admin.Core.Enum.Db;
using Fast.SqlSugar.IBaseEntities;

namespace Fast.Admin.Entity.Tenant.Organization;

/// <summary>
/// <see cref="TenUserOrgModel"/> 租户用户机构表Model类
/// </summary>
[SugarTable("Ten_User_Org", "租户用户机构表")]
[SugarDbType(FastDbTypeEnum.SysAdminCore)]
internal class TenUserOrgModel : IDatabaseEntity
{
    /// <summary>
    /// 用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "用户Id", IsNullable = false)]
    public long UserId { get; set; }

    /// <summary>
    /// 是否为主机构（部门）
    /// </summary>
    [SugarColumn(ColumnDescription = "是否为主机构（部门）", IsNullable = false)]
    public bool IsMainOrg { get; set; }

    /// <summary>
    /// 机构Id
    /// </summary>
    [SugarColumn(ColumnDescription = "机构Id", IsNullable = false)]
    public long OrgId { get; set; }

    /// <summary>
    /// 职位Id
    /// </summary>
    [SugarColumn(ColumnDescription = "职位Id", IsNullable = false)]
    public long PositionId { get; set; }

    /// <summary>
    /// 职级Id
    /// </summary>
    [SugarColumn(ColumnDescription = "职级Id", IsNullable = true)]
    public long? RankId { get; set; }

    /// <summary>
    /// 主管用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "主管用户Id", IsNullable = true)]
    public long? LeaderUserId { get; set; }
}