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

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fast.Serialization.JsonConverter.Internal;

/// <summary>
/// <see cref="IntJsonConverter"/> int 类型Json返回处理
/// </summary>
internal class IntJsonConverter : JsonConverter<int>
{
    /// <summary>Reads and converts the JSON to type <see cref="int"/>.</summary>
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
/// <see cref="NullableIntJsonConverter"/> int? 类型Json返回处理
/// </summary>
internal class NullableIntJsonConverter : JsonConverter<int?>
{
    /// <summary>Reads and converts the JSON to type <see cref="int"/>.</summary>
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