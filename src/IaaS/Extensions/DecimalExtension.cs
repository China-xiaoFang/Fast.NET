// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System;

namespace Fast.IaaS;

/// <summary>
/// 为 <see cref="decimal"/> 提供扩展方法
/// </summary>
public static class DecimalExtension
{
    /// <summary>
    /// 得到百分比
    /// </summary>
    /// <param name="data">要处理或传输的数据</param>
    /// <returns>得到百分比</returns>
    public static string GetPercentage(this decimal data)
    {
        decimal result = data * 100;
        return result == 100 ? "100%" : $"{Math.Round(result, 2)}%";
    }

    /// <summary>
    /// 得到百分比
    /// </summary>
    /// <param name="num1">参与计算的第一个数值</param>
    /// <param name="num2">参与计算的第二个数值</param>
    /// <returns>得到百分比</returns>
    public static string GetPercentage(this decimal num1, decimal num2)
    {
        decimal result = num2 == 0 ? 0 : Math.Round(num1 / num2, 4);
        return result.GetPercentage();
    }

    /// <summary>
    /// 获取 decimal，小数点后面有几位就保留几位
    /// </summary>
    /// <param name="data">要处理或传输的数据</param>
    /// <param name="places">保留的小数位数</param>
    /// <returns>获取到的 decimal，小数点后面有几位就保留几位</returns>
    public static decimal GetDecimal(this decimal data, int? places = null)
    {
        if (places == null)
        {
            return (decimal)(double)data;
        }

        return decimal.Round(data, places.Value);
    }
}
