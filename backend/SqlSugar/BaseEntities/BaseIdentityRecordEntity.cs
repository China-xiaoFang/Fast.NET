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

using Fast.IaaS;
using Fast.SqlSugar.IBaseEntities;
using Microsoft.AspNetCore.Http;
using SqlSugar;

namespace Fast.SqlSugar.BaseEntities;

/// <summary>
/// <see cref="BaseIdentityRecordEntity"/> 自增主键记录Entity基类
/// </summary>
[SuppressSniffer]
public class BaseIdentityRecordEntity : IdentityKeyEntity, IBaseIdentityRecordEntity
{
    /// <summary>
    /// 设备
    /// </summary>
    [SugarColumn(ColumnDescription = "设备", ColumnDataType = "Nvarchar(100)", IsNullable = true, CreateTableFieldSort = 983)]
    public virtual string Device { get; set; }

    /// <summary>
    /// 操作系统（版本）
    /// </summary>
    [SugarColumn(ColumnDescription = "操作系统（版本）", ColumnDataType = "Nvarchar(100)", IsNullable = true, CreateTableFieldSort = 984)]
    public virtual string OS { get; set; }

    /// <summary>
    /// 浏览器（版本）
    /// </summary>
    [SugarColumn(ColumnDescription = "浏览器（版本）", ColumnDataType = "Nvarchar(100)", IsNullable = true, CreateTableFieldSort = 985)]
    public virtual string Browser { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    [SugarColumn(ColumnDescription = "省份", ColumnDataType = "Nvarchar(20)", IsNullable = true, CreateTableFieldSort = 986)]
    public virtual string Province { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    [SugarColumn(ColumnDescription = "城市", ColumnDataType = "Nvarchar(20)", IsNullable = true, CreateTableFieldSort = 987)]
    public virtual string City { get; set; }

    /// <summary>
    /// Ip
    /// </summary>
    [SugarColumn(ColumnDescription = "Ip", ColumnDataType = "Nvarchar(15)", IsNullable = true, CreateTableFieldSort = 988)]
    public virtual string Ip { get; set; }

    /// <summary>
    /// 部门Id
    /// </summary>
    [SugarColumn(ColumnDescription = "部门Id", IsNullable = true, CreateTableFieldSort = 989)]
    public virtual long? DepartmentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    [SugarColumn(ColumnDescription = "部门名称", ColumnDataType = "Nvarchar(20)", IsNullable = true, CreateTableFieldSort = 990)]
    public string DepartmentName { get; set; }

    /// <summary>
    /// 创建者用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "创建者用户Id", IsNullable = true, CreateTableFieldSort = 991)]
    public long? CreatedUserId { get; set; }

    /// <summary>
    /// 创建者用户名称
    /// </summary>
    [SugarColumn(ColumnDescription = "创建者用户名称", ColumnDataType = "Nvarchar(20)", IsNullable = true, CreateTableFieldSort = 992)]
    public string CreatedUserName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间", ColumnDataType = "datetimeoffset", IsNullable = true, CreateTableFieldSort = 993)]
    public DateTime? CreatedTime { get; set; }

    /// <summary>
    /// 记录表创建
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/> 请求上下文</param>
    public void RecordCreate(HttpContext httpContext)
    {
        var userAgentInfo = httpContext.RequestUserAgentInfo();
        var wanInfo = httpContext.RemoteIpv4Info();

        Device = userAgentInfo.Device;
        OS = userAgentInfo.OS;
        Browser = userAgentInfo.Browser;
        Province = wanInfo.Province;
        City = wanInfo.City;
        Ip = wanInfo.Ip;
    }
}