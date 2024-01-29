﻿// Apache开源许可证
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
using Microsoft.AspNetCore.Http;

namespace Fast.SqlSugar.IBaseEntities;

/// <summary>
/// <see cref="IBaseIdentityRecordEntity"/> 自增主键记录Entity基类接口
/// </summary>
[SuppressSniffer]
public interface IBaseIdentityRecordEntity : IDatabaseEntity
{
    /// <summary>
    /// 设备
    /// </summary>
    string Device { get; set; }

    /// <summary>
    /// 操作系统（版本）
    /// </summary>
    string OS { get; set; }

    /// <summary>
    /// 浏览器（版本）
    /// </summary>
    string Browser { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    string Province { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    string City { get; set; }

    /// <summary>
    /// Ip
    /// </summary>
    string Ip { get; set; }

    /// <summary>
    /// 部门Id
    /// </summary>
    long? DepartmentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    string DepartmentName { get; set; }

    /// <summary>
    /// 创建者用户Id
    /// </summary>
    long? CreatedUserId { get; set; }

    /// <summary>
    /// 创建者用户名称
    /// </summary>
    string CreatedUserName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    DateTime? CreatedTime { get; set; }

    /// <summary>
    /// 记录表创建
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/> 请求上下文</param>
    void RecordCreate(HttpContext httpContext);
}