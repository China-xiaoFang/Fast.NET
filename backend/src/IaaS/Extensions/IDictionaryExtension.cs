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

using System.Xml;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="IDictionary{TKey,TValue}"/> 拓展类
/// </summary>
[SuppressSniffer]
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
        dict.Where(item => string.IsNullOrEmpty(item.Value)).Select(item => item.Key).ToList().ForEach(key =>
        {
            dict.Remove(key);
        });
    }

    /// <summary>
    /// 添加或更新
    /// </summary>
    /// <typeparam name="TKey">字典键类型</typeparam>
    /// <typeparam name="TValue">字典值类型</typeparam>
    /// <param name="dictionary"><see cref="IDictionary{TKey, TValue}"/></param>
    /// <param name="key"><typeparamref name="TKey"/></param>
    /// <param name="value"><typeparamref name="TValue"/></param>
    public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
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
    public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary,
        IDictionary<TKey, List<TValue>> concatDictionary) where TKey : notnull
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
    public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> concatDictionary) where TKey : notnull
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
        if (string.IsNullOrEmpty(xml))
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
        if (string.IsNullOrEmpty(xml))
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