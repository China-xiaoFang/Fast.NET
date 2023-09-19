using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fast.Json.JsonConverter;

/// <summary>
/// Datetime 类型返回日期Json处理
/// </summary>
public class DateJsonConverter : JsonConverter<DateTime>
{
    /// <summary>
    /// 日期格式化
    /// 默认：yyyy-MM-dd
    /// </summary>
    public string Format { get; set; }

    public DateJsonConverter()
    {
        Format = "yyyy-MM-dd";
    }

    public DateJsonConverter(string format)
    {
        Format = format;
    }

    /// <summary>Reads and converts the JSON to type <typeparamref name="T" />.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetDateTime();
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }
}

/// <summary>
/// Datetime? 类型返回日期Json处理
/// </summary>
public class NullableDateJsonConverter : JsonConverter<DateTime?>
{
    /// <summary>
    /// 日期格式化
    /// 默认：yyyy-MM-dd
    /// </summary>
    public string Format { get; set; }

    public NullableDateJsonConverter()
    {
        Format = "yyyy-MM-dd";
    }

    public NullableDateJsonConverter(string format)
    {
        Format = format;
    }

    /// <summary>Reads and converts the JSON to type <typeparamref name="T" />.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetDateTime();
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.Value.ToString(Format));
    }
}