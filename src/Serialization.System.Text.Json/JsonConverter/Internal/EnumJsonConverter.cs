// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;

// ReSharper disable once CheckNamespace
namespace Fast.Serialization;

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