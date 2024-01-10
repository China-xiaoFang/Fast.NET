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

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fast.Serialization.JsonConverter.Internal;

/// <summary>
/// <see cref="ExceptionJsonConverter"/> Exception 类型Json返回处理
/// <remarks>解决 <see cref="Exception"/> 类型不能被正常序列化和反序列化操作</remarks>
/// </summary>
internal class ExceptionJsonConverter : JsonConverter<Exception>
{
    /// <summary>Determines whether the specified type can be converted.</summary>
    /// <param name="typeToConvert">The type to compare against.</param>
    /// <returns>
    /// <see langword="true" /> if the type can be converted; otherwise, <see langword="false" />.</returns>
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(Exception).IsAssignableFrom(typeToConvert);
    }

    /// <summary>Reads and converts the JSON to type <see cref="Exception"/>.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override Exception Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 反序列化异常是不允许的。
        throw new NotSupportedException("Deserializing exceptions is not allowed.");
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, Exception value, JsonSerializerOptions options)
    {
        // 获取可序列化的属性，排除 TargetSite 属性
        var serializableProperties = value.GetType().GetProperties().Select(sl => new {sl.Name, Value = sl.GetValue(value)})
            .Where(wh => wh.Name != nameof(Exception.TargetSite));

        // 如果设置了 DefaultIgnoreCondition 为 JsonIgnoreCondition.WhenWritingNull，则过滤掉值为 Null 的属性
        if (options?.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull)
        {
            serializableProperties = serializableProperties.Where(wh => wh.Value != null);
        }

        var propList = serializableProperties.ToList();

        // 判断是否还存在可以序列化的属性
        if (propList.Count == 0)
        {
            return;
        }

        // 开始写入对象
        writer.WriteStartObject();

        foreach (var prop in propList)
        {
            // 写入属性名
            writer.WritePropertyName(prop.Name);
            // 使用 JsonSerializer 序列化属性值
            JsonSerializer.Serialize(writer, prop.Value, options);
        }

        // 结束写入对象
        writer.WriteEndObject();
    }
}