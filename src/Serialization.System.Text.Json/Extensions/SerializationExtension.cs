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

namespace Fast.Serialization;

/// <summary>
/// 提供 System.Text.Json 序列化扩展方法。
/// </summary>
public static class SerializationExtension
{
    /// <summary>
    /// 将 JSON 文本反序列化为对象。
    /// </summary>
    /// <param name="json">要解析的 JSON 文本。</param>
    /// <typeparam name="T">序列化或转换后的对象类型。</typeparam>
    /// <returns>转换得到的对象。</returns>
    public static T ToObject<T>(this string json)
    {
        ArgumentNullException.ThrowIfNull(json);
        json = json.Replace("&nbsp;", "", StringComparison.Ordinal);
        return JsonSerializer.Deserialize<T>(json, JsonContext.SerializerOptions);
    }

    /// <summary>
    /// 将 JSON 文本反序列化为对象。
    /// </summary>
    /// <param name="json">要解析的 JSON 文本。</param>
    /// <param name="type">目标类型。</param>
    /// <returns>转换得到的对象。</returns>
    public static object ToObject(this string json, Type type)
    {
        ArgumentNullException.ThrowIfNull(json);
        ArgumentNullException.ThrowIfNull(type);
        json = json.Replace("&nbsp;", "", StringComparison.Ordinal);
        return JsonSerializer.Deserialize(json, type, JsonContext.SerializerOptions);
    }

    /// <summary>
    /// 将对象序列化为 JSON 文本。
    /// </summary>
    /// <param name="obj">要处理的对象。</param>
    /// <returns>序列化后的 JSON 文本。</returns>
    public static string ToJsonString(this object obj)
    {
        return JsonSerializer.Serialize(obj, JsonContext.SerializerOptions);
    }

    /// <summary>
    /// 将字典数据转换为对象。
    /// </summary>
    /// <param name="dictionary">要处理的字典。</param>
    /// <typeparam name="T">序列化或转换后的对象类型。</typeparam>
    /// <returns>转换得到的对象。</returns>
    public static T ToObject<T>(this IDictionary<string, object> dictionary)
    {
        return dictionary.ToJsonString()
            .ToObject<T>();
    }

    /// <summary>
    /// 将字典数据转换为对象。
    /// </summary>
    /// <param name="dictionary">要处理的字典。</param>
    /// <param name="type">目标类型。</param>
    /// <returns>转换得到的对象。</returns>
    public static object ToObject(this IDictionary<string, object> dictionary, Type type)
    {
        return dictionary.ToJsonString()
            .ToObject(type);
    }

    /// <summary>
    /// 通过 JSON 序列化创建对象的深层副本。
    /// </summary>
    /// <remarks>该方法通过一次序列化和反序列化创建副本，开销高于成员复制，且只保留可序列化的数据。</remarks>
    /// <param name="source">需要拷贝的对象。</param>
    /// <typeparam name="T">要复制的对象类型。</typeparam>
    /// <returns>创建的对象副本。</returns>
    public static T DeepCopy<T>(this T source)
    {
        return source is null
            ? default
            : source.ToJsonString()
                .ToObject<T>();
    }
}
