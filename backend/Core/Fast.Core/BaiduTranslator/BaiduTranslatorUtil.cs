using Fast.Core.BaiduTranslator.Dto;
using Fast.Core.Const;
using Fast.Core.Operation.Config;
using Furion.RemoteRequest.Extensions;

namespace Fast.Core.BaiduTranslator;

/// <summary>
/// 百度翻译工具类
/// </summary>
public static class BaiduTranslatorUtil
{
    /// <summary>
    /// 百度翻译开发者AppId
    /// </summary>
    private static string AppId => ConfigOperation.System.GetConfig(ConfigConst.BaiduTranslator.AppId).Value;

    /// <summary>
    /// 百度翻译开发者密钥
    /// </summary>
    private static string SecretKey => ConfigOperation.System.GetConfig(ConfigConst.BaiduTranslator.SecretKey).Value;

    /// <summary>
    /// 百度翻译请求Url
    /// </summary>
    private static string Url => ConfigOperation.System.GetConfig(ConfigConst.BaiduTranslator.Url).Value;

    /// <summary>
    /// 自动检测
    /// </summary>
    public static string Auto => "auto";

    /// <summary>
    /// 中文
    /// </summary>
    public static string CN => "zh";

    /// <summary>
    /// 英文
    /// </summary>
    public static string EN => "en";

    /// <summary>
    /// 百度翻译
    /// </summary>
    /// <param name="content">文本</param>
    /// <param name="from">来源语言</param>
    /// <param name="to">翻译语言</param>
    /// <returns></returns>
    public static BaiduTranslatorResultDto Translator(string content, string from, string to)
    {
        return TranslatorAsync(content, from, to).Result;
    }

    /// <summary>
    /// 百度翻译
    /// </summary>
    /// <param name="content">文本</param>
    /// <param name="from">来源语言</param>
    /// <param name="to">翻译语言</param>
    /// <returns></returns>
    public static async Task<BaiduTranslatorResultDto> TranslatorAsync(string content, string from, string to)
    {
        // 随机数，时间戳
        var salt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        // 签名
        var sign = $"{AppId}{content}{salt}{SecretKey}".GenerateMd5();
        // 参数
        var parameter = new Dictionary<string, string>
        {
            {"q", content},
            {"from", from},
            {"to", to},
            {"appid", AppId},
            {"salt", salt},
            {"sign", sign}
        };
        // Get请求请求翻译
        return await Url.SetBody(parameter, "application/x-www-form-urlencoded").PostAsAsync<BaiduTranslatorResultDto>();
    }
}