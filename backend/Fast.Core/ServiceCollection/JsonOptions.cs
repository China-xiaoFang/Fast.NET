using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Fast.Core.ServiceCollection;

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
    /// <param name="isRun"></param>
    public static void AddJsonOptions(this IServiceCollection service, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss",
        bool isRun = true)
    {
        if (isRun)
        {
            service.AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.AddDateFormatString(dateTimeFormat);
                // Configuring too long integer types to return to the front end can cause a loss of precision.
                options.JsonSerializerOptions.Converters.Add(new LongJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new DecimalJsonConverter());
                // Ignore circular references only .NET 6 support.
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                // JSON garbled code problem.
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            });
        }
    }
}