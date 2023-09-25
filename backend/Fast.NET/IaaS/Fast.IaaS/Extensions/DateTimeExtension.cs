﻿// Apache开源许可证
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

using System.Globalization;

namespace Fast.IaaS.Extensions;

/// <summary>
/// <see cref="DateTime"/> 拓展类
/// </summary>
public static class DateTimeExtension
{
    /// <summary>
    /// 得到问好
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns><see cref="string"/></returns>
    public static string GetSayHello(this DateTime dateTime)
    {
        var hour = DateTime.Now.Hour;
        if (hour < 6)
            return "凌晨好！";
        if (hour < 9)
            return "早上好！";
        if (hour < 12)
            return "上午好！";
        if (hour < 14)
            return "中午好！";
        if (hour < 17)
            return "下午好！";
        if (hour < 19)
            return "傍晚好！";
        if (hour < 22)
            return "晚上好！";
        return "夜里好！";
    }

    /// <summary>
    /// 获取指定年月的第一天
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <param name="year"></param>
    /// <param name="mon"></param>
    /// <returns><see cref="string"/></returns>
    public static DateTime GetCurMonthFirstDay(this DateTime dateTime, string year, string mon)
    {
        var AssemblyDate = Convert.ToDateTime(year + "-" + mon + "-" + "01"); // 组装当前指定月份
        var result = AssemblyDate.AddDays(1 - AssemblyDate.Day); // 返回指定当前月份的第一天
        return new DateTime(result.Year, result.Month, result.Day, 0, 0, 0);
    }

    /// <summary>
    /// 获取指定年月的第一天
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <param name="year"></param>
    /// <param name="mon"></param>
    /// <returns><see cref="DateTime"/></returns>
    public static DateTime GetCurMonthFirstDay(this DateTime dateTime, int year, int mon)
    {
        var AssemblyDate = Convert.ToDateTime(year + "-" + mon + "-" + "01"); // 组装当前指定月份
        var result = AssemblyDate.AddDays(1 - AssemblyDate.Day); // 返回指定当前月份的第一天
        return new DateTime(result.Year, result.Month, result.Day, 0, 0, 0);
    }

    /// <summary>
    /// 获取指定年月的最后一天
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <param name="year"></param>
    /// <param name="mon"></param>
    /// <returns><see cref="DateTime"/></returns>
    public static DateTime GetCurMonthLastDay(this DateTime dateTime, string year, string mon)
    {
        var AssemblyDate = Convert.ToDateTime(year + "-" + mon + "-" + "01"); // 组装当前指定月份
        var result = AssemblyDate.AddDays(1 - AssemblyDate.Day).AddMonths(1).AddDays(-1); // 返回指定当前月份的最后一天
        return new DateTime(result.Year, result.Month, result.Day, 23, 59, 59);
    }

    /// <summary>
    /// 获取指定年月的最后一天
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <param name="year"></param>
    /// <param name="mon"></param>
    /// <returns><see cref="DateTime"/></returns>
    public static DateTime GetCurMonthLastDay(this DateTime dateTime, int year, int mon)
    {
        var AssemblyDate = Convert.ToDateTime(year + "-" + mon + "-" + "01"); // 组装当前指定月份
        var result = AssemblyDate.AddDays(1 - AssemblyDate.Day).AddMonths(1).AddDays(-1); // 返回指定当前月份的最后一天
        return new DateTime(result.Year, result.Month, result.Day, 23, 59, 59);
    }

