using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fast.Util;

/// <summary>
/// 每日一句工具类
/// </summary>
public static class DaySentenceUtil
{
    /// <summary>
    /// 得到每日一句
    /// </summary>
    /// <returns></returns>
    public static async Task<DaySentenceOutPut> GetDaySentence()
    {
        var apiUrl = "http://open.iciba.com/dsapi/";

        using var client = new HttpClient();
        var response = await client.GetAsync(apiUrl);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Day Sentence Request failed with status code {response.StatusCode}");

        var responseData = await response.Content.ReadAsStringAsync();

        return ParseJsonToDto(responseData);
    }

    private static DaySentenceOutPut ParseJsonToDto(string jsonString)
    {
        var dto = new DaySentenceOutPut();

        var dictionary = ParseJsonToDictionary(jsonString);
        if (dictionary.TryGetValue("picture2", out var dtoPicture2))
        {
            dto.Picture2 = dtoPicture2;
        }

        if (dictionary.TryGetValue("caption", out var dtoCaption))
        {
            dto.Caption = dtoCaption;
        }

        if (dictionary.TryGetValue("note", out var dtoNote))
        {
            dto.Note = dtoNote;
        }

        if (dictionary.TryGetValue("content", out var dtoContent))
        {
            dto.Content = dtoContent;
        }

        if (dictionary.TryGetValue("fenxiang_img", out var dtoShareImg))
        {
            dto.shareImg = dtoShareImg;
        }

        if (dictionary.ContainsKey("dateline") && DateTime.TryParse(dictionary["dateline"], out var dtoDateTime))
        {
            dto.DateTime = dtoDateTime;
        }

        return dto;
    }

    private static Dictionary<string, string> ParseJsonToDictionary(string jsonString)
    {
        var dictionary = new Dictionary<string, string>();

        var index = 0;
        while (index < jsonString.Length)
        {
            // 查找属性名
            var propertyNameStartIndex = jsonString.IndexOf('"', index) + 1;
            var propertyNameEndIndex = jsonString.IndexOf('"', propertyNameStartIndex);
            var propertyName = jsonString.Substring(propertyNameStartIndex, propertyNameEndIndex - propertyNameStartIndex);

            // 查找属性值
            var propertyValueStartIndex = jsonString.IndexOf(':', propertyNameEndIndex) + 1;
            var propertyValueEndIndex = jsonString.IndexOfAny(new[] {',', '}'}, propertyValueStartIndex);
            if (propertyValueStartIndex < propertyValueEndIndex)
            {
                var propertyValue = jsonString.Substring(propertyValueStartIndex, propertyValueEndIndex - propertyValueStartIndex)
                    .Trim('"');
                dictionary[propertyName] = propertyValue;
            }

            index = propertyValueEndIndex + 1;
        }

        return dictionary;
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
    public string shareImg { get; set; }

    /// <summary>
    /// 时间
    /// </summary>
    public DateTime DateTime { get; set; }
}