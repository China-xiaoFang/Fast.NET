// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
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

namespace Fast.Serialization.Extensions;

/// <summary>
/// Json 扩展
/// </summary>
public static class SerializationExtension
{
    /// <summary>
    /// JSON 字符串转 Object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public static T ToObject<T>(this string json)
    {
        json = json.Replace("&nbsp;", "");
        return JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    /// JSON 字符串转 Object
    /// </summary>
    /// <param name="json"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object ToObject(this string json, Type type)
    {
        json = json.Replace("&nbsp;", "");
        return JsonSerializer.Deserialize(json, type);
    }

    /// <summary>
    /// Object 转 JSON字符串
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ToJsonString(this object obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    /// <summary>
    /// Dictionary 字符串转 Object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    public static T ToObject<T>(this IDictionary<string, object> dictionary)
    {
        return dictionary.ToJsonString().ToObject<T>();
    }

    /// <summary>
    /// Dictionary 字符串转 Object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object ToObject<T>(this IDictionary<string, object> dictionary, Type type)
    {
        return dictionary.ToJsonString().ToObject(type);
    }

    /// <summary>
    /// 深度拷贝
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T DeepCopy<T>(this T source)
    {
        return ReferenceEquals(source, null) ? default : JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(source));
    }
}