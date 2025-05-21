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
using System.Text.Json;
using System.Text.Json.Serialization;

// ReSharper disable once CheckNamespace
namespace Fast.Serialization;

/// <summary>
/// <see cref="TimeJsonConverter"/> Datetime 类型Json返回时间处理
/// </summary>
public class TimeJsonConverter : JsonConverter<DateTime>
{
    /// <summary>
    /// 日期格式化
    /// 默认：HH:mm:ss
    /// </summary>
    public string Format { get; set; }

    public TimeJsonConverter()
    {
        Format = "HH:mm:ss";
    }

    public TimeJsonConverter(string format)
    {
        Format = format;
    }

    /// <summary>Reads and converts the JSON to type <see cref="DateTime"/>.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString()!;

        if (value.Contains("-") || value.Contains("/") || value.Contains(":"))
        {
            var result = DateTime.Parse(value);

            result = new DateTime(1970, 1, 1, result.Hour, result.Minute, result.Second);
            return result;
        }

        switch (value.Length)
        {
            case 2:
            {
                var result = DateTime.ParseExact(value, "HH", CultureInfo.CurrentCulture, DateTimeStyles.None);

                result = new DateTime(1970, 1, 1, result.Hour, 0, 0);
                return result;
            }
            case 4:
            {
                var result = DateTime.ParseExact(value, "HHmm", CultureInfo.CurrentCulture, DateTimeStyles.None);

                result = new DateTime(1970, 1, 1, result.Hour, result.Minute, 0);
                return result;
            }
            default:
            {
                var result = DateTime.ParseExact(value, "HHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None);

                result = new DateTime(1970, 1, 1, result.Hour, result.Minute, result.Second);
                return result;
            }
        }
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }
}

/// <summary>
/// <see cref="NullableTimeJsonConverter"/> Datetime? 类型Json返回时间处理
/// </summary>
public class NullableTimeJsonConverter : JsonConverter<DateTime?>
{
    /// <summary>
    /// 日期格式化
    /// 默认：HH:mm:ss
    /// </summary>
    public string Format { get; set; }

    public NullableTimeJsonConverter()
    {
        Format = "HH:mm:ss";
    }

    public NullableTimeJsonConverter(string format)
    {
        Format = format;
    }

    /// <summary>Reads and converts the JSON to type <see cref="DateTime"/>.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        if (value.Contains("-") || value.Contains("/") || value.Contains(":"))
        {
            var result = DateTime.Parse(value);

            result = new DateTime(1970, 1, 1, result.Hour, result.Minute, result.Second);
            return result;
        }

        switch (value.Length)
        {
            case 2:
            {
                var result = DateTime.ParseExact(value, "HH", CultureInfo.CurrentCulture, DateTimeStyles.None);

                result = new DateTime(1970, 1, 1, result.Hour, 0, 0);
                return result;
            }
            case 4:
            {
                var result = DateTime.ParseExact(value, "HHmm", CultureInfo.CurrentCulture, DateTimeStyles.None);

                result = new DateTime(1970, 1, 1, result.Hour, result.Minute, 0);
                return result;
            }
            default:
            {
                var result = DateTime.ParseExact(value, "HHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None);

                result = new DateTime(1970, 1, 1, result.Hour, result.Minute, result.Second);
                return result;
            }
        }
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.Value.ToString(Format));
    }
}