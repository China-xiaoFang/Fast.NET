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

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Serialization;

/// <summary>
/// System.Text.Json 序列化上下文。
/// </summary>
public static class JsonContext
{
    /// <summary>
    /// System.Text.Json 选项。
    /// </summary>
    internal static Action<JsonOptions> JsonOptionsAction =>
        options =>
        {
            // 两套 JSON 实现使用同一默认格式，确保切换序列化器后日期协议保持一致。
            const string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter(dateTimeFormat));
            options.JsonSerializerOptions.Converters.Add(new NullableDateTimeJsonConverter(dateTimeFormat));

            options.JsonSerializerOptions.Converters.Add(new DateTimeOffsetJsonConverter(dateTimeFormat));
            options.JsonSerializerOptions.Converters.Add(new NullableDateTimeOffsetJsonConverter(dateTimeFormat));

            // 将 64 位整数按约定格式输出，避免 JavaScript 数值精度丢失。
            options.JsonSerializerOptions.Converters.Add(new LongJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableLongJsonConverter());

            options.JsonSerializerOptions.Converters.Add(new IntJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableIntJsonConverter());

            options.JsonSerializerOptions.Converters.Add(new DecimalJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableDecimalJsonConverter());

            options.JsonSerializerOptions.Converters.Add(new DoubleJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableDoubleJsonConverter());

            // Exception 包含反射成员和循环引用，使用专用转换器输出安全的属性集合。
            options.JsonSerializerOptions.Converters.Add(new ExceptionJsonConverter());

            // 同时接受枚举名称和数值输入。
            options.JsonSerializerOptions.Converters.Add(new EnumConverterFactory());
            options.JsonSerializerOptions.Converters.Add(new NullableEnumConverterFactory());

            // 忽略对象图中的循环引用
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

            // UnsafeRelaxedJsonEscaping 保留中文等非 ASCII 字符；输出内容仍需由调用方按目标上下文进行 HTML 转义。
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

            options.JsonSerializerOptions.AllowTrailingCommas = true;
            options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        };

    /// <summary>
    /// System.Text.Json 序列化选项。
    /// </summary>
    public static JsonSerializerOptions SerializerOptions { get; internal set; }
}
