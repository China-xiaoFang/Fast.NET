using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Fast.Json.JsonConverter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Json.Extensions;

/// <summary>
/// <see cref="IServiceCollection"/> 拓展类
/// </summary>
public static class JsonIServiceCollectionExtension
{
    /// <summary>
    /// 添加JSON序列化配置
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddJsonOptions(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableDateTimeJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new DateTimeOffsetJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableDateTimeOffsetJsonConverter());
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

        return services;
    }
}