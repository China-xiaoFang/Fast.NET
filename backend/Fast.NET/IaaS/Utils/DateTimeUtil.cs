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
/// <see cref="DateTimeUtil"/> DateTime工具类
/// </summary>
[SuppressSniffer]
public static class DateTimeUtil
{
    /// <summary>
    /// 获取指定年月的第一天
    /// </summary>
    /// <param name="year"><see cref="string"/> 年份</param>
    /// <param name="month"><see cref="string"/> 月份</param>
    /// <returns><see cref="DateTime"/> 第一天的 DateTime</returns>
    public static DateTime GetYearMonthFirstDay(string year, string month)
    {
        // 组装当前指定月份，默认为：yyyy-MM-01 00:00:00
        var internalDate = Convert.ToDateTime($"{year}-{month}-01 00:00:00");
        return internalDate;
    }

    /// <summary>
    /// 获取指定年月的第一天
    /// </summary>
    /// <param name="year"><see cref="int"/> 年份</param>
    /// <param name="month"><see cref="int"/> 月份</param>
    /// <returns><see cref="DateTime"/> 第一天的 DateTime</returns>
    public static DateTime GetYearMonthFirstDay(int year, int month)
    {
        // 组装当前指定月份，默认为：yyyy-MM-01 00:00:00
        var internalDate = new DateTime(year, month, 01, 00, 00, 00);
        return internalDate;
    }

    /// <summary>
    /// 获取指定年月的最后一天
    /// </summary>
    /// <param name="year"><see cref="string"/> 年份</param>
    /// <param name="month"><see cref="string"/> 月份</param>
    /// <returns><see cref="DateTime"/> 最后一天的 DateTime</returns>
    public static DateTime GetYearMonthLastDay(string year, string month)
    {
        // 组装当前指定月份，默认为：yyyy-MM-01 23:59:59
        var internalDate = Convert.ToDateTime($"{year}-{month}-01 23:59:59");
        return internalDate.AddMonths(+1).AddDays(-1);
    }

    /// <summary>
    /// 获取指定年月的最后一天
    /// </summary>
    /// <param name="year"><see cref="int"/> 年份</param>
    /// <param name="month"><see cref="int"/> 月份</param>
    /// <returns><see cref="DateTime"/> 最后一天的 DateTime</returns>
    public static DateTime GetYearMonthLastDay(int year, int month)
    {
        // 组装当前指定月份，默认为：yyyy-MM-01 23:59:59
        var internalDate = new DateTime(year, month, 01, 23, 59, 59);
        return internalDate.AddMonths(+1).AddDays(-1);
    }

    /// <summary>
    /// 计算两个时间的差，返回天数
    /// </summary>
    /// <param name="startTime"><see cref="DateTime"/> 开始时间</param>
    /// <param name="lastTime"><see cref="DateTime"/> 结束时间</param>
    /// <returns><see cref="int"/><see cref="int"/> 天数</returns>
    public static int DateDiffDay(DateTime startTime, DateTime lastTime)
    {
        var start = Convert.ToDateTime(startTime.ToShortDateString());
        var end = Convert.ToDateTime(lastTime.ToShortDateString());
        var sp = end.Subtract(start);
        return sp.Days;
    }
}