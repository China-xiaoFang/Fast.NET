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
using Fast.Admin.Core.Enum.System;

namespace Fast.Admin.Entity.Tenant.Organization;

/// <summary>
/// <see cref="TenUserModel"/> 租户用户表Model类
/// </summary>
[SugarTable("Ten_User", "租户用户表")]
[SugarDbType(FastDbTypeEnum.SysAdminCore)]
public class TenUserModel : BaseEntity
{
    /// <summary>
    /// 账号Id
    /// </summary>
    [SugarColumn(ColumnDescription = "账号ID", IsNullable = false)]
    public long AccountId { get; set; }

    /// <summary>
    /// 工号
    /// </summary>
    [SugarColumn(ColumnDescription = "工号", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string JobNumber { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [SugarColumn(ColumnDescription = "昵称", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string NickName { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    [SugarColumn(ColumnDescription = "头像", ColumnDataType = "Nvarchar(max)", IsNullable = false)]
    public string Avatar { get; set; }

    /// <summary>
    /// 主部门Id
    /// </summary>
    [SugarColumn(ColumnDescription = "部门Id", IsNullable = false)]
    public long DepartmentId { get; set; }

    /// <summary>
    /// 主部门名称
    /// </summary>
    [SugarColumn(ColumnDescription = "部门名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string DepartmentName { get; set; }

    /// <summary>
    /// 管理员类型
    /// </summary>
    [SugarColumn(ColumnDescription = "管理员类型", ColumnDataType = "tinyint", IsNullable = false)]
    public AdminTypeEnum AdminType { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", ColumnDataType = "tinyint", IsNullable = false)]
    public CommonStatusEnum Status { get; set; } = CommonStatusEnum.Enable;

    /// <summary>
    /// 最后登录设备
    /// </summary>
    [SugarColumn(ColumnDescription = "最后登录设备", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string LastLoginDevice { get; set; }

    /// <summary>
    /// 最后登录操作系统（版本）
    /// </summary>
    [SugarColumn(ColumnDescription = "最后登录操作系统（版本）", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string LastLoginOS { get; set; }

    /// <summary>
    /// 最后登录浏览器（版本）
    /// </summary>
    [SugarColumn(ColumnDescription = "最后登录浏览器（版本）", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string LastLoginBrowser { get; set; }

    /// <summary>
    /// 最后登录省份
    /// </summary>
    [SugarColumn(ColumnDescription = "最后登录省份", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string LastLoginProvince { get; set; }

    /// <summary>
    /// 最后登录城市
    /// </summary>
    [SugarColumn(ColumnDescription = "最后登录城市", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string LastLoginCity { get; set; }

    /// <summary>
    /// 最后登录Ip
    /// </summary>
    [SugarColumn(ColumnDescription = "最后登录Ip", ColumnDataType = "Nvarchar(15)", IsNullable = true)]
    public string LastLoginIp { get; set; }

    /// <summary>
    /// 最后登录时间
    /// </summary>
    [SugarColumn(ColumnDescription = "最后登录时间", ColumnDataType = "datetimeoffset", IsNullable = true)]
    public DateTime? LastLoginTime { get; set; }
}