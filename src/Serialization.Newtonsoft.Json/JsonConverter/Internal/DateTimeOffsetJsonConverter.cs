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

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace Fast.Serialization;

/// <summary>
/// <see cref="DateTimeOffsetJsonConverter"/> DateTimeOffset 类型Json返回处理
/// </summary>
internal class DateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
{
    /// <summary>
    /// 格式化
    /// 默认：yyyy-MM-dd HH:mm:ss
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// 是否输出为为当地时间
    /// </summary>
    public bool Localized { get; private set; }

    public DateTimeOffsetJsonConverter()
    {
        Format = "yyyy-MM-dd HH:mm:ss";
    }

    public DateTimeOffsetJsonConverter(string format)
    {
        Format = format;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="format"></param>
    /// <param name="outputToLocalDateTime"></param>
    public DateTimeOffsetJsonConverter(string format, bool outputToLocalDateTime)
    {
        Format = format;
        Localized = outputToLocalDateTime;
    }

    /// <summary>Writes the JSON representation of the object.</summary>
    /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, DateTimeOffset value, JsonSerializer serializer)
    {
        // 判断是否序列化成当地时间
        var formatDateTime = value;
        if (Localized)
        {
            if (value.Offset.Equals(TimeSpan.Zero))
                formatDateTime = value.UtcDateTime;
            else if (value.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(value.Date)))
                formatDateTime = value.ToLocalTime()
                    .DateTime;
            else
                formatDateTime = value.DateTime;
        }

        writer.WriteValue(formatDateTime.ToString(Format));
    }

    /// <summary>Reads the JSON representation of the object.</summary>
    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing value of object being read. If there is no existing value then <c>null</c> will be used.</param>
    /// <param name="hasExistingValue">The existing value has a value.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>The object value.</returns>
    public override DateTimeOffset ReadJson(JsonReader reader, Type objectType, DateTimeOffset existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        var jToken = JToken.ReadFrom(reader);
        var value = jToken.Value<string>();

        DateTime result;

        if (value.Contains("-") || value.Contains("/") || value.Contains(":"))
        {
            result = DateTime.Parse(value);
        }
        else
        {
            switch (value.Length)
            {
                case 4:
                {
                    result = DateTime.ParseExact(value, "yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, 1, 1, 0, 0, 0);
                }
                    break;
                case 6:
                {
                    result = DateTime.ParseExact(value, "yyyyMM", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, 1, 0, 0, 0);
                }
                    break;
                case 8:
                {
                    result = DateTime.ParseExact(value, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, result.Day, 0, 0, 0);
                }
                    break;
                case 10:
                {
                    result = DateTime.ParseExact(value, "yyyyMMddHH", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, result.Day, result.Hour, 0, 0);
                }
                    break;
                case 12:
                {
                    result = DateTime.ParseExact(value, "yyyyMMddHHmm", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, result.Day, result.Hour, result.Minute, 0);
                }
                    break;
                default:
                {
                    result = DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None);
                }
                    break;
            }
        }

        return DateTime.SpecifyKind(result, Localized ? DateTimeKind.Local : DateTimeKind.Utc);
    }
}

/// <summary>
/// <see cref="NullableDateTimeOffsetJsonConverter"/> DateTimeOffset? 类型Json返回处理
/// </summary>
internal class NullableDateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset?>
{
    /// <summary>
    /// 格式化
    /// 默认：yyyy-MM-dd HH:mm:ss
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// 是否输出为为当地时间
    /// </summary>
    public bool Localized { get; private set; }

    public NullableDateTimeOffsetJsonConverter()
    {
        Format = "yyyy-MM-dd HH:mm:ss";
    }

    public NullableDateTimeOffsetJsonConverter(string format)
    {
        Format = format;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="format"></param>
    /// <param name="outputToLocalDateTime"></param>
    public NullableDateTimeOffsetJsonConverter(string format, bool outputToLocalDateTime)
    {
        Format = format;
        Localized = outputToLocalDateTime;
    }

    /// <summary>Writes the JSON representation of the object.</summary>
    /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, DateTimeOffset? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
        }
        else
        {
            // 判断是否序列化成当地时间
            var formatDateTime = value.Value;
            if (Localized)
            {
                if (value.Value.Offset.Equals(TimeSpan.Zero))
                    formatDateTime = value.Value.UtcDateTime;
                else if (value.Value.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(value.Value.Date)))
                    formatDateTime = value.Value.ToLocalTime()
                        .DateTime;
                else
                    formatDateTime = value.Value.DateTime;
            }

            writer.WriteValue(formatDateTime.ToString(Format));
        }
    }

    /// <summary>Reads the JSON representation of the object.</summary>
    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing value of object being read. If there is no existing value then <c>null</c> will be used.</param>
    /// <param name="hasExistingValue">The existing value has a value.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>The object value.</returns>
    public override DateTimeOffset? ReadJson(JsonReader reader, Type objectType, DateTimeOffset? existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var jToken = JToken.ReadFrom(reader);
        var value = jToken.Value<string>();

        DateTime result;

        if (value.Contains("-") || value.Contains("/") || value.Contains(":"))
        {
            result = DateTime.Parse(value);
        }
        else
        {
            switch (value.Length)
            {
                case 4:
                {
                    result = DateTime.ParseExact(value, "yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, 1, 1, 0, 0, 0);
                }
                    break;
                case 6:
                {
                    result = DateTime.ParseExact(value, "yyyyMM", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, 1, 0, 0, 0);
                }
                    break;
                case 8:
                {
                    result = DateTime.ParseExact(value, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, result.Day, 0, 0, 0);
                }
                    break;
                case 10:
                {
                    result = DateTime.ParseExact(value, "yyyyMMddHH", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, result.Day, result.Hour, 0, 0);
                }
                    break;
                case 12:
                {
                    result = DateTime.ParseExact(value, "yyyyMMddHHmm", CultureInfo.CurrentCulture, DateTimeStyles.None);

                    result = new DateTime(result.Year, result.Month, result.Day, result.Hour, result.Minute, 0);
                }
                    break;
                default:
                {
                    result = DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None);
                }
                    break;
            }
        }

        return DateTime.SpecifyKind(result, Localized ? DateTimeKind.Local : DateTimeKind.Utc);
    }
}