// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Serialization;

/// <summary>
/// System.Text.Json 序列化上下文
/// </summary>
public static class JsonContext
{
    /// <summary>
    /// System.Text.Json 选项
    /// </summary>
    internal static Action<JsonOptions> JsonOptionsAction =>
        options =>
        {
            // 两套 JSON 实现使用同一默认格式，确保切换序列化器后日期协议保持一致
            const string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter(dateTimeFormat));
            options.JsonSerializerOptions.Converters.Add(new NullableDateTimeJsonConverter(dateTimeFormat));

            options.JsonSerializerOptions.Converters.Add(new DateTimeOffsetJsonConverter(dateTimeFormat));
            options.JsonSerializerOptions.Converters.Add(new NullableDateTimeOffsetJsonConverter(dateTimeFormat));

            // 将 64 位整数按约定格式输出，避免 JavaScript 数值精度丢失
            options.JsonSerializerOptions.Converters.Add(new LongJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableLongJsonConverter());

            options.JsonSerializerOptions.Converters.Add(new IntJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableIntJsonConverter());

            options.JsonSerializerOptions.Converters.Add(new DecimalJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableDecimalJsonConverter());

            options.JsonSerializerOptions.Converters.Add(new DoubleJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableDoubleJsonConverter());

            // Exception 包含反射成员和循环引用，使用专用转换器输出安全的属性集合
            options.JsonSerializerOptions.Converters.Add(new ExceptionJsonConverter());

            // 同时接受枚举名称和数值输入
            options.JsonSerializerOptions.Converters.Add(new EnumConverterFactory());
            options.JsonSerializerOptions.Converters.Add(new NullableEnumConverterFactory());

            // 忽略对象图中的循环引用
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

            // UnsafeRelaxedJsonEscaping 保留中文等非 ASCII 字符；输出内容仍需由调用方按目标上下文进行 HTML 转义
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

            options.JsonSerializerOptions.AllowTrailingCommas = true;
            options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        };

    /// <summary>
    /// System.Text.Json 序列化选项
    /// </summary>
    public static JsonSerializerOptions SerializerOptions { get; internal set; }
}
