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

using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Fast.IaaS;
using Fast.Serialization.ConverterFactory;
using Fast.Serialization.JsonConverter.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Serialization.Injections;

/// <summary>
/// <see cref="SerializationInjection"/> 序列化服务注册
/// <remarks>DateTime 和 DateTimeOffset 格式化字符串，默认 "yyyy-MM-dd HH:mm:ss"</remarks>
/// </summary>
public class SerializationInjection : IHostInjection
{
    /// <summary>
    /// 排序
    /// <remarks>
    /// <para>顺序越大，越优先注册</para>
    /// <para>建议最大不超过9999</para>
    /// </remarks>
    /// </summary>
    public int Order => 69944;

    /// <summary>
    /// 配置
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((hostContext, services) =>
        {
            Debugging.Info("Registering serialization......");

            // 这里其实可以改为从配置文件中读取，但是目前好像没必要，后续再看吧~~~
            const string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            services.Configure<JsonOptions>(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter(dateTimeFormat));
                options.JsonSerializerOptions.Converters.Add(new NullableDateTimeJsonConverter(dateTimeFormat));
                options.JsonSerializerOptions.Converters.Add(new DateTimeOffsetJsonConverter(dateTimeFormat));
                options.JsonSerializerOptions.Converters.Add(new NullableDateTimeOffsetJsonConverter(dateTimeFormat));
                // 解决 long 类型返回前端可能会导致精度丢失的问题
                options.JsonSerializerOptions.Converters.Add(new LongJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new NullableLongJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new IntJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new NullableIntJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new DecimalJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new NullableDecimalJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new DoubleJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new NullableDoubleJsonConverter());
                // 解决 Exception 类型不能被正常序列化和反序列化的问题
                options.JsonSerializerOptions.Converters.Add(new ExceptionJsonConverter());
                // 解决 Enum 类型前端传入 string 的问题
                options.JsonSerializerOptions.Converters.Add(new EnumConverterFactory());
                options.JsonSerializerOptions.Converters.Add(new NullableEnumConverterFactory());
                // 忽略只有在 .NET 6 才会存在的循环引用问题
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                // 解决 JSON 乱码问题
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            });
        });
    }
}