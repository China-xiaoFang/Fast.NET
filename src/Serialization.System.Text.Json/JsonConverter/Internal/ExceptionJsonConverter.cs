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

using System.Text.Json;
using System.Text.Json.Serialization;

// ReSharper disable once CheckNamespace
namespace Fast.Serialization;

/// <summary>
/// <see cref="ExceptionJsonConverter"/> Exception 类型Json返回处理
/// </summary>
/// <remarks>解决 <see cref="Exception"/> 类型不能被正常序列化和反序列化操作</remarks>
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
        // 默认只写入 Message，Source，StackTrace，InnerException
        var writeNameArr = new[]
        {
            nameof(Exception.Message), nameof(Exception.Source), nameof(Exception.StackTrace),
            nameof(Exception.InnerException)
        };
        // 获取可序列化的属性，排除 TargetSite 属性
        var serializableProperties = value.GetType().GetProperties().Select(sl => new {sl.Name, Value = sl.GetValue(value)})
            .Where(wh => writeNameArr.Contains(wh.Name));

        // 如果设置了 DefaultIgnoreCondition 为 JsonIgnoreCondition.WhenWritingNull，则过滤掉值为 Null 的属性
        if (options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull)
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

    ///// <summary>Writes a specified value as JSON.</summary>
    ///// <param name="writer">The writer to write to.</param>
    ///// <param name="value">The value to convert to JSON.</param>
    ///// <param name="options">An object that specifies serialization options to use.</param>
    //public override void Write(Utf8JsonWriter writer, Exception value, JsonSerializerOptions options)
    //{
    //    // 获取可序列化的属性，排除 TargetSite 属性
    //    var serializableProperties = value.GetType().GetProperties().Select(sl => new {sl.Name, Value = sl.GetValue(value)})
    //        .Where(wh => wh.Name != nameof(Exception.TargetSite));

    //    // 如果设置了 DefaultIgnoreCondition 为 JsonIgnoreCondition.WhenWritingNull，则过滤掉值为 Null 的属性
    //    if (options?.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull)
    //    {
    //        serializableProperties = serializableProperties.Where(wh => wh.Value != null);
    //    }

    //    var propList = serializableProperties.ToList();

    //    // 判断是否还存在可以序列化的属性
    //    if (propList.Count == 0)
    //    {
    //        return;
    //    }

    //    // 开始写入对象
    //    writer.WriteStartObject();

    //    foreach (var prop in propList)
    //    {
    //        // 写入属性名
    //        writer.WritePropertyName(prop.Name);
    //        // 使用 JsonSerializer 序列化属性值
    //        JsonSerializer.Serialize(writer, prop.Value, options);
    //    }

    //    // 结束写入对象
    //    writer.WriteEndObject();
    //}
}