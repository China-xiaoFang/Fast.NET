using System.ComponentModel.DataAnnotations;

namespace Fast.Core.Util.Captcha.General;

/// <summary>
/// 常规验证码输入
/// </summary>
[SuppressSniffer]
public class GeneralCaptchaInput
{
    /// <summary>
    /// 验证码类型
    /// </summary>
    [Required(ErrorMessage = "验证码类型")]
    public string CaptchaType { get; set; }

    /// <summary>
    /// 验证码字符
    /// </summary>
    [Required(ErrorMessage = "验证码字符不能为空")]
    public string CaptchaCode { get; set; }

    /// <summary>
    /// Token
    /// </summary>
    public string Token { get; set; }
}