// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Fast.Serialization;

/// <summary>
/// Newtonsoft.Json 序列化上下文
/// </summary>
public static class JsonContext
{
    /// <summary>
    /// Newtonsoft.Json 选项
    /// </summary>
    internal static Action<MvcNewtonsoftJsonOptions> JsonOptionsAction =>
        options =>
        {
            // 两套 JSON 实现使用同一默认格式，确保切换序列化器后日期协议保持一致
            const string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            options.SerializerSettings.Converters.Add(new DateTimeJsonConverter(dateTimeFormat));
            options.SerializerSettings.Converters.Add(new NullableDateTimeJsonConverter(dateTimeFormat));

            options.SerializerSettings.Converters.Add(new DateTimeOffsetJsonConverter(dateTimeFormat));
            options.SerializerSettings.Converters.Add(new NullableDateTimeOffsetJsonConverter(dateTimeFormat));

            // 将 64 位整数按约定格式输出，避免 JavaScript 数值精度丢失
            options.SerializerSettings.Converters.Add(new LongJsonConverter());
            options.SerializerSettings.Converters.Add(new NullableLongJsonConverter());

            options.SerializerSettings.Converters.Add(new IntJsonConverter());
            options.SerializerSettings.Converters.Add(new NullableIntJsonConverter());

            options.SerializerSettings.Converters.Add(new DecimalJsonConverter());
            options.SerializerSettings.Converters.Add(new NullableDecimalJsonConverter());

            options.SerializerSettings.Converters.Add(new DoubleJsonConverter());
            options.SerializerSettings.Converters.Add(new NullableDoubleJsonConverter());

            // Exception 包含反射成员和循环引用，使用专用转换器输出安全的属性集合
            options.SerializerSettings.Converters.Add(new ExceptionJsonConverter());

            // 同时接受枚举名称和数值输入
            options.SerializerSettings.Converters.Add(new EnumJsonConverter());

            // 忽略对象图中的循环引用
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            // 保留非 ASCII 字符，避免中文被转义为 Unicode 序列
            options.SerializerSettings.StringEscapeHandling = StringEscapeHandling.Default;

            // 默认日期格式
            options.SerializerSettings.DateFormatString = dateTimeFormat;
        };

    /// <summary>
    /// Newtonsoft.Json 序列化选项
    /// </summary>
    public static JsonSerializerSettings SerializerOptions { get; internal set; }
}
