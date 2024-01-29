// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
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

using System.Collections;
using System.Globalization;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="Convert"/> 转换拓展类
/// </summary>
[SuppressSniffer]
public static class ConvertExtension
{
    #region 转换为long

    /// <summary>
    /// 将 String 类型 转换为 Long 类型
    /// </summary>
    /// <param name="value"><see cref="string"/>值</param>
    /// <param name="isThrow"><see cref="bool"/>是否抛出异常，默认为 true</param>
    /// <param name="defaultValue"><see cref="long"/>异常默认值，默认为 0L</param>
    /// <returns><see cref="long"/></returns>
    /// <exception cref="ArgumentNullException">传入的值为空或者空字符串</exception>
    public static long ParseToLong(this string value, bool isThrow = true, long defaultValue = 0L)
    {
        if (isThrow)
        {
            if (value.IsEmpty())
            {
                throw new ArgumentNullException(nameof(value), "传入的值为空或者空字符串");
            }

            return long.Parse(value);
        }

        if (value.IsEmpty())
        {
            return defaultValue;
        }

        if (long.TryParse(value, out var result))
        {
            return result;
        }

        return defaultValue;
    }

    /// <summary>
    /// 将 Enum 类型 转换为 Long 类型
    /// </summary>
    /// <param name="value"><see cref="string"/>枚举值</param>
    /// <param name="isThrow"><see cref="bool"/>是否抛出异常，默认为 true</param>
    /// <param name="defaultValue"><see cref="long"/>异常默认值，默认为 0L</param>
    /// <returns><see cref="long"/></returns>
    /// <exception cref="ArgumentNullException">传入的枚举值为空</exception>
    public static long ParseToLong<TEnum>(this TEnum value, bool isThrow = true, long defaultValue = 0L)
        where TEnum : struct, Enum
    {
        if (isThrow)
        {
            if (value.IsEmpty())
            {
                throw new ArgumentNullException(nameof(value), "传入的枚举值为空");
            }

            return Convert.ToInt64(value);
        }

        if (value.IsEmpty())
        {
            return defaultValue;
        }

        return Convert.ToInt64(value);
    }

    /// <summary>
    /// 将 可空的Enum 类型 转换为 Long 类型
    /// </summary>
    /// <param name="value"><see cref="string"/>枚举值</param>
    /// <param name="isThrow"><see cref="bool"/>是否抛出异常，默认为 true</param>
    /// <param name="defaultValue"><see cref="long"/>异常默认值，默认为 0L</param>
    /// <returns><see cref="long"/></returns>
    /// <exception cref="ArgumentNullException">传入的枚举值为空</exception>
    public static long ParseToLong<TEnum>(this TEnum? value, bool isThrow = true, long defaultValue = 0L)
        where TEnum : struct, Enum
    {
        if (isThrow)
        {
            if (value.IsEmpty())
            {
                throw new ArgumentNullException(nameof(value), "传入的枚举值为空");
            }

            return Convert.ToInt64(value);
        }

        if (value.IsEmpty())
        {
            return defaultValue;
        }

        return Convert.ToInt64(value);
    }

    #endregion

    #region 转换为int

    /// <summary>
    /// 将 String 类型 转换为 Int 类型
    /// </summary>
    /// <param name="value"><see cref="string"/>值</param>
    /// <param name="isThrow"><see cref="bool"/>是否抛出异常，默认为 true</param>
    /// <param name="defaultValue"><see cref="int"/>异常默认值，默认为 0</param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="ArgumentNullException">传入的值为空或者空字符串</exception>
    public static int ParseToInt(this string value, bool isThrow = true, int defaultValue = 0)
    {
        if (isThrow)
        {
            if (value.IsEmpty())
            {
                throw new ArgumentNullException(nameof(value), "传入的值为空或者空字符串");
            }

            return int.Parse(value);
        }

        if (value.IsEmpty())
        {
            return defaultValue;
        }

        if (int.TryParse(value, out var result))
        {
            return result;
        }

        return defaultValue;
    }

    /// <summary>
    /// 将 Enum 类型 转换为 Int 类型
    /// </summary>
    /// <param name="value"><see cref="string"/>枚举值</param>
    /// <param name="isThrow"><see cref="bool"/>是否抛出异常，默认为 true</param>
    /// <param name="defaultValue"><see cref="int"/>异常默认值，默认为 0</param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="ArgumentNullException">传入的枚举值为空</exception>
    public static int ParseToInt<TEnum>(this TEnum value, bool isThrow = true, int defaultValue = 0) where TEnum : struct, Enum
    {
        if (isThrow)
        {
            if (value.IsEmpty())
            {
                throw new ArgumentNullException(nameof(value), "传入的枚举值为空");
            }

            return Convert.ToInt32(value);
        }

        if (value.IsEmpty())
        {
            return defaultValue;
        }

        return Convert.ToInt32(value);
    }

