using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fast.SDK.Common.Util.Json.JsonConverter;

/// <summary>
/// decimal 类型Json返回处理
/// </summary>
public class DecimalJsonConverter : JsonConverter<decimal>
{
    /// <summary>Reads and converts the JSON to type decimal.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetDecimal();
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("#0.00"));
    }
}