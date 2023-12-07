// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
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

// ReSharper disable once CheckNamespace

namespace Fast.IaaS;

/// <summary>
/// <see cref="decimal"/> 拓展类
/// </summary>
[SuppressSniffer]
public static class DecimalExtension
{
    /// <summary>
    /// 得到百分比
    /// </summary>
    /// <param name="data"><see cref="decimal"/></param>
    /// <returns><see cref="string"/></returns>
    public static string GetPercentage(this decimal data)
    {
        var result = data * 100;
        return result == 100 ? "100%" : $"{Math.Round(result, 2)}%";
    }

    /// <summary>
    /// 得到百分比
    /// </summary>
    /// <param name="num1"><see cref="decimal"/></param>
    /// <param name="num2"><see cref="decimal"/></param>
    /// <returns><see cref="string"/></returns>
    public static string GetPercentage(this decimal num1, decimal num2)
    {
        var result = num2 == 0 ? 0 : Math.Round(num1 / num2, 4);
        return result.GetPercentage();
    }

    /// <summary>
    /// 获取 decimal，小数点后面有几位就保留几位
    /// </summary>
    /// <param name="data"><see cref="decimal"/></param>
    /// <param name="places"><see cref="int"/>要保留的小数据，不传默认有几位就保留几位</param>
    /// <returns><see cref="decimal"/></returns>
    public static decimal GetDecimal(this decimal data, int? places = null)
    {
        if (places == null)
        {
            return (decimal) (double) data;
        }

        return decimal.Round(data, places.Value);
    }
}