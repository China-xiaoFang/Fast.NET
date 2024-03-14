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

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fast.Serialization.JsonConverter.Internal;

/// <summary>
/// <see cref="EnumJsonConverter{T}"/> Enum 类型Json返回处理
/// </summary>
internal class EnumJsonConverter<T> : JsonConverter<T> where T : struct, Enum
{
    /// <summary>Reads and converts the JSON to type <see cref="int"/>.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 这里做处理，前端传入的Enum类型可能为String类型，或者Number类型。
        if (reader.TokenType == JsonTokenType.String)
        {
            var enumValueStr = reader.GetString();
            if (Enum.TryParse(enumValueStr, out T enumValue))
            {
                return enumValue;
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            // 通过 Type.GetTypeCode() 获取底层类型的 TypeCode，判断是是什么类型的值
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (Type.GetTypeCode(typeToConvert))
            {
                case TypeCode.SByte:
                    return (T) Enum.ToObject(typeToConvert, reader.GetSByte());
                case TypeCode.Byte:
                    return (T) Enum.ToObject(typeToConvert, reader.GetByte());
                case TypeCode.Int16:
                    return (T) Enum.ToObject(typeToConvert, reader.GetInt16());
                case TypeCode.UInt16:
                    return (T) Enum.ToObject(typeToConvert, reader.GetUInt16());
                case TypeCode.Int32:
                    return (T) Enum.ToObject(typeToConvert, reader.GetInt32());
                case TypeCode.UInt32:
                    return (T) Enum.ToObject(typeToConvert, reader.GetUInt32());
                case TypeCode.Int64:
                    return (T) Enum.ToObject(typeToConvert, reader.GetInt64());
                case TypeCode.UInt64:
                    return (T) Enum.ToObject(typeToConvert, reader.GetUInt64());
                case TypeCode.Boolean:
                    return (T) Enum.ToObject(typeToConvert, reader.GetBoolean());
            }
        }

        throw new JsonException($"Unable to convert JSON value to Enum {typeToConvert}");
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(Convert.ToInt64(value));
    }
}

/// <summary>
/// <see cref="NullableEnumJsonConverter{T}"/> Enum? 类型Json返回处理
/// </summary>
internal class NullableEnumJsonConverter<T> : JsonConverter<T?> where T : struct, Enum
{
    /// <summary>Reads and converts the JSON to type <see cref="int"/>.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 这里做处理，前端传入的Enum类型可能为String类型，或者Number类型。
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        var underlyingType = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;

        if (reader.TokenType == JsonTokenType.String)
        {
            if (Enum.TryParse(underlyingType, reader.GetString(), out var enumValueObj))
            {
                return (T?) enumValueObj;
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            // 通过 Type.GetTypeCode() 获取底层类型的 TypeCode，判断是是什么类型的值
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (Type.GetTypeCode(underlyingType))
            {
                case TypeCode.SByte:
                    return (T?) Enum.ToObject(underlyingType, reader.GetSByte());
                case TypeCode.Byte:
                    return (T?) Enum.ToObject(underlyingType, reader.GetByte());
                case TypeCode.Int16:
                    return (T?) Enum.ToObject(underlyingType, reader.GetInt16());
                case TypeCode.UInt16:
                    return (T?) Enum.ToObject(underlyingType, reader.GetUInt16());
                case TypeCode.Int32:
                    return (T?) Enum.ToObject(underlyingType, reader.GetInt32());
                case TypeCode.UInt32:
                    return (T?) Enum.ToObject(underlyingType, reader.GetUInt32());
                case TypeCode.Int64:
                    return (T?) Enum.ToObject(underlyingType, reader.GetInt64());
                case TypeCode.UInt64:
                    return (T?) Enum.ToObject(underlyingType, reader.GetUInt64());
                case TypeCode.Boolean:
                    return (T?) Enum.ToObject(underlyingType, reader.GetBoolean());
            }
        }

        throw new JsonException($"Unable to convert JSON value to Enum {typeToConvert}");
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteNumberValue(Convert.ToInt64(value));
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}