    /// <summary>
    /// 将 可空的Enum 类型 转换为 Int 类型
    /// </summary>
    /// <param name="value"><see cref="string"/>枚举值</param>
    /// <param name="isThrow"><see cref="bool"/>是否抛出异常，默认为 true</param>
    /// <param name="defaultValue"><see cref="int"/>异常默认值，默认为 0</param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="ArgumentNullException">传入的枚举值为空</exception>
    public static int ParseToInt<TEnum>(this TEnum? value, bool isThrow = true, int defaultValue = 0) where TEnum : struct, Enum
    {
        if (isThrow)
        {
            if (value.IsEmpty())
            {
                throw new ArgumentNullException(nameof(value), "传入的枚举值为空");
            }

            return Convert.ToInt32(value);
        }

        if (value.IsEmpty())
        {
            return defaultValue;
        }

        return Convert.ToInt32(value);
    }

    #endregion

    #region 转换为short

    /// <summary>
    /// 将 String 类型 转换为 Short 类型
    /// </summary>
    /// <param name="value"><see cref="string"/>值</param>
    /// <param name="isThrow"><see cref="bool"/>是否抛出异常，默认为 true</param>
    /// <param name="defaultValue"><see cref="short"/>异常默认值，默认为 0</param>
    /// <returns><see cref="short"/></returns>
    /// <exception cref="ArgumentNullException">传入的值为空或者空字符串</exception>
    public static short ParseToShort(this string value, bool isThrow = true, short defaultValue = 0)
    {
        if (isThrow)
        {
            if (value.IsEmpty())
            {
                throw new ArgumentNullException(nameof(value), "传入的值为空或者空字符串");
            }

            return short.Parse(value);
        }

        if (value.IsEmpty())
        {
            return defaultValue;
        }

        if (short.TryParse(value, out var result))
        {
            return result;
        }

        return defaultValue;
    }

    #endregion

    #region 转换为decimal

    /// <summary>
    /// 将 String 类型 转换为 Decimal 类型
    /// </summary>
    /// <param name="value"><see cref="string"/>值</param>
    /// <param name="isThrow"><see cref="bool"/>是否抛出异常，默认为 true</param>
    /// <param name="defaultValue"><see cref="decimal"/>异常默认值，默认为 0M</param>
    /// <returns><see cref="decimal"/></returns>
    /// <exception cref="ArgumentNullException">传入的值为空或者空字符串</exception>
    public static decimal ParseToDecimal(this string value, bool isThrow = true, decimal defaultValue = 0M)
    {
        if (isThrow)
        {
            if (value.IsEmpty())
            {
                throw new ArgumentNullException(nameof(value), "传入的值为空或者空字符串");
            }

            return decimal.Parse(value);
        }

        if (value.IsEmpty())
        {
            return defaultValue;
        }

        if (decimal.TryParse(value, out var result))
        {
            return result;
        }

        return defaultValue;
    }

    #endregion

    #region 转化为bool

    /// <summary>
    /// 将 String 类型 转换为 Bool 类型
    /// </summary>
    /// <param name="value"><see cref="string"/>值</param>
    /// <param name="isThrow"><see cref="bool"/>是否抛出异常，默认为 true</param>
    /// <param name="defaultValue"><see cref="bool"/>异常默认值，默认为 false</param>
    /// <returns><see cref="bool"/></returns>
    /// <exception cref="ArgumentNullException">传入的值为空或者空字符串</exception>
    public static bool ParseToBool(this string value, bool isThrow = true, bool defaultValue = false)
    {
        if (isThrow)
        {
            if (value.IsEmpty())
            {
                throw new ArgumentNullException(nameof(value), "传入的值为空或者空字符串");
            }

            return bool.Parse(value);
        }

        if (value.IsEmpty())
        {
            return defaultValue;
        }

        if (bool.TryParse(value, out var result))
        {
            return result;
        }

        return defaultValue;
    }

    #endregion

    #region 转换为float

    /// <summary>
    /// 将 String 类型 转换为 Float 类型
    /// </summary>
    /// <param name="value"><see cref="string"/>值</param>
    /// <param name="isThrow"><see cref="bool"/>是否抛出异常，默认为 true</param>
    /// <param name="defaultValue"><see cref="float"/>异常默认值，默认为 0F</param>
    /// <returns><see cref="float"/></returns>
    /// <exception cref="ArgumentNullException">传入的值为空或者空字符串</exception>
    public static float ParseToFloat(this string value, bool isThrow = true, float defaultValue = 0F)
    {
        if (isThrow)
        {
            if (value.IsEmpty())
            {
                throw new ArgumentNullException(nameof(value), "传入的值为空或者空字符串");
            }

            return float.Parse(value);
        }

        if (value.IsEmpty())
        {
            return defaultValue;
        }

        if (float.TryParse(value, out var result))
        {
            return result;
        }

        return defaultValue;
    }

