using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fast.Json.JsonConverter;

internal static class DateTimeOffsetExtension
{
    /// <summary>
    /// 将 DateTimeOffset 转换成本地 DateTime
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime ConvertToDateTime(this DateTimeOffset dateTime)
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
    public static DateTime? ConvertToDateTime(this DateTimeOffset? dateTime)
    {
        return dateTime.HasValue ? dateTime.Value.ConvertToDateTime() : null;
    }
}

/// <summary>
/// DateTimeOffset 序列化处理
/// </summary>
public class DateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
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

    /// <summary>Reads and converts the JSON to type <typeparamref name="T" />.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.SpecifyKind(Convert.ToDateTime(reader.GetString()), Localized ? DateTimeKind.Local : DateTimeKind.Utc);
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        // 判断是否序列化成当地时间
        var formatDateTime = Localized ? value.ConvertToDateTime() : value;
        writer.WriteStringValue(formatDateTime.ToString(Format));
    }
}

/// <summary>
/// DateTimeOffset? 序列化处理
/// </summary>
public class NullableDateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset?>
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

    /// <summary>Reads and converts the JSON to type <typeparamref name="T" />.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.SpecifyKind(Convert.ToDateTime(reader.GetString()), Localized ? DateTimeKind.Local : DateTimeKind.Utc);
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
        {
            // 判断是否序列化成当地时间
            var formatDateTime = Localized ? value.ConvertToDateTime() : value;
            writer.WriteStringValue(formatDateTime.Value.ToString(Format));
        }
    }
}