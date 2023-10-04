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

using System.Collections;
using System.Globalization;

namespace Fast.IaaS.Extensions;

/// <summary>
/// <see cref="Convert"/> 转换拓展类
/// </summary>
public static class ConvertExtension
{
    #region 转换为long

    /// <summary>
    /// 将object转换为long，若转换失败，则返回0。不抛出异常。  
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static long ParseToLong(this object obj)
    {
        try
        {
            return Convert.ToInt64(obj);
        }
        catch
        {
            try
            {
                return long.Parse(obj.ToString() ?? string.Empty);
            }
            catch
            {
                return 0L;
            }
        }
    }

    /// <summary>
    /// 将object转换为long，若转换失败，则返回指定值。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static long ParseToLong(this string str, long defaultValue)
    {
        try
        {
            return Convert.ToInt64(str);
        }
        catch
        {
            try
            {
                return long.Parse(str ?? string.Empty);
            }
            catch
            {
                return 0L;
            }
        }
    }

    #endregion

    #region 转换为int

    /// <summary>
    /// 将object转换为int，若转换失败，则返回0。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int ParseToInt(this object str)
    {
        try
        {
            return Convert.ToInt32(str);
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// 将object转换为int，若转换失败，则返回指定值。不抛出异常。 
    /// null返回默认值
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static int ParseToInt(this object str, int defaultValue)
    {
        if (str == null)
        {
            return defaultValue;
        }

        try
        {
            return Convert.ToInt32(str);
        }
        catch
        {
            return defaultValue;
        }
    }

    #endregion

    #region 转换为short

    /// <summary>
    /// 将object转换为short，若转换失败，则返回0。不抛出异常。  
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static short ParseToShort(this object obj)
    {
        try
        {
            return short.Parse(obj.ToString() ?? string.Empty);
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// 将object转换为short，若转换失败，则返回指定值。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static short ParseToShort(this object str, short defaultValue)
    {
        try
        {
            return short.Parse(str.ToString() ?? string.Empty);
        }
        catch
        {
            return defaultValue;
        }
    }

    #endregion

    #region 转换为demical

    /// <summary>
    /// 将object转换为demical，若转换失败，则返回指定值。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static decimal ParseToDecimal(this object str, decimal defaultValue)
    {
        try
        {
            return decimal.Parse(str.ToString() ?? string.Empty);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// 将object转换为demical，若转换失败，则返回0。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static decimal ParseToDecimal(this object str)
    {
        try
        {
            return decimal.Parse(str.ToString() ?? string.Empty);
        }
        catch
        {
            return 0;
        }
    }

    #endregion

    #region 转化为bool

    /// <summary>
    /// 将object转换为bool，若转换失败，则返回false。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool ParseToBool(this object str)
    {
        try
        {
            return bool.Parse(str.ToString() ?? string.Empty);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 将object转换为bool，若转换失败，则返回指定值。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool ParseToBool(this object str, bool result)
    {
        try
        {
            return bool.Parse(str.ToString() ?? string.Empty);
        }
        catch
        {
            return result;
        }
    }

    #endregion

    #region 转换为float

    /// <summary>
    /// 将object转换为float，若转换失败，则返回0。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static float ParseToFloat(this object str)
    {
        try
        {
            return float.Parse(str.ToString() ?? string.Empty);
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// 将object转换为float，若转换失败，则返回指定值。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static float ParseToFloat(this object str, float result)
    {
        try
        {
            return float.Parse(str.ToString() ?? string.Empty);
        }
        catch
        {
            return result;
        }
    }

    #endregion

    #region 转换为Guid

    /// <summary>
    /// 将string转换为Guid，若转换失败，则返回Guid.Empty。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static Guid ParseToGuid(this string str)
    {
        try
        {
            return new Guid(str);
        }
        catch
        {
            return Guid.Empty;
        }
    }

    #endregion

    #region 转换为DateTime

    /// <summary>
    /// 将string转换为DateTime，若转换失败，则返回日期最小值。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static DateTime ParseToDateTime(this string str)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return DateTime.MinValue;
            }

            if (str.Contains("-") || str.Contains("/"))
            {
                return DateTime.Parse(str);
            }

            var length = str.Length;
            return length switch
            {
                4 => DateTime.ParseExact(str, "yyyy", CultureInfo.CurrentCulture),
                6 => DateTime.ParseExact(str, "yyyyMM", CultureInfo.CurrentCulture),
                8 => DateTime.ParseExact(str, "yyyyMMdd", CultureInfo.CurrentCulture),
                10 => DateTime.ParseExact(str, "yyyyMMddHH", CultureInfo.CurrentCulture),
                12 => DateTime.ParseExact(str, "yyyyMMddHHmm", CultureInfo.CurrentCulture),
                // ReSharper disable once StringLiteralTypo
                14 => DateTime.ParseExact(str, "yyyyMMddHHmmss", CultureInfo.CurrentCulture),
                // ReSharper disable once StringLiteralTypo
                _ => DateTime.ParseExact(str, "yyyyMMddHHmmss", CultureInfo.CurrentCulture)
            };
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// 将string转换为DateTime，若转换失败，则返回默认值。  
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static DateTime ParseToDateTime(this string str, DateTime? defaultValue)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return defaultValue.GetValueOrDefault();
            }

            if (str.Contains("-") || str.Contains("/"))
            {
                return DateTime.Parse(str);
            }

            var length = str.Length;
            return length switch
            {
                4 => DateTime.ParseExact(str, "yyyy", CultureInfo.CurrentCulture),
                6 => DateTime.ParseExact(str, "yyyyMM", CultureInfo.CurrentCulture),
                8 => DateTime.ParseExact(str, "yyyyMMdd", CultureInfo.CurrentCulture),
                10 => DateTime.ParseExact(str, "yyyyMMddHH", CultureInfo.CurrentCulture),
                12 => DateTime.ParseExact(str, "yyyyMMddHHmm", CultureInfo.CurrentCulture),
                // ReSharper disable once StringLiteralTypo
                14 => DateTime.ParseExact(str, "yyyyMMddHHmmss", CultureInfo.CurrentCulture),
                // ReSharper disable once StringLiteralTypo
                _ => DateTime.ParseExact(str, "yyyyMMddHHmmss", CultureInfo.CurrentCulture)
            };
        }
        catch
        {
            return defaultValue.GetValueOrDefault();
        }
    }

    /// <summary>
    /// 将 DateTimeOffset 转换成本地 DateTime
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime ParseToDateTime(this DateTimeOffset dateTime)
    {
        if (dateTime.Offset.Equals(TimeSpan.Zero))
            return dateTime.UtcDateTime;
        if (dateTime.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(dateTime.DateTime)))
            return dateTime.ToLocalTime().DateTime;
        return dateTime.DateTime;
    }

    /// <summary>
    /// 将 DateTimeOffset? 转换成本地 DateTime?
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime? ParseToDateTime(this DateTimeOffset? dateTime)
    {
        return dateTime?.ParseToDateTime();
    }

    /// <summary>
    /// 将 DateTime 转换成 DateTimeOffset
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTimeOffset ParseToDateTimeOffset(this DateTime dateTime)
    {
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
    }

    /// <summary>
    /// 将 DateTime? 转换成 DateTimeOffset?
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTimeOffset? ParseToDateTimeOffset(this DateTime? dateTime)
    {
        return dateTime?.ParseToDateTimeOffset();
    }

    #endregion

    #region 转换为string

    /// <summary>
    /// 将object转换为string，若转换失败，则返回""。不抛出异常。  
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ParseToString(this object obj)
    {
        try
        {
            return obj == null ? string.Empty : obj.ToString();
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ParseToStrings<T>(this object obj)
    {
        try
        {
            if (obj is IEnumerable<T> list)
            {
                return string.Join(",", list);
            }

            return obj.ToString();
        }
        catch
        {
            return string.Empty;
        }
    }

    #endregion

    #region 转换为double

    /// <summary>
    /// 将object转换为double，若转换失败，则返回0。不抛出异常。  
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static double ParseToDouble(this object obj)
    {
        try
        {
            return double.Parse(obj.ToString() ?? string.Empty);
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// 将object转换为double，若转换失败，则返回指定值。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static double ParseToDouble(this object str, double defaultValue)
    {
        try
        {
            return double.Parse(str.ToString() ?? string.Empty);
        }
        catch
        {
            return defaultValue;
        }
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

    #region 转换为ToUnixTime

    public static long ParseToUnixTime(this DateTime nowTime)
    {
        var startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return (long) Math.Round((nowTime - startTime).TotalMilliseconds, MidpointRounding.AwayFromZero);
    }

    #endregion
}