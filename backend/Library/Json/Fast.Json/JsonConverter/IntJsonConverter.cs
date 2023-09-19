using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fast.Json.JsonConverter;

/// <summary>
/// int 类型Json返回处理
/// </summary>
public class IntJsonConverter : JsonConverter<int>
{
    /// <summary>Reads and converts the JSON to type int.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 这里做处理，前端传入的Int类型可能为String类型，或者Number类型。
        return reader.TokenType == JsonTokenType.String ? int.Parse(reader.GetString()) : reader.GetInt32();
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}

/// <summary>
/// int? 类型Json返回处理
/// </summary>
public class NullableIntJsonConverter : JsonConverter<int?>
{
    /// <summary>Reads and converts the JSON to type <typeparamref name="T" />.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 这里做处理，前端传入的Int类型可能为String类型，或者Number类型。
        if (reader.TokenType != JsonTokenType.String)
            return reader.GetInt32();

        var intString = reader.GetString();
        if (string.IsNullOrEmpty(intString))
        {
            return null;
        }

        return int.Parse(reader.GetString());
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(value.Value);
    }
}