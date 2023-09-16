using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Fast.Iaas.Extension
{
    
    /// <summary>
    /// 字典扩展
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// 将一个字典转化为 QueryString
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="urlEncode"></param>
        /// <param name="isToLower">首字母是否小写</param>
        /// <returns></returns>
        public static string ToQueryString(this IDictionary<string, string> dict, bool urlEncode = true, bool isToLower = false)
        {
            return string.Join("&",
                dict.Select(p =>
                    $"{(urlEncode ? isToLower ? p.Key?.FirstCharToLower().UrlEncode() : p.Key?.UrlEncode() : "")}={(urlEncode ? p.Value?.UrlEncode() : "")}"));
        }

        /// <summary>
        /// 将一个字符串 URL 编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlEncode(this string str)
        {
            return string.IsNullOrEmpty(str) ? "" : HttpUtility.UrlEncode(str, Encoding.UTF8);
        }

        /// <summary>
        /// 移除空值项
        /// </summary>
        /// <param name="dict"></param>
        public static void RemoveEmptyValueItems(this IDictionary<string, string> dict)
        {
            dict.Where(item => string.IsNullOrEmpty(item.Value)).Select(item => item.Key).ToList().ForEach(key =>
            {
                dict.Remove(key);
            });
        }

        /// <summary>
        /// 将一个Url 编码 转为字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlDecode(this string str)
        {
            return string.IsNullOrEmpty(str) ? "" : HttpUtility.UrlDecode(str, Encoding.UTF8);
        }
    /// <summary>
    /// 添加或更新
    /// </summary>
    /// <typeparam name="TKey">字典键类型</typeparam>
    /// <typeparam name="TValue">字典值类型</typeparam>
    /// <param name="dictionary"><see cref="IDictionary{TKey, TValue}"/></param>
    /// <param name="key"><typeparamref name="TKey"/></param>
    /// <param name="value"><typeparamref name="TValue"/></param>
    public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary
        , TKey key
        , TValue value)
        where TKey : notnull
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(value);

        // 检查键是否存在
        if (!dictionary.TryGetValue(key, out var values))
        {
            values = new();
            dictionary.Add(key, values);
        }

        values.Add(value);
    }

    /// <summary>
    /// 添加或更新
    /// </summary>
    /// <typeparam name="TKey">字典键类型</typeparam>
    /// <typeparam name="TValue">字典值类型</typeparam>
    /// <param name="dictionary"><see cref="IDictionary{TKey, TValue}"/></param>
    /// <param name="concatDictionary"><see cref="IDictionary{TKey, TValue}"/></param>
    public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, IDictionary<TKey, List<TValue>> concatDictionary)
         where TKey : notnull
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(concatDictionary);

        // 逐条遍历合并更新
        foreach (var (key, newValues) in concatDictionary)
        {
            // 检查键是否存在
            if (!dictionary.TryGetValue(key, out var values))
            {
                values = new();
                dictionary.Add(key, values);
            }

            values.AddRange(newValues);
        }
    }

    /// <summary>
    /// 添加或更新
    /// </summary>
    /// <typeparam name="TKey">字典键类型</typeparam>
    /// <typeparam name="TValue">字典值类型</typeparam>
    /// <param name="dictionary"><see cref="IDictionary{TKey, TValue}"/></param>
    /// <param name="concatDictionary"><see cref="IDictionary{TKey, TValue}"/></param>
    public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> concatDictionary)
         where TKey : notnull
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(concatDictionary);

        // 逐条遍历合并更新
        foreach (var (key, value) in concatDictionary)
        {
            // 检查键是否存在
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
    }
    }
}
