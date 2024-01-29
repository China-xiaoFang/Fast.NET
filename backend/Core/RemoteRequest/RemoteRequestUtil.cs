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

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fast.NET.Core.RemoteRequest;

/// <summary>
/// 远程请求工具类
/// </summary>
internal static class RemoteRequestUtil
{
    // TODO：后期移动单独类库，或者直接集成到项目中
    /// <summary>
    /// 得到每日一句
    /// </summary>
    /// <returns></returns>
    public static async Task<DaySentenceEntity> GetDaySentence()
    {
        using var client = new HttpClient();
        try
        {
            var response = await client.GetAsync("http://open.iciba.com/dsapi/");
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DaySentenceEntity>(responseBody);
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Day sentence request error，{ex.Message}");
        }
    }

    ///// <summary>
    ///// 得到天气预报
    ///// </summary>
    ///// <param name="cityName"></param>
    ///// <returns></returns>
    //public static async Task<WeatherInfoEntity> GetWeatherInfo(string cityName = "长沙")
    //{
    //    //var weatherOutPut = await $"http://wthrcdn.etouch.cn/weather_mini?city={cityName}".SetClient("weatherCdn").GetAsAsync<WeatherOutPut>();
    //    //if (weatherOutPut.Status != 1000 || weatherOutPut.Desc != "OK")
    //    //{
    //    //    return new WeatherInfoOutPut {Success = false, Desc = weatherOutPut.Desc};
    //    //}

    //    //weatherOutPut.Data.Success = true;

    //    //return weatherOutPut.Data;
    //    throw new Exception("暂未实现！");
    //}
}

internal class DaySentenceEntity
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
    public string ShareImg { get; set; }

    /// <summary>
    /// 时间
    /// </summary>
    [JsonPropertyName("dateline")]
    public DateTime DateTime { get; set; }
}

public class WeatherInfoEntity
{
    /// <summary>
    /// 昨日天气
    /// </summary>
    public WeatherInfo Yesterday { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// 未来五天天气
    /// </summary>
    public List<WeatherInfo> Forecast { get; set; }

    /// <summary>
    /// 感冒
    /// </summary>
    public string Ganmao { get; set; }

    /// <summary>
    /// 温度
    /// </summary>
    public string Wendu { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 失败描述
    /// </summary>
    public string Desc { get; set; }


    /// <summary>
    /// 天气信息
    /// </summary>
    public class WeatherInfo
    {
        /// <summary>
        /// 时间
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// 最高温度
        /// </summary>
        public string High { get; set; }

        /// <summary>
        /// 风力
        /// </summary>
        public string Fengli { get; set; }

        /// <summary>
        /// 最低温度
        /// </summary>
        public string Low { get; set; }

        /// <summary>
        /// 分向
        /// </summary>
        public string Fengxiang { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
    }
}