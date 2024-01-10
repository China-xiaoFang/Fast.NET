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
using Fast.Admin.Core.Enum.System;

namespace Fast.Admin.Core.Entity.System.Entities;

/// <summary>
/// <see cref="SysTenantModel"/> 租户信息表Model类
/// </summary>
[SugarTable("Sys_Tenant", "租户信息表")]
[SugarDbType(FastDbTypeEnum.SysCore)]
public class SysTenantModel : BaseEntity
{
    /// <summary>
    /// 租户公司中文名称
    /// </summary>
    [SugarColumn(ColumnDescription = "租户公司中文名称", ColumnDataType = "Nvarchar(50)", IsNullable = false,
        UniqueGroupNameList = new[] {nameof(ChName)})]
    public string ChName { get; set; }

    /// <summary>
    /// 租户公司英文名称（拼音）
    /// </summary>
    [SugarColumn(ColumnDescription = "租户公司英文名称", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string EnName { get; set; }

    /// <summary>
    /// 租户公司中文简称
    /// </summary>
    [SugarColumn(ColumnDescription = "租户公司中文简称", ColumnDataType = "Nvarchar(30)", IsNullable = false)]
    public string ChShortName { get; set; }

    /// <summary>
    /// 租户公司英文简称（拼音）
    /// </summary>
    [SugarColumn(ColumnDescription = "租户公司英文简称", ColumnDataType = "Nvarchar(30)", IsNullable = false)]
    public string EnShortName { get; set; }

    /// <summary>
    /// 租户密钥
    /// <remarks>32位长度</remarks>
    /// </summary>
    [SugarColumn(ColumnDescription = "租户密钥", ColumnDataType = "Nvarchar(32)", IsNullable = false,
        UniqueGroupNameList = new[] {nameof(Secret)})]
    public string Secret { get; set; }

    /// <summary>
    /// 租户公钥
    /// </summary>
    [SugarColumn(ColumnDescription = "租户公钥", ColumnDataType = "Nvarchar(MAX)", IsNullable = false)]
    public string PublicKey { get; set; }

    /// <summary>
    /// 租户私钥
    /// <remarks></remarks>
    /// </summary>
    [SugarColumn(ColumnDescription = "租户私钥", ColumnDataType = "Nvarchar(MAX)", IsNullable = false)]
    public string PrivateKey { get; set; }

    /// <summary>
    /// 租户管理员名称
    /// </summary>
    [SugarColumn(ColumnDescription = "租户管理员名称", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string AdminName { get; set; }

    /// <summary>
    /// 租户管理员邮箱
    /// </summary>
    [SugarColumn(ColumnDescription = "租户管理员邮箱", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string Email { get; set; }

    /// <summary>
    /// 租户电话
    /// </summary>
    [SugarColumn(ColumnDescription = "租户电话", ColumnDataType = "Nvarchar(20)", IsNullable = false)]
    public string Phone { get; set; }

    /// <summary>
    /// 租户类型
    /// </summary>
    [SugarColumn(ColumnDescription = "租户类型", ColumnDataType = "tinyint", IsNullable = false)]
    public TenantTypeEnum TenantType { get; set; }

    /// <summary>
    /// LogoUrl
    /// </summary>
    [SugarColumn(ColumnDescription = "LogoUrl", ColumnDataType = "Nvarchar(max)", IsNullable = false)]
    public string LogoUrl { get; set; }

    /// <summary>
    /// App授权信息
    /// </summary>
    [Navigate(NavigateType.OneToMany, nameof(SysTenantAppInfoModel.TenantId), nameof(Id))]
    public List<SysTenantAppInfoModel> AppList { get; set; }

    /// <summary>
    /// 数据库信息
    /// </summary>
    [Navigate(NavigateType.OneToMany, nameof(SysTenantDataBaseModel.TenantId), nameof(Id))]
    public List<SysTenantDataBaseModel> DataBaseList { get; set; }

    ///// <summary>
    ///// 系统管理员用户
    ///// </summary>
    //[SugarColumn(IsIgnore = true)]
    //public TenUserModel SystemAdminUser { get; set; }

    ///// <summary>
    ///// 租户管理员用户
    ///// </summary>
    //[SugarColumn(IsIgnore = true)]
    //public TenUserModel TenantAdminUser { get; set; }
}