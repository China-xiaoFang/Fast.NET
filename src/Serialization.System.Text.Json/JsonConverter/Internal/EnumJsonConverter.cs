// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fast.Serialization;

/// <summary>
/// Enum 类型 JSON 返回处理
/// </summary>
/// <typeparam name="T">序列化或转换后的对象类型</typeparam>
internal sealed class EnumJsonConverter<T> : JsonConverter<T> where T : struct, Enum
{
    /// <inheritdoc />
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 同时接受枚举名称和底层数值
        if (reader.TokenType == JsonTokenType.String)
        {
            string enumValueStr = reader.GetString();
            if (Enum.TryParse(enumValueStr, out T enumValue))
            {
                return enumValue;
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            // 按枚举底层类型的 TypeCode 分派数值转换逻辑
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (Type.GetTypeCode(typeToConvert))
            {
                case TypeCode.SByte:
                    return (T)Enum.ToObject(typeToConvert, reader.GetSByte());
                case TypeCode.Byte:
                    return (T)Enum.ToObject(typeToConvert, reader.GetByte());
                case TypeCode.Int16:
                    return (T)Enum.ToObject(typeToConvert, reader.GetInt16());
                case TypeCode.UInt16:
                    return (T)Enum.ToObject(typeToConvert, reader.GetUInt16());
                case TypeCode.Int32:
                    return (T)Enum.ToObject(typeToConvert, reader.GetInt32());
                case TypeCode.UInt32:
                    return (T)Enum.ToObject(typeToConvert, reader.GetUInt32());
                case TypeCode.Int64:
                    return (T)Enum.ToObject(typeToConvert, reader.GetInt64());
                case TypeCode.UInt64:
                    return (T)Enum.ToObject(typeToConvert, reader.GetUInt64());
                case TypeCode.Boolean:
                    return (T)Enum.ToObject(typeToConvert, reader.GetBoolean());
            }
        }

        throw new JsonException($"Unable to convert JSON value to Enum {typeToConvert}");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(Convert.ToInt64(value));
    }
}

/// <summary>
/// Enum? 类型 JSON 返回处理
/// </summary>
/// <typeparam name="T">序列化或转换后的对象类型</typeparam>
internal sealed class NullableEnumJsonConverter<T> : JsonConverter<T?> where T : struct, Enum
{
    /// <inheritdoc />
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 同时接受枚举名称和底层数值；空字符串按 null 处理
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        Type underlyingType = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;

        if (reader.TokenType == JsonTokenType.String)
        {
            if (Enum.TryParse(underlyingType, reader.GetString(), out object enumValueObj))
            {
                return (T?)enumValueObj;
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            // 按枚举底层类型的 TypeCode 分派数值转换逻辑
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (Type.GetTypeCode(underlyingType))
            {
                case TypeCode.SByte:
                    return (T?)Enum.ToObject(underlyingType, reader.GetSByte());
                case TypeCode.Byte:
                    return (T?)Enum.ToObject(underlyingType, reader.GetByte());
                case TypeCode.Int16:
                    return (T?)Enum.ToObject(underlyingType, reader.GetInt16());
                case TypeCode.UInt16:
                    return (T?)Enum.ToObject(underlyingType, reader.GetUInt16());
                case TypeCode.Int32:
                    return (T?)Enum.ToObject(underlyingType, reader.GetInt32());
                case TypeCode.UInt32:
                    return (T?)Enum.ToObject(underlyingType, reader.GetUInt32());
                case TypeCode.Int64:
                    return (T?)Enum.ToObject(underlyingType, reader.GetInt64());
                case TypeCode.UInt64:
                    return (T?)Enum.ToObject(underlyingType, reader.GetUInt64());
                case TypeCode.Boolean:
                    return (T?)Enum.ToObject(underlyingType, reader.GetBoolean());
            }
        }

        throw new JsonException($"Unable to convert JSON value to Enum {typeToConvert}");
    }

    /// <inheritdoc />
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
