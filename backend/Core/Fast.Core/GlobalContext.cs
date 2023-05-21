using Fast.Core.Const;
using Fast.Core.Options;
using Fast.Iaas.Extension;

namespace Fast.Core;

/// <summary>
/// 系统通用上下文
/// </summary>
public class GlobalContext
{
    /// <summary>
    /// 请求来源Url
    /// </summary>
    public static string OriginUrl => App.HttpContext.Request.Headers[CommonConst.Origin].ParseToString();

    /// <summary>
    /// 请求客户端UUID，唯一标识，不安全
    /// </summary>
    public static string UUID => App.HttpContext.Request.Headers[CommonConst.UUID].ParseToString();

    /// <summary>
    /// 系统配置
    /// </summary>
    public static SystemSettingsOptions SystemSettingsOptions { get; set; }

    /// <summary>
    /// 版权信息
    /// </summary>
    public static CopyrightInfoOptions CopyrightInfoOptions { get; set; }

    /// <summary>
    /// 上传文件配置
    /// </summary>
    public static UploadFileOptions UploadFileOptions { get; set; }
}