// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

namespace Fast.SqlSugar;

/// <summary>
/// SqlSugar DB 类型
/// </summary>
/// <remarks>放入 Class 头部，支持传入 Object，然后在 <see cref="ISqlSugarEntityHandler"/> 自行解析</remarks>
[SuppressSniffer]
[AttributeUsage(AttributeTargets.Class)]
public class SugarDbTypeAttribute : Attribute
{
    /// <summary>
    /// Entity 的 DB 类型
    /// </summary>
    /// <remarks>如果为<see langword="null"/>，则代表的默认库</remarks>
    public object Type { get; set; }

    /// <summary>
    /// SqlSugar DB 类型
    /// </summary>
    /// <remarks>放入 Class 头部，支持传入 Object，然后在 <see cref="ISqlSugarEntityHandler"/> 自行解析</remarks>
    public SugarDbTypeAttribute()
    {
        Type = null;
    }

    /// <summary>
    /// SqlSugar DB 类型
    /// </summary>
    /// <remarks>放入 Class 头部，支持传入 Object，然后在 <see cref="ISqlSugarEntityHandler"/> 自行解析</remarks>
    /// <param name="type">Entity 的 DB 类型</param>
    public SugarDbTypeAttribute(object type)
    {
        Type = type;
    }
}
