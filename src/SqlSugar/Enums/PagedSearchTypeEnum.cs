// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System.ComponentModel;

namespace Fast.SqlSugar;

/// <summary>
/// 分页搜索类型枚举
/// </summary>
[FastEnum("分页搜索类型枚举")]
public enum PagedSearchTypeEnum : byte
{
    /// <summary>
    /// 模糊匹配
    /// </summary>
    [Description("模糊匹配")]
    Like = 1,

    /// <summary>
    /// 等于
    /// </summary>
    [Description("等于")]
    Equal = 2,

    /// <summary>
    /// 不等于
    /// </summary>
    [Description("不等于")]
    NotEqual = 3,

    /// <summary>
    /// 大于
    /// </summary>
    [Description("大于")]
    GreaterThan = 4,

    /// <summary>
    /// 大于等于
    /// </summary>
    [Description("大于等于")]
    GreaterThanOrEqual = 5,

    /// <summary>
    /// 小于
    /// </summary>
    [Description("小于")]
    LessThan = 6,

    /// <summary>
    /// 小于等于
    /// </summary>
    [Description("小于等于")]
    LessThanOrEqual = 7,

    /// <summary>
    /// 包含
    /// </summary>
    [Description("包含")]
    Include = 8,

    /// <summary>
    /// 排除
    /// </summary>
    [Description("排除")]
    NotInclude = 9
}
