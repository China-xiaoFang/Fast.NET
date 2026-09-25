// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using System.ComponentModel.DataAnnotations;

namespace Fast.SqlSugar;

/// <summary>
/// SqlSugar 统一分页搜索输入
/// </summary>
[SuppressSniffer]
public class PagedSearchInput
{
    /// <summary>
    /// 搜索字段英文
    /// </summary>
    /// <remarks>主要字段，用于生成查询语句</remarks>
    [StringRequired(ErrorMessage = "搜索字段不能为空")]
    public virtual string EnField { get; set; }

    /// <summary>
    /// 搜索字段中文
    /// </summary>
    /// <remarks>次要字段，用于提示</remarks>
    public virtual string ChField { get; set; }

    /// <summary>
    /// 搜索值
    /// </summary>
    public virtual string Value { get; set; }

    /// <summary>
    /// 搜索类型
    /// </summary>
    /// <remarks>默认模糊匹配</remarks>
    public virtual PagedSearchTypeEnum Type { get; set; } = PagedSearchTypeEnum.Like;
}
