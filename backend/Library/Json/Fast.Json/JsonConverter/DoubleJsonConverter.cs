using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fast.Json.JsonConverter;

/// <summary>
/// double 类型Json返回处理
/// </summary>
public class DoubleJsonConverter : JsonConverter<double>
{
    /// <summary>
    /// 小数点位数
    /// </summary>
    public int? Places { get; set; }

    public DoubleJsonConverter()
    {
        Places = null;
    }

    public DoubleJsonConverter(int places)
    {
        Places = places;
    }

    /// <summary>Reads and converts the JSON to type double.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 这里做处理，前端传入的Double类型可能为String类型，或者Number类型。
        return reader.TokenType == JsonTokenType.String ? double.Parse(reader.GetString()) : reader.GetDouble();
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(Places == null ? value : Math.Round(value, Places.Value));
    }
}

/// <summary>
/// double? 类型Json返回处理
/// </summary>
public class NullableDoubleJsonConverter : JsonConverter<double?>
{
    /// <summary>
    /// 小数点位数
    /// </summary>
    public int? Places { get; set; }

    public NullableDoubleJsonConverter()
    {
        Places = null;
    }

    public NullableDoubleJsonConverter(int places)
    {
        Places = places;
    }

    /// <summary>Reads and converts the JSON to type <typeparamref name="T" />.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override double? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 这里做处理，前端传入的Double类型可能为String类型，或者Number类型。
        if (reader.TokenType != JsonTokenType.String)
            return reader.GetDouble();

        var doubleString = reader.GetString();
        if (string.IsNullOrEmpty(doubleString))
        {
            return null;
        }

        return double.Parse(reader.GetString());
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, double? value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(Places == null ? value.Value : Math.Round(value.Value, Places.Value));
    }
}