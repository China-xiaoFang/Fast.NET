namespace Fast.Iaas.Extension;

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
    public static string ToQueryString(this Dictionary<string, string> dict, bool urlEncode = true, bool isToLower = false)
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
    public static void RemoveEmptyValueItems(this Dictionary<string, string> dict)
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
}