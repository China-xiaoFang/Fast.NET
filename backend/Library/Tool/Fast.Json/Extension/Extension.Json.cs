using System.Collections.Generic;
using System.Text.Json;

namespace Fast.Iaas.Extension
{
    /// <summary>
    /// Json 扩展
    /// </summary>
    // ReSharper disable once PartialTypeWithSinglePart
    public static partial class Extensions
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
}