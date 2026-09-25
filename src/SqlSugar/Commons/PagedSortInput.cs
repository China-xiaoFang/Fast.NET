// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using System.ComponentModel.DataAnnotations;

namespace Fast.SqlSugar;

/// <summary>
/// SqlSugar 统一分页排序输入
/// </summary>
[SuppressSniffer]
public class PagedSortInput
{
    /// <summary>
    /// 排序字段英文
    /// </summary>
    /// <remarks>主要字段，用于生成排序语句</remarks>
    [StringRequired(ErrorMessage = "排序字段不能为空")]
    public virtual string EnField { get; set; }

    /// <summary>
    /// 排序字段中文
    /// </summary>
    /// <remarks>次要字段，用于提示</remarks>
    public virtual string ChField { get; set; }

    /// <summary>
    /// 排序方式
    /// </summary>
    /// <remarks>
    /// <para>ElementPlus 的 Table 排序方式</para>
    /// <para>ascending 正序；descending：倒序；为空默认正序</para>
    /// </remarks>
    public virtual string Mode { get; set; }

    /// <summary>
    /// 是否倒序排序
    /// </summary>
    public virtual bool IsDescending =>
        !string.IsNullOrEmpty(Mode) && Mode.Equals("descending", StringComparison.InvariantCultureIgnoreCase);
}
