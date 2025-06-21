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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace Fast.Serialization;

/// <summary>
/// <see cref="EnumJsonConverter"/> Enum 类型Json返回处理
/// </summary>
internal class EnumJsonConverter : JsonConverter
{
    /// <summary>Writes the JSON representation of the object.</summary>
    /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value == null)
            writer.WriteNull();
        else
        {
            // 获取枚举类型
            var enumType = value.GetType();
            var underlyingType = Nullable.GetUnderlyingType(enumType) ?? enumType;

            // 通过 Type.GetTypeCode() 获取底层类型的 TypeCode，判断是是什么类型的值
            var typeCode = Type.GetTypeCode(underlyingType);

            // 判断是否为 long 类型
            if (typeCode == TypeCode.Int64)
            {
                writer.WriteValue($"{Convert.ToInt64(value)}");
            }
            else
            {
                writer.WriteValue(value);
            }
        }
    }

    /// <summary>Reads the JSON representation of the object.</summary>
    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing value of object being read.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>The object value.</returns>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        // 前端传入的 Enum 类型可能为 String 类型，或者 Number 类型。
        if (reader.TokenType == JsonToken.Null)
            return null;

        var underlyingType = Nullable.GetUnderlyingType(objectType) ?? objectType;

        if (reader.TokenType == JsonToken.String)
        {
            var jToken = JToken.ReadFrom(reader);
            var enumValueStr = jToken.Value<string>();
            if (Enum.TryParse(underlyingType, enumValueStr, out var enumValue))
            {
                return enumValue;
            }
        }
        else if (reader.TokenType == JsonToken.Integer)
        {
            // 通过 Type.GetTypeCode() 获取底层类型的 TypeCode，判断是是什么类型的值
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (Type.GetTypeCode(underlyingType))
            {
                case TypeCode.SByte:
                {
                    var value = Convert.ToSByte(reader.Value);

                    sbyte flagValues = 0;
                    foreach (var enumValue in Enum.GetValues(underlyingType))
                    {
                        flagValues |= Convert.ToSByte(enumValue);
                    }

                    if ((value & flagValues) == value)
                    {
                        return Enum.ToObject(underlyingType, value);
                    }

                    if (Enum.IsDefined(underlyingType, value))
                    {
                        return Enum.ToObject(underlyingType, value);
                    }

                    // 如果不是枚举值，并且值还是0，则默认为null
                    if (value == 0)
                    {
                        return null;
                    }
                }
                    break;
                case TypeCode.Byte:
                {
                    var value = Convert.ToByte(reader.Value);

                    byte flagValues = 0;
                    foreach (var enumValue in Enum.GetValues(underlyingType))
                    {
                        flagValues |= Convert.ToByte(enumValue);
                    }

                    if ((value & flagValues) == value)
                    {
                        return Enum.ToObject(underlyingType, value);
                    }

                    if (Enum.IsDefined(underlyingType, value))
                    {
                        return Enum.ToObject(underlyingType, value);
                    }

                    // 如果不是枚举值，并且值还是0，则默认为null
                    if (value == 0)
                    {
                        return null;
                    }
                }
                    break;
                case TypeCode.Int16:
                {
                    var value = Convert.ToInt16(reader.Value);

                    short flagValues = 0;
                    foreach (var enumValue in Enum.GetValues(underlyingType))
                    {
                        flagValues |= Convert.ToInt16(enumValue);
                    }

                    if ((value & flagValues) == value)
                    {
                        return Enum.ToObject(underlyingType, value);
                    }

                    if (Enum.IsDefined(underlyingType, value))
                    {
                        return Enum.ToObject(underlyingType, value);
                    }

                    // 如果不是枚举值，并且值还是0，则默认为null
                    if (value == 0)
                    {
                        return null;
                    }
                }
                    break;
                case TypeCode.UInt16:
                {
                    var value = Convert.ToUInt16(reader.Value);

                    ushort flagValues = 0;
                    foreach (var enumValue in Enum.GetValues(underlyingType))
                    {
                        flagValues |= Convert.ToUInt16(enumValue);
                    }

                    if ((value & flagValues) == value)
                    {
                        return Enum.ToObject(underlyingType, value);
                    }

                    if (Enum.IsDefined(underlyingType, value))
                    {
                        return Enum.ToObject(underlyingType, value);
                    }

                    // 如果不是枚举值，并且值还是0，则默认为null
                    if (value == 0)
                    {
                        return null;
                    }
                }
                    break;
                case TypeCode.Int32:
                {
                    var value = Convert.ToInt32(reader.Value);

                    var flagValues = 0;
                    foreach (var enumValue in Enum.GetValues(underlyingType))
                    {
                        flagValues |= Convert.ToInt32(enumValue);
                    }

                    if ((value & flagValues) == value)
                    {
                        return Enum.ToObject(underlyingType, value);
                    }

                    if (Enum.IsDefined(underlyingType, value))
                    {
                        return Enum.ToObject(underlyingType, value);
                    }

                    // 如果不是枚举值，并且值还是0，则默认为null
                    if (value == 0)
                    {
                        return null;
                    }
                }
                    break;
                case TypeCode.UInt32:
                {
                    var value = Convert.ToUInt32(reader.Value);

                    uint flagValues = 0;
                    foreach (var enumValue in Enum.GetValues(underlyingType))
                    {
                        flagValues |= Convert.ToUInt32(enumValue);
                    }

                    if ((value & flagValues) == value)
                    {
                        return Enum.ToObject(underlyingType, value);
                    }

                    if (Enum.IsDefined(underlyingType, value))
                    {
                        return Enum.ToObject(underlyingType, value);
                    }

                    // 如果不是枚举值，并且值还是0，则默认为null
                    if (value == 0)
                    {
                        return null;
                    }
                }
                    break;
                case TypeCode.Int64:
                {
                    var value = Convert.ToInt64(reader.Value);

                    long flagValues = 0;
                    foreach (var enumValue in Enum.GetValues(underlyingType))
                    {
                        flagValues |= Convert.ToInt64(enumValue);
                    }

                    if ((value & flagValues) == value)
                    {
                        return Enum.ToObject(underlyingType, value);
                    }

                    if (Enum.IsDefined(underlyingType, value))
                    {
                        return Enum.ToObject(underlyingType, value);
                    }

                    // 如果不是枚举值，并且值还是0，则默认为null
                    if (value == 0)
                    {
                        return null;
                    }
                }
                    break;
                case TypeCode.UInt64:
                {
                    var value = Convert.ToUInt64(reader.Value);

                    ulong flagValues = 0;
                    foreach (var enumValue in Enum.GetValues(underlyingType))
                    {
                        flagValues |= Convert.ToUInt64(enumValue);
                    }

                    if ((value & flagValues) == value)
                    {
                        return Enum.ToObject(underlyingType, value);
                    }

                    if (Enum.IsDefined(underlyingType, value))
                    {
                        return Enum.ToObject(underlyingType, value);
                    }

                    // 如果不是枚举值，并且值还是0，则默认为null
                    if (value == 0)
                    {
                        return null;
                    }
                }
                    break;
                case TypeCode.Boolean:
                {
                    var value = Convert.ToBoolean(reader.Value);
                    if (Enum.IsDefined(underlyingType, value))
                    {
                        return Enum.ToObject(underlyingType, value);
                    }
                }
                    break;
            }
        }

        throw new JsonSerializationException($"Unable to convert JSON value to Enum {objectType}");
    }

    /// <summary>
    /// Determines whether this instance can convert the specified object type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>
    /// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
    /// </returns>
    public override bool CanConvert(Type objectType)
    {
        if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>))
            return Nullable.GetUnderlyingType(objectType)
                       ?.IsEnum
                   == true;
        return objectType.IsEnum;
    }
}