    /// <summary>
    /// 获取当前月的第一天
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns><see cref="DateTime"/></returns>
    public static DateTime GetCurMonthFirstDay(this DateTime dateTime)
    {
        // 第一种写法
        //DateTime CurDate =Convert.ToDateTime(DateTime.Now.ToString());  // 组装当前指定月份
        //return CurDate.AddDays(1 - CurDate.Day);  // 返回指定当前月份的第一天

        // 第二种写法
        return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0); // 该方法可以指定，年、月、日
    }

    /// <summary>
    /// 获取当前月的最后一天
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns><see cref="DateTime"/></returns>
    public static DateTime GetCurMonthLastDay(this DateTime dateTime)
    {
        var CurDate = Convert.ToDateTime(dateTime.ToString(CultureInfo.InvariantCulture)); // 组装当前指定月份
        var result = CurDate.AddDays(1 - CurDate.Day).AddMonths(1).AddDays(-1); // 返回指定当前月份的最后一天
        return new DateTime(result.Year, result.Month, result.Day, 23, 59, 59);
    }

    /// <summary>
    /// 获取上月的第一天
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns><see cref="DateTime"/></returns>
    public static DateTime GetUpMonthFirstDay(this DateTime dateTime)
    {
        var nowDate = dateTime.AddMonths(-1);
        return new DateTime(nowDate.Year, nowDate.Month, 1, 0, 0, 0); // 该方法可以指定，年、月、日
    }

    /// <summary>
    /// 获取上月的最后一天
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns><see cref="DateTime"/></returns>
    public static DateTime GetUpMonthLastDay(this DateTime dateTime)
    {
        var CurDate = Convert.ToDateTime(dateTime.ToString(CultureInfo.InvariantCulture)); // 组装当前指定月份
        var result = CurDate.AddDays(1 - CurDate.Day).AddDays(-1); // 返回指定上月份的最后一天
        return new DateTime(result.Year, result.Month, result.Day, 23, 59, 59);
    }

    /// <summary>
    /// 获取本周时间
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns>(<see cref="DateTime"/>, <see cref="DateTime"/>)</returns>
    public static (DateTime startTime, DateTime lastTime) GetCurWeekDay(this DateTime dateTime)
    {
        var startTime = dateTime.AddDays(0 - Convert.ToInt16(dateTime.DayOfWeek) + 1);
        var lastTime = dateTime.AddDays(6 - Convert.ToInt16(dateTime.DayOfWeek) + 1);
        return (new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0),
            new DateTime(lastTime.Year, lastTime.Month, lastTime.Day, 23, 59, 59));
    }

    /// <summary>
    /// 获取上周时间
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns>(<see cref="DateTime"/>, <see cref="DateTime"/>)</returns>
    public static (DateTime startTime, DateTime lastTime) GetUpWeekDay(this DateTime dateTime)
    {
        var startTime = dateTime.AddDays(0 - Convert.ToInt16(dateTime.DayOfWeek) - 6);
        var lastTime = dateTime.AddDays(6 - Convert.ToInt16(dateTime.DayOfWeek) - 6);
        return (new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0),
            new DateTime(lastTime.Year, lastTime.Month, lastTime.Day, 23, 59, 59));
    }

    /// <summary>
    /// 获取当天时间
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns>(<see cref="DateTime"/>, <see cref="DateTime"/>)</returns>
    public static (DateTime startTime, DateTime lastTime) GetCurDay(this DateTime dateTime)
    {
        return (new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0),
            new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59));
    }

    /// <summary>
    /// 获取昨天时间
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns>(<see cref="DateTime"/>, <see cref="DateTime"/>)</returns>
    public static (DateTime startTime, DateTime lastTime) GetUpDay(this DateTime dateTime)
    {
        var dt = dateTime.AddDays(-1);
        return (new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0), new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59));
    }

    /// <summary>
    /// 计算两个时间的差
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <param name="startTime"></param>
    /// <param name="lastTime"></param>
    /// <returns><see cref="int"/></returns>
    public static int DateDiff(this DateTime dateTime, DateTime startTime, DateTime lastTime)
    {
        var start = Convert.ToDateTime(startTime.ToShortDateString());
        var end = Convert.ToDateTime(lastTime.ToShortDateString());
        var sp = end.Subtract(start);
        return sp.Days;
    }

    /// <summary>
    /// 将毫秒时间戳转换为DateTime，若转换失败，则返回日期最小值。不抛出异常。  
    /// </summary>
    /// <param name="timeStamps"><see cref="long"/></param>
    /// <returns><see cref="DateTime"/></returns>
    public static DateTime ToDateTime_Milliseconds(this long timeStamps)
    {
        try
        {
            // 当地时区
            return timeStamps.IsNullOrZero() ? DateTime.MinValue : GlobalConstant.DefaultTime.AddMilliseconds(timeStamps);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// 将毫秒时间戳转换为DateTime，若转换失败，则返回默认值。
    /// </summary>
    /// <param name="timeStamps"><see cref="long"/></param>
    /// <param name="defaultValue"></param>
    /// <returns><see cref="DateTime"/></returns>
    public static DateTime ToDateTime_Milliseconds(this long timeStamps, DateTime? defaultValue)
    {
        try
        {
            // 当地时区
            return timeStamps.IsNullOrZero()
                ? defaultValue.GetValueOrDefault()
                : GlobalConstant.DefaultTime.AddMilliseconds(timeStamps);
        }
        catch
        {
            return defaultValue.GetValueOrDefault();
        }
    }

    /// <summary>
    /// 将秒时间戳转换为DateTime，若转换失败，则返回日期最小值。不抛出异常。  
    /// </summary>
    /// <param name="timeStamps"><see cref="long"/></param>
    /// <returns><see cref="DateTime"/></returns>
    public static DateTime ToDateTime_Seconds(this long timeStamps)
    {
        try
        {
            // 当地时区
            return timeStamps.IsNullOrZero() ? DateTime.MinValue : GlobalConstant.DefaultTime.AddSeconds(timeStamps);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// 将秒时间戳转换为DateTime，若转换失败，则返回默认值。
    /// </summary>
    /// <param name="timeStamps"><see cref="long"/></param>
    /// <param name="defaultValue"></param>
    /// <returns><see cref="DateTime"/></returns>
    public static DateTime ToDateTime_Seconds(this long timeStamps, DateTime? defaultValue)
    {
        try
        {
            // 当地时区
            return timeStamps.IsNullOrZero()
                ? defaultValue.GetValueOrDefault()
                : GlobalConstant.DefaultTime.AddSeconds(timeStamps);
        }
        catch
        {
            return defaultValue.GetValueOrDefault();
        }
    }

    /// <summary>
    /// 获取生肖
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns><see cref="string"/></returns>
    public static string GetZodiac(this DateTime dateTime)
    {
        if (dateTime < new DateTime(1900, 1, 1))
        {
            return "";
        }

        var calendar = new ChineseLunisolarCalendar();

        const string zodiac = "鼠牛虎兔龙蛇马羊猴鸡狗猪";

        var year = calendar.GetSexagenaryYear(dateTime);

        return zodiac.Substring(calendar.GetTerrestrialBranch(year) - 1, 1);
    }

    /// <summary>
    /// 获取星座
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns><see cref="string"/></returns>
    public static string GetConstellation(this DateTime dateTime)
    {
        if (dateTime < new DateTime(1900, 1, 1))
        {
            return "";
        }

        var monthDay = dateTime.ToString("MMdd");

        if (dateTime.Month == 1 && dateTime.Day < 20)
        {
            monthDay = "13" + dateTime.Day.ToString("00");
        }

        string[] atomBound =
        {
            "0120", "0219", "0321", "0420", "0521", "0622", "0723", "0823", "0923", "1024", "1123", "1222", "1320"
        };
        string[] atoms = {"水瓶座", "双鱼座", "白羊座", "金牛座", "双子座", "巨蟹座", "狮子座", "处女座", "天秤座", "天蝎座", "射手座", "魔羯座"};

        var result = "未知";

        for (var i = 0; i < atomBound.Length - 1; i++)
        {
            if (string.Compare(atomBound[i], monthDay, StringComparison.Ordinal) > 1 ||
                string.Compare(atomBound[i + 1], monthDay, StringComparison.Ordinal) <= 0)
                continue;
            result = atoms[i];
            break;
        }

        return result;
    }

    /// <summary>
    /// 生命密码
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns><see cref="int"/></returns>
    public static int GetLifeCode(this DateTime dateTime)
    {
        if (dateTime < new DateTime(1900, 1, 1))
        {
            return -1;
        }

        var lifeNum = GetSum(dateTime.Year) + GetSum(dateTime.Month) + GetSum(dateTime.Day);

        while (lifeNum > 9)
        {
            lifeNum = GetSum(lifeNum);
        }

        return lifeNum;
    }

    private static int GetSum(int num)
    {
        var b = (num.ToString()).ToCharArray();
        return b.Sum(t => Convert.ToInt32(t.ToString()));
    }
}