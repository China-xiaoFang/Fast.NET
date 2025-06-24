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
using System.Globalization;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

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
        var hour = dateTime.Hour;
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
    /// 获取当前月的第一天
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns><see cref="DateTime"/></returns>
    public static DateTime GetCurMonthFirstDay(this DateTime dateTime)
    {
        return DateTimeUtil.GetYearMonthFirstDay(dateTime.Year, dateTime.Month);
    }

    /// <summary>
    /// 获取当前月的最后一天
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns><see cref="DateTime"/></returns>
    public static DateTime GetCurMonthLastDay(this DateTime dateTime)
    {
        return DateTimeUtil.GetYearMonthLastDay(dateTime.Year, dateTime.Month);
    }

    /// <summary>
    /// 获取上月的第一天
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns><see cref="DateTime"/></returns>
    public static DateTime GetUpMonthFirstDay(this DateTime dateTime)
    {
        var nowDate = dateTime.AddMonths(-1);
        return new DateTime(nowDate.Year, nowDate.Month, 01, 00, 00, 00); // 该方法可以指定，年、月、日
    }

    /// <summary>
    /// 获取上月的最后一天
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns><see cref="DateTime"/></returns>
    public static DateTime GetUpMonthLastDay(this DateTime dateTime)
    {
        var internalDate = new DateTime(dateTime.Year, dateTime.Month, 01, 23, 59, 59);
        return internalDate.AddMonths(-1);
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
        var internalDate = dateTime.AddDays(-1);
        return (new DateTime(internalDate.Year, internalDate.Month, internalDate.Day, 0, 0, 0),
            new DateTime(internalDate.Year, internalDate.Month, internalDate.Day, 23, 59, 59));
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
            if (string.Compare(atomBound[i], monthDay, StringComparison.Ordinal) > 1
                || string.Compare(atomBound[i + 1], monthDay, StringComparison.Ordinal) <= 0)
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
        var b = num.ToString()
            .ToCharArray();
        return b.Sum(t => Convert.ToInt32(t.ToString()));
    }
}