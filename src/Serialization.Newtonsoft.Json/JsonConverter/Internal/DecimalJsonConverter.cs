﻿// ------------------------------------------------------------------------
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
/// <see cref="DecimalJsonConverter"/> decimal 类型Json返回处理
/// </summary>
internal class DecimalJsonConverter : JsonConverter<decimal>
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

    /// <summary>Writes the JSON representation of the object.</summary>
    /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, decimal value, JsonSerializer serializer)
    {
        writer.WriteValue(Places == null ? (decimal) (double) value : Math.Round(value, Places.Value));
    }

    /// <summary>Reads the JSON representation of the object.</summary>
    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing value of object being read. If there is no existing value then <c>null</c> will be used.</param>
    /// <param name="hasExistingValue">The existing value has a value.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>The object value.</returns>
    public override decimal ReadJson(JsonReader reader, Type objectType, decimal existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        // 这里做处理，前端传入的Decimal类型可能为String类型，或者Number类型。
        if (reader.TokenType == JsonToken.String)
        {
            var jToken = JToken.ReadFrom(reader);
            var value = jToken.Value<string>();
            return decimal.Parse(value);
        }

        return Convert.ToDecimal(reader.Value);
    }
}

/// <summary>
/// <see cref="NullableDecimalJsonConverter"/> decimal? 类型Json返回处理
/// </summary>
internal class NullableDecimalJsonConverter : JsonConverter<decimal?>
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

    /// <summary>Writes the JSON representation of the object.</summary>
    /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, decimal? value, JsonSerializer serializer)
    {
        if (value == null)
            writer.WriteNull();
        else
            writer.WriteValue(Places == null ? (decimal) (double) value.Value : Math.Round(value.Value, Places.Value));
    }

    /// <summary>Reads the JSON representation of the object.</summary>
    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing value of object being read. If there is no existing value then <c>null</c> will be used.</param>
    /// <param name="hasExistingValue">The existing value has a value.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>The object value.</returns>
    public override decimal? ReadJson(JsonReader reader, Type objectType, decimal? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        // 这里做处理，前端传入的Decimal类型可能为String类型，或者Number类型。
        if (reader.TokenType != JsonToken.String)
            return Convert.ToDecimal(reader.Value);

        var jToken = JToken.ReadFrom(reader);
        var value = jToken.Value<string>();
        if (string.IsNullOrEmpty(value))
            return null;

        return decimal.Parse(value);
    }
}