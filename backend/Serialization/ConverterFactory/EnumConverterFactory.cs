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
using Fast.Serialization.JsonConverter.Internal;

namespace Fast.Serialization.ConverterFactory;

/// <summary>
/// <see cref="EnumConverterFactory"/> Enum 类型Json转换工厂
/// </summary>
internal class EnumConverterFactory : JsonConverterFactory
{
    /// <summary>When overridden in a derived class, determines whether the converter instance can convert the specified object type.</summary>
    /// <param name="typeToConvert">The type of the object to check whether it can be converted by this converter instance.</param>
    /// <returns>
    /// <see langword="true" /> if the instance can convert the specified object type; otherwise, <see langword="false" />.</returns>
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsEnum;
    }

    /// <summary>Creates a converter for a specified type.</summary>
    /// <param name="typeToConvert">The type handled by the converter.</param>
    /// <param name="options">The serialization options to use.</param>
    /// <returns>A converter for which <see cref="Enum"/> is compatible with <paramref name="typeToConvert" />.</returns>
    public override System.Text.Json.Serialization.JsonConverter CreateConverter(Type typeToConvert,
        JsonSerializerOptions options)
    {
        var converterType = typeof(EnumJsonConverter<>).MakeGenericType(typeToConvert);
        return (System.Text.Json.Serialization.JsonConverter) Activator.CreateInstance(converterType);
    }
}

/// <summary>
/// <see cref="NullableEnumConverterFactory"/> Enum 类型Json转换工厂
/// </summary>
internal class NullableEnumConverterFactory : JsonConverterFactory
{
    /// <summary>When overridden in a derived class, determines whether the converter instance can convert the specified object type.</summary>
    /// <param name="typeToConvert">The type of the object to check whether it can be converted by this converter instance.</param>
    /// <returns>
    /// <see langword="true" /> if the instance can convert the specified object type; otherwise, <see langword="false" />.</returns>
    public override bool CanConvert(Type typeToConvert)
    {
        if (typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return Nullable.GetUnderlyingType(typeToConvert)?.IsEnum == true;
        }

        return false;
    }

    /// <summary>Creates a converter for a specified type.</summary>
    /// <param name="typeToConvert">The type handled by the converter.</param>
    /// <param name="options">The serialization options to use.</param>
    /// <returns>A converter for which <see cref="Enum"/> is compatible with <paramref name="typeToConvert" />.</returns>
    public override System.Text.Json.Serialization.JsonConverter CreateConverter(Type typeToConvert,
        JsonSerializerOptions options)
    {
        var enumType = Nullable.GetUnderlyingType(typeToConvert);
        var converterType = typeof(NullableEnumJsonConverter<>).MakeGenericType(enumType);
        return (System.Text.Json.Serialization.JsonConverter) Activator.CreateInstance(converterType);
    }
}