    #endregion

    #region 转换为double

    /// <summary>
    /// 将 String 类型 转换为 Float 类型
    /// </summary>
    /// <param name="value"><see cref="string"/>值</param>
    /// <param name="isThrow"><see cref="bool"/>是否抛出异常，默认为 true</param>
    /// <param name="defaultValue"><see cref="double"/>异常默认值，默认为 0D</param>
    /// <returns><see cref="double"/></returns>
    /// <exception cref="ArgumentNullException">传入的值为空或者空字符串</exception>
    public static double ParseToDouble(this string value, bool isThrow = true, double defaultValue = 0D)
    {
        if (isThrow)
        {
            if (value.IsEmpty())
            {
                throw new ArgumentNullException(nameof(value), "传入的值为空或者空字符串");
            }

            return double.Parse(value);
        }

        if (value.IsEmpty())
        {
            return defaultValue;
        }

        if (double.TryParse(value, out var result))
        {
            return result;
        }

        return defaultValue;
    }

    #endregion

    #region 转换为Guid

    /// <summary>
    /// 将 String 类型 转换为 Guid 类型
    /// </summary>
    /// <param name="value"><see cref="string"/>值</param>
    /// <param name="isThrow"><see cref="bool"/>是否抛出异常，默认为 true</param>
    /// <param name="defaultValue"><see cref="Guid"/>异常默认值，默认为 Guid.Empty</param>
    /// <returns><see cref="Guid"/></returns>
    /// <exception cref="ArgumentNullException">传入的值为空或者空字符串</exception>
    public static Guid ParseToGuid(this string value, bool isThrow = true, Guid? defaultValue = null)
    {
        if (isThrow)
        {
            if (value.IsEmpty())
            {
                throw new ArgumentNullException(nameof(value), "传入的值为空或者空字符串");
            }

            return Guid.Parse(value);
        }

        if (value.IsEmpty())
        {
            if (defaultValue == null)
            {
                return Guid.Empty;
            }

            return defaultValue.Value;
        }

        if (Guid.TryParse(value, out var result))
        {
            return result;
        }

        if (defaultValue == null)
        {
            return Guid.Empty;
        }

        return defaultValue.Value;
    }

    #endregion

    #region 转换为DateTime

    /// <summary>
    /// 将 String 类型 转换为 DateTime 类型
    /// </summary>
    /// <param name="value"><see cref="string"/>值</param>
    /// <param name="isThrow"><see cref="bool"/>是否抛出异常，默认为 true</param>
    /// <param name="defaultValue"><see cref="DateTime"/>异常默认值，默认为 DateTime.MinValue</param>
    /// <returns><see cref="DateTime"/></returns>
    /// <exception cref="ArgumentNullException">传入的值为空或者空字符串</exception>
    public static DateTime ParseToDateTime(this string value, bool isThrow = true, DateTime defaultValue = default)
    {
        if (isThrow)
        {
            if (value.IsEmpty())
            {
                throw new ArgumentNullException(nameof(value), "传入的值为空或者空字符串");
            }

            if (value.Contains("-") || value.Contains("/") || value.Contains(":"))
            {
                return DateTime.Parse(value);
            }

            switch (value.Length)
            {
                case 4:
                {
                    var result = DateTime.ParseExact(value, "yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, 1, 1, 0, 0, 0);
                    return result;
                }
                case 6:
                {
                    var result = DateTime.ParseExact(value, "yyyyMM", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, 1, 0, 0, 0);
                    return result;
                }
                case 8:
                {
                    var result = DateTime.ParseExact(value, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, result.Day, 0, 0, 0);
                    return result;
                }
                case 10:
                {
                    var result = DateTime.ParseExact(value, "yyyyMMddHH", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, result.Day, result.Hour, 0, 0);
                    return result;
                }
                case 12:
                {
                    var result = DateTime.ParseExact(value, "yyyyMMddHHmm", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, result.Day, result.Hour, result.Minute, 0);
                    return result;
                }
                default:
                {
                    var result = DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    return result;
                }
            }
        }

        if (value.IsEmpty())
        {
            return defaultValue;
        }

        if (value.Contains("-") || value.Contains("/") || value.Contains(":"))
        {
            if (DateTime.TryParse(value, out var result))
            {
                return result;
            }
        }

        switch (value.Length)
        {
            case 4:
            {
                if (DateTime.TryParseExact(value, "yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out var result))
                {
                    result = new DateTime(result.Year, 1, 1, 0, 0, 0);
                    return result;
                }
            }

                break;
            case 6:
            {
                if (DateTime.TryParseExact(value, "yyyyMM", CultureInfo.CurrentCulture, DateTimeStyles.None, out var result))
                {
                    result = new DateTime(result.Year, result.Month, 1, 0, 0, 0);
                    return result;
                }
            }

                break;
            case 8:
            {
                if (DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out var result))
                {
                    result = new DateTime(result.Year, result.Month, result.Day, 0, 0, 0);
                    return result;
                }
            }

                break;
            case 10:
            {
                if (DateTime.TryParseExact(value, "yyyyMMddHH", CultureInfo.CurrentCulture, DateTimeStyles.None, out var result))
                {
                    result = new DateTime(result.Year, result.Month, result.Day, result.Hour, 0, 0);
                    return result;
                }
            }

                break;
            case 12:
            {
                if (DateTime.TryParseExact(value, "yyyyMMddHHmm", CultureInfo.CurrentCulture, DateTimeStyles.None,
                        out var result))
                {
                    result = new DateTime(result.Year, result.Month, result.Day, result.Hour, result.Minute, 0);
                    return result;
                }
            }

                break;
            default:
            {
                if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None,
                        out var result))
                {
                    return result;
                }
            }
                break;
        }

        return defaultValue;
    }

