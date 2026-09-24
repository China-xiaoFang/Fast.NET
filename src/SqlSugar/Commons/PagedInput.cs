// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

namespace Fast.SqlSugar;

/// <summary>
/// SqlSugar 统一分页输入
/// </summary>
[SuppressSniffer]
public class PagedInput
{
    /// <summary>
    /// 当前页面索引值，默认为 1
    /// </summary>
    public virtual int PageIndex { get; set; } = 1;

    /// <summary>
    /// 页码容量
    /// </summary>
    public virtual int PageSize { get; set; } = 20;

    /// <summary>
    /// 搜索值
    /// </summary>
    public virtual string SearchValue { get; set; }

    /// <summary>
    /// 搜索时间
    /// </summary>
    public virtual IList<DateTime?> SearchTimeList { get; set; } = [];

    /// <summary>
    /// 搜索集合
    /// </summary>
    public virtual PagedSearchInput[] SearchList { get; set; } = [];

    /// <summary>
    /// 排序集合
    /// </summary>
    public virtual PagedSortInput[] SortList { get; set; } = [];

    /// <summary>
    /// 启用分页
    /// </summary>
    /// <remarks>默认启用</remarks>
    public virtual bool EnablePaged { get; set; } = true;

    /// <summary>
    /// 是否可以使用默认排序
    /// </summary>
    public bool IsOrderBy => SortList is not {Length: > 0};
}
