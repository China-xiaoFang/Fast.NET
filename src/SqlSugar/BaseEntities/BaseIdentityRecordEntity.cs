// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using Fast.Runtime;
using Microsoft.AspNetCore.Http;
using SqlSugar;

// ReSharper disable once CheckNamespace
namespace Fast.SqlSugar;

/// <summary>
/// <see cref="BaseIdentityRecordEntity"/> 自增主键记录Entity基类
/// </summary>
[SuppressSniffer]
public class BaseIdentityRecordEntity : IdentityKeyEntity, IBaseIdentityRecordEntity
{
    /// <summary>
    /// 设备
    /// </summary>
    [SugarSearchValue,
     SugarColumn(ColumnDescription = "设备", ColumnDataType = "Nvarchar(100)", IsNullable = true, CreateTableFieldSort = 983)]
    public virtual string Device { get; set; }

    /// <summary>
    /// 操作系统（版本）
    /// </summary>
    [SugarSearchValue,
     SugarColumn(ColumnDescription = "操作系统（版本）", ColumnDataType = "Nvarchar(100)", IsNullable = true, CreateTableFieldSort = 984)]
    public virtual string OS { get; set; }

    /// <summary>
    /// 浏览器（版本）
    /// </summary>
    [SugarSearchValue,
     SugarColumn(ColumnDescription = "浏览器（版本）", ColumnDataType = "Nvarchar(100)", IsNullable = true, CreateTableFieldSort = 985)]
    public virtual string Browser { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    [SugarSearchValue,
     SugarColumn(ColumnDescription = "省份", ColumnDataType = "Nvarchar(20)", IsNullable = true, CreateTableFieldSort = 986)]
    public virtual string Province { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    [SugarSearchValue,
     SugarColumn(ColumnDescription = "城市", ColumnDataType = "Nvarchar(20)", IsNullable = true, CreateTableFieldSort = 987)]
    public virtual string City { get; set; }

    /// <summary>
    /// Ip
    /// </summary>
    [SugarSearchValue,
     SugarColumn(ColumnDescription = "Ip", ColumnDataType = "Nvarchar(15)", IsNullable = true, CreateTableFieldSort = 988)]
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
    [SugarSearchTime,
     SugarColumn(ColumnDescription = "创建时间", ColumnDataType = "datetimeoffset", IsNullable = true, CreateTableFieldSort = 993)]
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