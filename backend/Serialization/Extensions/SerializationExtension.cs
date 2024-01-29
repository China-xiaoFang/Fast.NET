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

using System.Collections;
using System.Text.Json;
using Fast.IaaS;

namespace Fast.Serialization.Extensions;

/// <summary>
/// <see cref="SerializationExtension"/> 序列化拓展类
/// </summary>
[SuppressSniffer]
public static class SerializationExtension
{
    /// <summary>
    /// JSON 字符串转 Object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"><see cref="string"/> 需要序列化的 JSON 字符串</param>
    /// <returns></returns>
    public static T ToObject<T>(this string json)
    {
        json = json.Replace("&nbsp;", "");
        return JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    /// JSON 字符串转 Object
    /// </summary>
    /// <param name="json"><see cref="string"/> 需要序列化的 JSON 字符串</param>
    /// <param name="type"><see cref="Type"/> 需要序列化成的类型</param>
    /// <returns><see cref="object"/> 序列化后的对象</returns>
    public static object ToObject(this string json, Type type)
    {
        json = json.Replace("&nbsp;", "");
        return JsonSerializer.Deserialize(json, type);
    }

    /// <summary>
    /// Object 转 JSON字符串
    /// </summary>
    /// <param name="obj"><see cref="object"/> 需要反序列化的对象</param>
    /// <returns><see cref="string"/> 反序列化后的 JSON 字符串</returns>
    public static string ToJsonString(this object obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    /// <summary>
    /// Dictionary 字符串转 Object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dictionary"><see cref="IDictionary"/> 需要序列化的字典</param>
    /// <returns></returns>
    public static T ToObject<T>(this IDictionary<string, object> dictionary)
    {
        return dictionary.ToJsonString().ToObject<T>();
    }

    /// <summary>
    /// Dictionary 字符串转 Object
    /// </summary>
    /// <param name="dictionary"><see cref="IDictionary"/> 需要序列化的字典</param>
    /// <param name="type"><see cref="Type"/> 需要序列化成的类型</param>
    /// <returns><see cref="object"/> 序列化后的对象</returns>
    public static object ToObject(this IDictionary<string, object> dictionary, Type type)
    {
        return dictionary.ToJsonString().ToObject(type);
    }

    /// <summary>
    /// 深度拷贝
    /// </summary>
    /// <remarks>此方法是通过将对象序列化成 JSON 字符串，再将 JSON 字符串反序列化成对象，所以性能不是很高，如果介意，请慎用</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">需要拷贝的对象</param>
    /// <returns></returns>
    public static T DeepCopy<T>(this T source)
    {
        return source is null ? default : JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(source));
    }
}