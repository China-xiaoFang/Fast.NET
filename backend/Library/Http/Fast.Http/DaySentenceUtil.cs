using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fast.Http;

/// <summary>
/// 每日一句工具类
/// </summary>
public static class DaySentenceUtil
{
    private static readonly string _getDaySentenceUrl = "http://open.iciba.com/dsapi/";

    /// <summary>
    /// 得到每日一句
    /// </summary>
    /// <returns></returns>
    public static async Task<DaySentenceOutPut> GetDaySentence()
    {
        using var client = new HttpClient();
        try
        {
            var response = await client.GetAsync(_getDaySentenceUrl);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DaySentenceOutPut>(responseBody);
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Day sentence request error，{ex.Message}");
        }
    }
}

public class DaySentenceOutPut
{
    /// <summary>
    /// 图片2
    /// </summary>
    public string Picture2 { get; set; }

    /// <summary>
    /// 说明
    /// </summary>
    public string Caption { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// 英文内容
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 分享图片
    /// </summary>
    [JsonPropertyName("fenxiang_img")]
    public string shareImg { get; set; }

    /// <summary>
    /// 时间
    /// </summary>
    [JsonPropertyName("dateline")]
    public DateTime DateTime { get; set; }
}