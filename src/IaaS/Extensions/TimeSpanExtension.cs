// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System;

namespace Fast.IaaS;

/// <summary>
/// 为 <see cref="TimeSpan"/> 提供扩展方法
/// </summary>
public static class TimeSpanExtension
{
    /// <summary>
    /// 获取描述
    /// </summary>
    /// <param name="timeSpan">要处理的时间间隔</param>
    /// <returns>获取到的描述</returns>
    public static string ToDescription(this TimeSpan timeSpan)
    {
        string formatStr = "ss\\秒";
        if (timeSpan.TotalDays >= 1)
        {
            formatStr = "dd\\天hh\\时mm\\分" + formatStr;
        }
        else if (timeSpan.TotalHours >= 1)
        {
            formatStr = "hh\\时mm\\分" + formatStr;
        }
        else if (timeSpan.TotalMinutes >= 1)
        {
            formatStr = "mm\\分" + formatStr;
        }

        if (timeSpan < TimeSpan.Zero)
        {
            // 负数
            return $"-{timeSpan.ToString(formatStr)}";
        }

        return timeSpan.ToString(formatStr);
    }
}
