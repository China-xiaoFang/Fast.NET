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

using System.Collections;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Fast.Serialization;

/// <summary>
/// <see cref="SerializationExtension"/> Newtonsoft.Json 拓展类
/// </summary>
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
        return JsonConvert.DeserializeObject<T>(json, JsonContext.SerializerOptions);
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
        return JsonConvert.DeserializeObject(json, type, JsonContext.SerializerOptions);
    }

    /// <summary>
    /// Object 转 JSON字符串
    /// </summary>
    /// <param name="obj"><see cref="object"/> 需要反序列化的对象</param>
    /// <returns><see cref="string"/> 反序列化后的 JSON 字符串</returns>
    public static string ToJsonString(this object obj)
    {
        return JsonConvert.SerializeObject(obj, JsonContext.SerializerOptions);
    }

    /// <summary>
    /// Dictionary 字符串转 Object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dictionary"><see cref="IDictionary"/> 需要序列化的字典</param>
    /// <returns></returns>
    public static T ToObject<T>(this IDictionary<string, object> dictionary)
    {
        return dictionary.ToJsonString()
            .ToObject<T>();
    }

    /// <summary>
    /// Dictionary 字符串转 Object
    /// </summary>
    /// <param name="dictionary"><see cref="IDictionary"/> 需要序列化的字典</param>
    /// <param name="type"><see cref="Type"/> 需要序列化成的类型</param>
    /// <returns><see cref="object"/> 序列化后的对象</returns>
    public static object ToObject(this IDictionary<string, object> dictionary, Type type)
    {
        return dictionary.ToJsonString()
            .ToObject(type);
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
        return source is null
            ? default
            : source.ToJsonString()
                .ToObject<T>();
    }
}