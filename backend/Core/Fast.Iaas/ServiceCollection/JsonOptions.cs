using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Fast.ServiceCollection.JsonConverter;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.ServiceCollection.ServiceCollection;

/// <summary>
/// JSON序列化配置
/// </summary>
public static class JsonOptions
{
    /// <summary>
    /// 添加JSON序列化配置
    /// </summary>
    /// <param name="service"></param>
    /// <param name="dateTimeFormat">DateTime类型返回格式</param>
    public static void AddJsonOptions(this IServiceCollection service, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss")
    {
        service.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.AddDateTimeTypeConverters(dateTimeFormat);
            // Configuring too long integer types to return to the front end can cause a loss of precision.
            options.JsonSerializerOptions.Converters.Add(new LongJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableLongJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new IntJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableIntJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new DecimalJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableDecimalJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new DoubleJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableDoubleJsonConverter());
            // Ignore circular references only .NET 6 support.
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            // JSON garbled code problem.
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        });
    }
}