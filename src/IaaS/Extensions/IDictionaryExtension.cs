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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="IDictionary{TKey,TValue}"/> 拓展类
/// </summary>
public static class IDictionaryExtension
{
    /// <summary>
    /// 将一个字典转化为 QueryString
    /// </summary>
    /// <param name="dict"><see cref="IDictionary{TKey,TValue}"/></param>
    /// <param name="urlEncode"></param>
    /// <param name="isToLower">首字母是否小写</param>
    /// <returns><see cref="string"/></returns>
    public static string ToQueryString(this IDictionary<string, string> dict, bool urlEncode = true, bool isToLower = false)
    {
        return string.Join("&",
            dict.Select(p =>
                $"{(urlEncode ? isToLower ? p.Key?.FirstCharToLower().UrlEncode() : p.Key?.UrlEncode() : "")}={(urlEncode ? p.Value?.UrlEncode() : "")}"));
    }

    /// <summary>
    /// 移除空值项
    /// </summary>
    /// <param name="dict"><see cref="IDictionary{TKey,TValue}"/></param>
    public static void RemoveEmptyValueItems(this IDictionary<string, string> dict)
    {
        dict.Where(item => string.IsNullOrEmpty(item.Value))
            .Select(item => item.Key)
            .ToList()
            .ForEach(key => { dict.Remove(key); });
    }

    /// <summary>
    /// 添加或更新
    /// </summary>
    /// <typeparam name="TKey">字典键类型</typeparam>
    /// <typeparam name="TValue">字典值类型</typeparam>
    /// <param name="dictionary"><see cref="IDictionary{TKey, TValue}"/></param>
    /// <param name="key"><typeparamref name="TKey"/></param>
    /// <param name="value"><typeparamref name="TValue"/></param>
    public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
        where TKey : notnull
    {
        // 空检查
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        // 检查键是否存在
        if (!dictionary.TryGetValue(key, out var values))
        {
            values = new List<TValue>();
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
    public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary,
        IDictionary<TKey, List<TValue>> concatDictionary) where TKey : notnull
    {
        // 空检查
        if (concatDictionary is null)
        {
            throw new ArgumentNullException(nameof(concatDictionary));
        }

        // 逐条遍历合并更新
        foreach (var (key, newValues) in concatDictionary)
        {
            // 检查键是否存在
            if (!dictionary.TryGetValue(key, out var values))
            {
                values = new List<TValue>();
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
    public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> concatDictionary) where TKey : notnull
    {
        // 空检查
        if (concatDictionary is null)
        {
            throw new ArgumentNullException(nameof(concatDictionary));
        }

        // 逐条遍历合并更新
        foreach (var (key, value) in concatDictionary)
        {
            // 检查键是否存在
            // ReSharper disable once RedundantDictionaryContainsKeyBeforeAdding
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

    /// <summary>
    /// 合并两个字典
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dic"><see cref="IDictionary{TKey,TValue}"/>字典</param>
    /// <param name="newDic"><see cref="IDictionary{TKey,TValue}"/>新字典</param>
    /// <returns><see cref="IDictionary{TKey,TValue}"/></returns>
    public static IDictionary<string, T> AddOrUpdate<T>(this IDictionary<string, T> dic, IDictionary<string, T> newDic)
    {
        foreach (var key in newDic.Keys)
        {
            if (dic.TryGetValue(key, out var value))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, newDic[key]);
            }
        }

        return dic;
    }


    /// <summary>
    /// 将Dic字典转换成字符串
    /// </summary>
    /// <param name="dic"><see cref="IDictionary{TKey,TValue}"/></param>
    /// <returns><see cref="string"/></returns>
    public static string DicToXmlStr(this IDictionary<string, object> dic)
    {
        var xml = "<xml>";
        foreach (var (key, value) in dic)
        {
            if (value is int)
                xml += "<" + key + ">" + value + "</" + key + ">";
            else if (value is string)
                xml += "<" + key + ">" + "<![CDATA[" + value + "]]></" + key + ">";
        }

        xml += "</xml>";
        return xml;
    }

    /// <summary>
    /// 将字符串转换为Dic字典
    /// </summary>
    /// <param name="xml"><see cref="string"/></param>
    /// <returns><see cref="IDictionary{TKey,TValue}"/></returns>
    public static IDictionary<string, object> XmlStrToDic(this string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            throw new Exception("不能转换空字符串！");
        }

        var rltDic = new Dictionary<string, object>();
        var xmlDoc = new XmlDocument {XmlResolver = null};
        xmlDoc.LoadXml(xml);
        var xmlNode = xmlDoc.FirstChild; //获取到根节点<xml>
        if (xmlNode != null)
        {
            var nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                var xe = (XmlElement) xn;
                rltDic[xe.Name] = xe.InnerText; //获取xml的键值对到WxPayData内部的数据中
            }
        }

        return rltDic;
    }

    /// <summary>
    /// 将Dic字典转换成字符串
    /// </summary>
    /// <param name="dic"><see cref="IDictionary{TKey,TValue}"/></param>
    /// <returns><see cref="string"/></returns>
    public static string SortDicToXmlStr(this SortedDictionary<string, object> dic)
    {
        var xml = "<xml>";
        foreach (var (key, value) in dic)
        {
            if (value is int)
                xml += "<" + key + ">" + value + "</" + key + ">";
            else if (value is string)
                xml += "<" + key + ">" + "<![CDATA[" + value + "]]></" + key + ">";
        }

        xml += "</xml>";
        return xml;
    }

    /// <summary>
    /// 将字符串转换为Dic字典
    /// </summary>
    /// <param name="xml"><see cref="string"/></param>
    /// <returns><see cref="SortedDictionary{TKey,TValue}"/></returns>
    public static SortedDictionary<string, object> XmlStrToSortDic(this string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            throw new Exception("不能转换空字符串！");
        }

        var rltDic = new SortedDictionary<string, object>();
        var xmlDoc = new XmlDocument {XmlResolver = null};
        xmlDoc.LoadXml(xml);
        var xmlNode = xmlDoc.FirstChild; //获取到根节点<xml>
        if (xmlNode != null)
        {
            var nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                var xe = (XmlElement) xn;
                rltDic[xe.Name] = xe.InnerText; //获取xml的键值对到WxPayData内部的数据中
            }
        }

        return rltDic;
    }
}