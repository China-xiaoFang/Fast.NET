// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
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

namespace Fast.Gateway.Entities.Entities.Projects;

/// <summary>
/// <see cref="ProjectVersion"/> 项目版本信息表
/// </summary>
[SugarTable("ProjectVersion")]
public class ProjectVersion : BaseEntity
{
    /// <summary>
    /// 项目Id
    /// </summary>
    [SugarColumn(ColumnDescription = "项目Id", IsNullable = false)]
    public long ProjectId { get; set; }

    /// <summary>
    /// 主版本号
    /// </summary>
    [SugarColumn(ColumnDescription = "主版本号", IsNullable = false)]
    public int MainVersion { get; set; }

    /// <summary>
    /// 次版本号
    /// </summary>
    [SugarColumn(ColumnDescription = "次版本号", IsNullable = false)]
    public int SubVersion { get; set; }

    /// <summary>
    /// 修订版本号
    /// </summary>
    [SugarColumn(ColumnDescription = "修订版本号", IsNullable = false)]
    public int ReviseVersion { get; set; }

    /// <summary>
    /// 副修订版本号
    /// </summary>
    [SugarColumn(ColumnDescription = "副修订版本号", IsNullable = false)]
    public int SubReviseVersion { get; set; }

    /// <summary>
    /// 是否在线
    /// </summary>
    [SugarColumn(ColumnDescription = "是否在线", IsNullable = false)]
    public bool IsOnline { get; set; }

    /// <summary>
    /// 项目版本文件夹地址
    /// </summary>
    [SugarColumn(ColumnDescription = "项目版本文件夹地址", ColumnDataType = "NVARCHAR(500)", IsNullable = false)]
    public string FilePath { get; set; }

    /// <summary>
    /// 健康检查地址
    /// </summary>
    [SugarColumn(ColumnDescription = "健康检查地址", ColumnDataType = "NVARCHAR(200)", IsNullable = false)]
    public string HealthCheckUrl { get; set; }

    /// <summary>
    /// 启动文件名称
    /// 如果不填写，则会默认获取文件夹根目录下第一个 exe 文件名称的 dll 文件
    /// </summary>
    [SugarColumn(ColumnDescription = "启动文件名称", ColumnDataType = "NVARCHAR(100)", IsNullable = false)]
    public string StartFileName { get; set; }

    /// <summary>
    /// 当前启用IP地址
    /// </summary>
    [SugarColumn(ColumnDescription = "当前启用IP地址", ColumnDataType = "NVARCHAR(15)", IsNullable = true)]
    public string StartIP { get; set; }

    /// <summary>
    /// 当前启动端口
    /// </summary>
    [SugarColumn(ColumnDescription = "当前启动端口", IsNullable = true)]
    public int? StartPort { get; set; }

    /// <summary>
    /// 最后在线时间
    /// </summary>
    [SugarColumn(ColumnDescription = "最后在线时间", ColumnDataType = "datetimeoffset", IsNullable = true, CreateTableFieldSort = 991)]
    public virtual DateTime? LaseOnlineTime { get; set; }

    /// <summary>
    /// 项目
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(ProjectId))]
    public Project Project { get; set; }
}