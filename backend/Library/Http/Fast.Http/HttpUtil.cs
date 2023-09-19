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
    /// IP地址
    /// </summary>
    public static string Ip
    {
        get
        {
            // 尝试获取客户端的IP地址
            var result = ClientIp;
            if (string.IsNullOrEmpty(result))
            {
                // 没有获取到获取局域网的
                result = LanIp;
            }

            return string.IsNullOrEmpty(result) ? string.Empty : result;
        }
    }

    /// <summary>
    /// 客户端IP地址
    /// </summary>
    /// <returns></returns>
    private static string ClientIp
    {
        get
        {
            if (App.HttpContext == null)
                return string.Empty;
            if (App.HttpContext.Connection.RemoteIpAddress == null)
                return string.Empty;
            var ip = App.HttpContext.Connection.RemoteIpAddress.ToString();
            if (App.HttpContext.Request.Headers.TryGetValue("X-Real-IP", out var header1))
            {
                ip = header1.FirstOrDefault();
            }

            if (string.IsNullOrEmpty(ip) && App.HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var header2))
            {
                ip = header2.FirstOrDefault();
            }

            if (ip == null)
                return string.Empty;

            foreach (var hostAddress in Dns.GetHostAddresses(ip))
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    return hostAddress.ToString();
                }
            }

            return string.Empty;

            // 第二种方式
            //return context.Connection.RemoteIpAddress?.MapToIPv4()?.ToString();
        }
    }

    /// <summary>
    /// 本机IP地址
    /// </summary>
    public static string LocalIp => App.HttpContext?.Connection.LocalIpAddress?.MapToIPv4()?.ToString();

    /// <summary>
    /// 局域网IP地址
    /// </summary>
    public static string LanIp
    {
        get
        {
            foreach (var hostAddress in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    return hostAddress.ToString();
                }
            }

            return string.Empty;
        }
    }

    /// <summary>
    /// 完整请求地址
    /// </summary>
    public static string RequestUrlAddress
    {
        get
        {
            var request = App.HttpContext?.Request;

            if (request != null)
            {
                return new StringBuilder().Append(request.Scheme).Append("://").Append(request.Host).Append(request.PathBase)
                    .Append(request.Path).Append(request.QueryString).ToString();
            }

            return string.Empty;
        }
    }

    /// <summary>
    /// 来源地址
    /// 默认从 HTTP Header['Referer'] 中获取
    /// </summary>
    public static string RefererUrlAddress
    {
        get
        {
            var request = App.HttpContext?.Request;

            return request != null ? request.Headers["Referer"].ToString() : string.Empty;
        }
    }

    /// <summary>
    /// 是否是 WebSocket 请求
    /// </summary>
    public static bool IsWebSocketRequest =>
        App.HttpContext?.WebSockets?.IsWebSocketRequest ?? false || App.HttpContext?.Request?.Path == "/ws";

    /// <summary>
    /// 请求UserAgent信息
    /// </summary>
    public static string UserAgent
    {
        get
        {
            string userAgent = App.HttpContext?.Request.Headers["User-Agent"];

            return userAgent;
        }
    }

    /// <summary>
    /// UserAgent信息
    /// </summary>
    /// <returns></returns>
    public static UserAgentInfoModel UserAgentInfo()
    {
        try
        {
            var parser = Parser.GetDefault();
            var clientInfo = parser.Parse(UserAgent);
            var result = new UserAgentInfoModel
            {
                PhoneModel = clientInfo.Device.ToString(), OS = clientInfo.OS.ToString(), Browser = clientInfo.UA.ToString()
            };
            return result;
        }
        catch (Exception)
        {
            return new UserAgentInfoModel();
        }
    }

    /// <summary>
    /// 得到操作系统版本
    /// </summary>
    /// <returns></returns>
    public static string OSVersion
    {
        get
        {
            var osVersion = string.Empty;
            var userAgent = UserAgent;
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
    }

    /// <summary>
    /// 根据IP地址获取公网信息
    /// 不传值默认获取服务器的公网信息
    /// 带缓存
    /// </summary>
    /// <returns></returns>
    public static WhoisIPInfoModel WanInfoCache(string ip = null)
    {
        return WanInfoCacheAsync(ip).Result;
    }

    /// <summary>
    /// 根据IP地址获取公网信息
    /// 不传值默认获取服务器的公网信息
    /// 带缓存
    /// </summary>
    /// <returns></returns>
    public static async Task<WhoisIPInfoModel> WanInfoCacheAsync(string ip = null)
    {
        WhoisIPInfoModel result = null;

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
    public static WhoisIPInfoModel WanInfo(string ip = null)
    {
        return WanInfoAsync(ip).Result;
    }

    /// <summary>
    /// 根据IP地址获取公网信息
    /// 不传值默认获取服务器的公网信息
    /// </summary>
    /// <returns></returns>
    public static async Task<WhoisIPInfoModel> WanInfoAsync(string ip = null)
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
            return JsonSerializer.Deserialize<WhoisIPInfoModel>(responseBody);
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
public class WhoisIPInfoModel
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