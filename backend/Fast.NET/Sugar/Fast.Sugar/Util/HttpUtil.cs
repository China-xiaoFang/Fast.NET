using System.Text.Json.Serialization;
using Fast.Cache;
using Fast.Core.DependencyInjection;
using Fast.UAParser;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Sugar.Util;

/// <summary>
/// Http 工具类
/// </summary>
public static class HttpUtil
{
    /// <summary>
    /// 得到UserAgent信息
    /// </summary>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    public static UserAgentInfoEntity GetUserAgentInfo(string userAgent)
    {
        try
        {
            var parser = Parser.GetDefault();
            var clientInfo = parser.Parse(userAgent);
            var result = new UserAgentInfoEntity
            {
                PhoneModel = clientInfo.Device.ToString(), OS = clientInfo.OS.ToString(), Browser = clientInfo.UA.ToString()
            };
            return result;
        }
        catch (Exception)
        {
            return new UserAgentInfoEntity();
        }
    }

    /// <summary>
    /// 获取操作系统版本
    /// </summary>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    public static string GetOSVersion(string userAgent)
    {
        var osVersion = string.Empty;
        if (userAgent.Contains("NT 10"))
        {
            osVersion = "Windows 10";
        }
        else if (userAgent.Contains("NT 6.3"))
        {
            osVersion = "Windows 8";
        }
        else if (userAgent.Contains("NT 6.1"))
        {
            osVersion = "Windows 7";
        }
        else if (userAgent.Contains("NT 6.0"))
        {
            osVersion = "Windows Vista/Server 2008";
        }
        else if (userAgent.Contains("NT 5.2"))
        {
            osVersion = "Windows Server 2003";
        }
        else if (userAgent.Contains("NT 5.1"))
        {
            osVersion = "Windows XP";
        }
        else if (userAgent.Contains("NT 5"))
        {
            osVersion = "Windows 2000";
        }
        else if (userAgent.Contains("NT 4"))
        {
            osVersion = "Windows NT4";
        }
        else if (userAgent.Contains("Android"))
        {
            osVersion = "Android";
        }
        else if (userAgent.Contains("Me"))
        {
            osVersion = "Windows Me";
        }
        else if (userAgent.Contains("98"))
        {
            osVersion = "Windows 98";
        }
        else if (userAgent.Contains("95"))
        {
            osVersion = "Windows 95";
        }
        else if (userAgent.Contains("Mac"))
        {
            osVersion = "Mac";
        }
        else if (userAgent.Contains("Unix"))
        {
            osVersion = "UNIX";
        }
        else if (userAgent.Contains("Linux"))
        {
            osVersion = "Linux";
        }
        else if (userAgent.Contains("SunOS"))
        {
            osVersion = "SunOS";
        }

        return osVersion;
    }

    /// <summary>
    /// 根据IP地址获取公网信息
    /// 不传值默认获取服务器的公网信息
    /// </summary>
    /// <returns></returns>
    public static WhoisIPInfoEntity WanInfo(string ip = null)
    {
        return WanInfoAsync(ip).Result;
    }

    /// <summary>
    /// 根据IP地址获取公网信息
    /// 不传值默认获取服务器的公网信息
    /// 带缓存
    /// </summary>
    /// <returns></returns>
    public static async Task<WhoisIPInfoEntity> WanInfoAsync(string ip = null)
    {
        WhoisIPInfoEntity result = null;

        // 如果IP为空，则默认获取服务器的公网信息
        if (string.IsNullOrEmpty(ip))
        {
            ip = "localhost";
        }

        var ipInfoCacheKey = $"IpInfo:{ip}";

        // 创建作用域
        await Scoped.CreateAsync(async (_, scope) =>
        {
            var services = scope.ServiceProvider;

            var _cache = services.GetService<ICache>();

            if (_cache == null)
            {
                // 直接获取
                result = await WanInfoAsync(ip);
            }
            else
            {
                // 从缓存中获取，获取失败，则直接获取
                result = await _cache.GetAndSetAsync(ipInfoCacheKey, TimeSpan.FromHours(12), async () => await WanInfoAsync(ip));
            }
        });

        return result;
    }

    ///// <summary>
    ///// 根据IP地址获取公网信息
    ///// 不传值默认获取服务器的公网信息
    ///// </summary>
    ///// <returns></returns>
    //public static WhoisIPInfoEntity WanInfo(string ip = null)
    //{
    //    return WanInfoAsync(ip).Result;
    //}

    ///// <summary>
    ///// 根据IP地址获取公网信息
    ///// 不传值默认获取服务器的公网信息
    ///// </summary>
    ///// <returns></returns>
    //public static async Task<WhoisIPInfoEntity> WanInfoAsync(string ip = null)
    //{
    //    var url = "http://whois.pconline.com.cn/ipJson.jsp";

    //    // 如果IP为空，则默认获取服务器的公网信息
    //    if (string.IsNullOrEmpty(ip))
    //    {
    //        ip = "localhost";
    //    }

    //    url += $"?ip={ip}";

    //    using var client = new HttpClient();
    //    try
    //    {
    //        var response = await client.GetAsync(url);
    //        response.EnsureSuccessStatusCode();

    //        var responseBody = await response.Content.ReadAsStringAsync();
    //        responseBody = responseBody[(responseBody.IndexOf("IPCallBack(", StringComparison.Ordinal) + "IPCallBack(".Length)..]
    //            .TrimEnd();
    //        responseBody = responseBody[..^3];
    //        return JsonSerializer.Deserialize<WhoisIPInfoEntity>(responseBody);
    //    }
    //    catch (HttpRequestException ex)
    //    {
    //        throw new HttpRequestException($"Get ip Info request error，{ex.Message}");
    //    }
    //}
}

/// <summary>
/// 万网Ip信息Model类
/// </summary>
public class WhoisIPInfoEntity
{
    /// <summary>
    /// Ip地址
    /// </summary>
    public string Ip { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    public string Pro { get; set; }

    /// <summary>
    /// 省份邮政编码
    /// </summary>
    public string ProCode { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// 城市邮政编码
    /// </summary>
    public string CityCode { get; set; }

    /// <summary>
    /// 地理信息
    /// </summary>
    [JsonPropertyName("addr")]
    public string Address { get; set; }

    /// <summary>
    /// 运营商
    /// </summary>
    public string Operator => Address[(Pro.Length + City.Length)..].Trim();
}

/// <summary>
/// UserAgent 信息Model类
/// </summary>
public class UserAgentInfoEntity
{
    /// <summary>
    /// 手机型号
    /// </summary>
    public string PhoneModel { get; set; }

    /// <summary>
    /// 操作系统（版本）
    /// </summary>
    public string OS { get; set; }

    /// <summary>
    /// 浏览器（版本）
    /// </summary>
    public string Browser { get; set; }
}