    /// <summary>
    /// 将 DateTimeOffset 转换成本地 DateTime
    /// </summary>
    /// <param name="dateTime"><see cref="DateTimeOffset"/></param>
    /// <returns></returns>
    public static DateTime ParseToDateTime(this DateTimeOffset dateTime)
    {
        if (dateTime.Offset.Equals(TimeSpan.Zero))
        {
            return dateTime.UtcDateTime;
        }

        if (dateTime.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(dateTime.DateTime)))
        {
            return dateTime.ToLocalTime().DateTime;
        }

        return dateTime.DateTime;
    }

    /// <summary>
    /// 将 DateTimeOffset? 转换成本地 DateTime?
    /// </summary>
    /// <param name="dateTime"><see cref="DateTimeOffset"/></param>
    /// <returns></returns>
    public static DateTime? ParseToDateTime(this DateTimeOffset? dateTime)
    {
        return dateTime?.ParseToDateTime();
    }

    /// <summary>
    /// 将 DateTime 转换成 DateTimeOffset
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns></returns>
    public static DateTimeOffset ParseToDateTimeOffset(this DateTime dateTime)
    {
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
    }

    /// <summary>
    /// 将 DateTime? 转换成 DateTimeOffset?
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns></returns>
    public static DateTimeOffset? ParseToDateTimeOffset(this DateTime? dateTime)
    {
        return dateTime?.ParseToDateTimeOffset();
    }

    /// <summary>
    /// 将毫秒时间戳转换为DateTime，若转换失败，则返回日期最小值。不抛出异常。  
    /// </summary>
    /// <param name="timeStamps"><see cref="long"/></param>
    /// <returns><see cref="DateTime"/></returns>
    public static DateTime ParseToDateTime_Milliseconds(this long timeStamps)
    {
        try
        {
            // 当地时区
            return timeStamps == 0 ? DateTime.MinValue : GlobalConstant.DefaultTime.AddMilliseconds(timeStamps);
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
    public static DateTime ParseToDateTime_Milliseconds(this long timeStamps, DateTime? defaultValue)
    {
        try
        {
            // 当地时区
            return timeStamps == 0 ? defaultValue.GetValueOrDefault() : GlobalConstant.DefaultTime.AddMilliseconds(timeStamps);
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
    public static DateTime ParseToDateTime_Seconds(this long timeStamps)
    {
        try
        {
            // 当地时区
            return timeStamps == 0 ? DateTime.MinValue : GlobalConstant.DefaultTime.AddSeconds(timeStamps);
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
    public static DateTime ParseToDateTime_Seconds(this long timeStamps, DateTime? defaultValue)
    {
        try
        {
            // 当地时区
            return timeStamps == 0 ? defaultValue.GetValueOrDefault() : GlobalConstant.DefaultTime.AddSeconds(timeStamps);
        }
        catch
        {
            return defaultValue.GetValueOrDefault();
        }
    }

    #endregion

    #region 转换为ToUnixTime

    /// <summary>
    /// 将 DateTime 转为 UnixTime
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime"/></param>
    /// <returns><see cref="long"/></returns>
    public static long ParseToUnixTime(this DateTime dateTime)
    {
        var startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return (long) Math.Round((dateTime - startTime).TotalMilliseconds, MidpointRounding.AwayFromZero);
    }

    #endregion

    #region 强制转换类型

    /// <summary>
    /// 强制转换类型
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<TResult> CastSuper<TResult>(this IEnumerable source)
    {
        return from object item in source select (TResult) Convert.ChangeType(item, typeof(TResult));
    }

    #endregion
}