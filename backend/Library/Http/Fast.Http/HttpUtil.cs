using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Fast.Cache;
using Fast.Core;
using Fast.Core.DependencyInjection;
using Fast.UAParser;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Http;

/// <summary>
/// HTTP网络工具
/// </summary>
public static class HttpUtil
{
    /// <summary>
    /// 根据IP地址获取公网信息
    /// 不传值默认获取服务器的公网信息
    /// 带缓存
    /// </summary>
    /// <returns></returns>
    public static WhoisIPInfoEntity WanInfoCache(string ip = null)
    {
        return WanInfoCacheAsync(ip).Result;
    }

    /// <summary>
    /// 根据IP地址获取公网信息
    /// 不传值默认获取服务器的公网信息
    /// 带缓存
    /// </summary>
    /// <returns></returns>
    public static async Task<WhoisIPInfoEntity> WanInfoCacheAsync(string ip = null)
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
    /// </summary>
    /// <returns></returns>
    public static async Task<WhoisIPInfoEntity> WanInfoAsync(string ip = null)
    {
        var url = "http://whois.pconline.com.cn/ipJson.jsp";

        // 如果IP为空，则默认获取服务器的公网信息
        if (string.IsNullOrEmpty(ip))
        {
            ip = "localhost";
        }

        url += $"?ip={ip}";

        using var client = new HttpClient();
        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody = responseBody[(responseBody.IndexOf("IPCallBack(", StringComparison.Ordinal) + "IPCallBack(".Length)..]
                .TrimEnd();
            responseBody = responseBody[..^3];
            return JsonSerializer.Deserialize<WhoisIPInfoEntity>(responseBody);
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Get ip Info request error，{ex.Message}");
        }
    }

    private static readonly char[] reserveChar = {'/', '?', '*', ':', '|', '\\', '<', '>', '\"'};

    /// <summary>
    /// 远程路径Encode处理,会保证开头是/，结尾也是/
    /// </summary>
    /// <param name="remotePath"></param>
    /// <returns></returns>
    public static string EncodeRemotePath(string remotePath)
    {
        if (remotePath == "/")
        {
            return remotePath;
        }

        var endWith = remotePath.EndsWith("/");
        var part = remotePath.Split('/');
        remotePath = "";
        foreach (var s in part)
        {
            if (s == "")
                continue;
            if (remotePath != "")
            {
                remotePath += "/";
            }

            remotePath += HttpUtility.UrlEncode(s).Replace("+", "%20");
        }

        remotePath = (remotePath.StartsWith("/") ? "" : "/") + remotePath + (endWith ? "/" : "");
        return remotePath;
    }

    /// <summary>
    /// 标准化远程目录路径,会保证开头是/，结尾也是/ ,如果命名不规范，存在保留字符，会返回空字符
    /// </summary>
    /// <param name="remotePath">要标准化的远程路径</param>
    /// <returns></returns>
    public static string StandardizationRemotePath(string remotePath)
    {
        if (string.IsNullOrEmpty(remotePath))
        {
            return "";
        }

        if (!remotePath.StartsWith("/"))
        {
            remotePath = "/" + remotePath;
        }

        if (!remotePath.EndsWith("/"))
        {
            remotePath = remotePath + "/";
        }

        var index1 = 1;
        while (index1 < remotePath.Length)
        {
            var index2 = remotePath.IndexOf('/', index1);
            if (index2 == index1)
            {
                return "";
            }

            var folderName = remotePath.Substring(index1, index2 - index1);
            if (folderName.IndexOfAny(reserveChar) != -1)
            {
                return "";
            }

            index1 = index2 + 1;
        }

        return remotePath;
    }
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
public class UserAgentInfoModel
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