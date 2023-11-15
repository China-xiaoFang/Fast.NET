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

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fast.NET;

namespace Fast.Serialization.JsonConverter.Internal;

/// <summary>
/// <see cref="DateTimeOffsetJsonConverter"/> DateTimeOffset 类型Json返回处理
/// </summary>
internal class DateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
{
    /// <summary>
    /// 格式化
    /// 默认：yyyy-MM-dd HH:mm:ss
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// 是否输出为为当地时间
    /// </summary>
    public bool Localized { get; private set; }

    public DateTimeOffsetJsonConverter()
    {
        Format = "yyyy-MM-dd HH:mm:ss";
    }

    public DateTimeOffsetJsonConverter(string format)
    {
        Format = format;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="format"></param>
    /// <param name="outputToLocalDateTime"></param>
    public DateTimeOffsetJsonConverter(string format, bool outputToLocalDateTime)
    {
        Format = format;
        Localized = outputToLocalDateTime;
    }

    /// <summary>Reads and converts the JSON to type <see cref="DateTimeOffset"/>.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString()!;

        DateTime result;

        if (value.Contains("-") || value.Contains("/") || value.Contains(":"))
        {
            result = DateTime.Parse(value);
        }
        else
        {
            switch (value.Length)
            {
                case 4:
                {
                    result = DateTime.ParseExact(value, "yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, 1, 1, 0, 0, 0);
                }
                    break;
                case 6:
                {
                    result = DateTime.ParseExact(value, "yyyyMM", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, 1, 0, 0, 0);
                }
                    break;
                case 8:
                {
                    result = DateTime.ParseExact(value, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, result.Day, 0, 0, 0);
                }
                    break;
                case 10:
                {
                    result = DateTime.ParseExact(value, "yyyyMMddHH", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, result.Day, result.Hour, 0, 0);
                }
                    break;
                case 12:
                {
                    result = DateTime.ParseExact(value, "yyyyMMddHHmm", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, result.Day, result.Hour, result.Minute, 0);
                }
                    break;
                default:
                {
                    result = DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None);
                }
                    break;
            }
        }

        return DateTime.SpecifyKind(result, Localized ? DateTimeKind.Local : DateTimeKind.Utc);
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        // 判断是否序列化成当地时间
        var formatDateTime = Localized ? value.ParseToDateTime() : value;
        writer.WriteStringValue(formatDateTime.ToString(Format));
    }
}

/// <summary>
/// <see cref="NullableDateTimeOffsetJsonConverter"/> DateTimeOffset? 类型Json返回处理
/// </summary>
internal class NullableDateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset?>
{
    /// <summary>
    /// 格式化
    /// 默认：yyyy-MM-dd HH:mm:ss
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// 是否输出为为当地时间
    /// </summary>
    public bool Localized { get; private set; }

    public NullableDateTimeOffsetJsonConverter()
    {
        Format = "yyyy-MM-dd HH:mm:ss";
    }

    public NullableDateTimeOffsetJsonConverter(string format)
    {
        Format = format;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="format"></param>
    /// <param name="outputToLocalDateTime"></param>
    public NullableDateTimeOffsetJsonConverter(string format, bool outputToLocalDateTime)
    {
        Format = format;
        Localized = outputToLocalDateTime;
    }

    /// <summary>Reads and converts the JSON to type <see cref="DateTimeOffset"/>.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString()!;

        DateTime result;

        if (value.Contains("-") || value.Contains("/") || value.Contains(":"))
        {
            result = DateTime.Parse(value);
        }
        else
        {
            switch (value.Length)
            {
                case 4:
                {
                    result = DateTime.ParseExact(value, "yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, 1, 1, 0, 0, 0);
                }
                    break;
                case 6:
                {
                    result = DateTime.ParseExact(value, "yyyyMM", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, 1, 0, 0, 0);
                }
                    break;
                case 8:
                {
                    result = DateTime.ParseExact(value, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, result.Day, 0, 0, 0);
                }
                    break;
                case 10:
                {
                    result = DateTime.ParseExact(value, "yyyyMMddHH", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, result.Day, result.Hour, 0, 0);
                }
                    break;
                case 12:
                {
                    result = DateTime.ParseExact(value, "yyyyMMddHHmm", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, result.Day, result.Hour, result.Minute, 0);
                }
                    break;
                default:
                {
                    result = DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None);
                }
                    break;
            }
        }

        return DateTime.SpecifyKind(result, Localized ? DateTimeKind.Local : DateTimeKind.Utc);
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            // 判断是否序列化成当地时间
            var formatDateTime = Localized ? value.ParseToDateTime() : value;
            if (formatDateTime == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(formatDateTime.Value.ToString(Format));
            }
        }
    }
}