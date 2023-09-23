using System.Xml;

namespace Fast.IaaS.Extensions;

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