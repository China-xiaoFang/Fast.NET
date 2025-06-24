// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using System;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="DateTimeUtil"/> DateTime工具类
/// </summary>
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
        return internalDate.AddMonths(+1)
            .AddDays(-1);
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
        return internalDate.AddMonths(+1)
            .AddDays(-1);
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