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

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Fast.Serialization;

/// <summary>
/// <see cref="JsonContext"/> Newtonsoft.Json 序列化上下文
/// </summary>
public static class JsonContext
{
    /// <summary>
    /// Newtonsoft.Json 选项
    /// </summary>
    internal static Action<MvcNewtonsoftJsonOptions> JsonOptionsAction =>
        options =>
        {
            // 这里其实可以改为从配置文件中读取，但是目前好像没必要，后续再看吧~~~
            const string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            options.SerializerSettings.Converters.Add(new DateTimeJsonConverter(dateTimeFormat));
            options.SerializerSettings.Converters.Add(new NullableDateTimeJsonConverter(dateTimeFormat));

            options.SerializerSettings.Converters.Add(new DateTimeOffsetJsonConverter(dateTimeFormat));
            options.SerializerSettings.Converters.Add(new NullableDateTimeOffsetJsonConverter(dateTimeFormat));

            // 解决 long 类型返回前端可能会导致精度丢失的问题
            options.SerializerSettings.Converters.Add(new LongJsonConverter());
            options.SerializerSettings.Converters.Add(new NullableLongJsonConverter());

            options.SerializerSettings.Converters.Add(new IntJsonConverter());
            options.SerializerSettings.Converters.Add(new NullableIntJsonConverter());

            options.SerializerSettings.Converters.Add(new DecimalJsonConverter());
            options.SerializerSettings.Converters.Add(new NullableDecimalJsonConverter());

            options.SerializerSettings.Converters.Add(new DoubleJsonConverter());
            options.SerializerSettings.Converters.Add(new NullableDoubleJsonConverter());

            // 解决 Exception 类型不能被正常序列化和反序列化的问题
            options.SerializerSettings.Converters.Add(new ExceptionJsonConverter());

            // 解决 Enum 类型前端传入 string 的问题
            options.SerializerSettings.Converters.Add(new EnumJsonConverter());

            // 忽略只有在.NET 6 才会存在的循环引用问题
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            // 解决 JSON 乱码问题
            options.SerializerSettings.StringEscapeHandling = StringEscapeHandling.Default;
            //options.SerializerSettings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;

            // 默认日期格式
            options.SerializerSettings.DateFormatString = dateTimeFormat;
        };

    /// <summary>
    /// Newtonsoft.Json 序列化选项
    /// </summary>
    public static JsonSerializerSettings SerializerOptions { get; internal set; }
}