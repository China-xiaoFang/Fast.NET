using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fast.Iaas.Extension;

namespace Gejia.WMS.Iaas.JsonConverter
{
    

/// <summary>
/// decimal 类型Json返回处理
/// </summary>
public class DecimalJsonConverter : JsonConverter<decimal>
{
    /// <summary>
    /// 小数点位数
    /// </summary>
    public int? Places { get; set; }

    public DecimalJsonConverter()
    {
        Places = null;
    }

    public DecimalJsonConverter(int places)
    {
        Places = places;
    }

    /// <summary>Reads and converts the JSON to type decimal.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 这里做处理，前端传入的Decimal类型可能为String类型，或者Number类型。
        return reader.TokenType == JsonTokenType.String ? decimal.Parse(reader.GetString()) : reader.GetDecimal();
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(Places == null ?(decimal) (double)  value : Math.Round(value, Places.Value));
    }
}

/// <summary>
/// decimal? 类型Json返回处理
/// </summary>
public class NullableDecimalJsonConverter : JsonConverter<decimal?>
{
    /// <summary>
    /// 小数点位数
    /// </summary>
    public int? Places { get; set; }

    public NullableDecimalJsonConverter()
    {
        Places = null;
    }

    public NullableDecimalJsonConverter(int places)
    {
        Places = places;
    }

    /// <summary>Reads and converts the JSON to type <typeparamref name="T" />.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 这里做处理，前端传入的Decimal类型可能为String类型，或者Number类型。
        if (reader.TokenType != JsonTokenType.String)
            return reader.GetDecimal();

        var decimalString = reader.GetString();
        if (string.IsNullOrEmpty(decimalString))
        {
            return null;
        }

        return decimal.Parse(reader.GetString());
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(Places == null ?(decimal) (double)  value.Value : Math.Round(value.Value, Places.Value));
    }
}
}