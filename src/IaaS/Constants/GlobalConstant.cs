// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System;

namespace Fast.IaaS;

/// <summary>
/// 常用常量
/// </summary>
public static class GlobalConstant
{
    /// <summary>
    /// 默认 DateTime
    /// </summary>
    public static DateTime DefaultTime => TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);

    /// <summary>
    /// 时间戳
    /// </summary>
    //public static long TimeStamp => Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds)
    public static long TimeStamp => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    /// <summary>
    /// SQL Server 最小时间
    /// </summary>
    public static DateTime SqlServerMinTime => new(1753, 01, 01, 00, 00, 00);

    /// <summary>
    /// SQL Server 最大时间
    /// </summary>
    public static DateTime SqlServerMaxTime => new(9999, 12, 31, 23, 59, 59);

    /// <summary>
    /// 特殊分割符号
    /// </summary>
    public static char SpecialSplitSymbol => '│';

    /// <summary>
    /// 特殊标记符号
    /// </summary>
    public static char SpecialMarkSymbol => '×';
}
