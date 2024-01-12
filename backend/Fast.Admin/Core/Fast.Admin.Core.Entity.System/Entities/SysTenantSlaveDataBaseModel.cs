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

namespace Fast.Admin.Core.Entity.System.Entities;

/// <summary>
/// <see cref="SysTenantSlaveDataBaseModel"/> 系统租户从数据库Model类
/// </summary>
[SugarTable("Sys_Tenant_Slave_Database", "系统租户从数据库表")]
[SugarDbType(FastDbTypeEnum.SysCore)]
public class SysTenantSlaveDataBaseModel : BaseTEntity
{
    /// <summary>
    /// 主库Id
    /// </summary>
    [SugarColumn(ColumnDescription = "主库Id", IsNullable = false)]
    public long MainId { get; set; }

    /// <summary>
    /// 服务器Ip地址
    /// </summary>
    [SugarColumn(ColumnDescription = "服务器Ip地址", ColumnDataType = "Nvarchar(15)", IsNullable = false)]
    public string ServiceIp { get; set; }

    /// <summary>
    /// 端口号
    /// </summary>
    [SugarColumn(ColumnDescription = "端口号", ColumnDataType = "Nvarchar(5)", IsNullable = false)]
    public string Port { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string DbName { get; set; }

    /// <summary>
    /// 数据库用户
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库用户", ColumnDataType = "Nvarchar(10)", IsNullable = false)]
    public string DbUser { get; set; }

    /// <summary>
    /// 数据库密码
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库密码", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string DbPwd { get; set; }

    /// <summary>
    /// 从库命中率
    /// <remarks>
    /// <para>为 0 则不命中</para>
    /// <para>建议相加不超过10</para>
    /// </remarks>
    /// </summary>
    [SugarColumn(ColumnDescription = "从库命中率", IsNullable = false)]
    public int HitRate { get; set; }
}