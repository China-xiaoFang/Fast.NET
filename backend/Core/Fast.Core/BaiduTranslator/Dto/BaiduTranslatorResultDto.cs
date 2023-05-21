namespace Fast.Core.BaiduTranslator.Dto;

/// <summary>
/// 百度翻译返回结果类
/// </summary>
public class BaiduTranslatorResultDto
{
    /// <summary>
    /// 翻译源语言
    /// </summary>
    public string From { get; set; }

    /// <summary>
    /// 译文语言
    /// </summary>
    public string To { get; set; }

    /// <summary>
    /// 翻译结果集合
    /// 只需默认拿第一个就好
    /// </summary>
    public List<TranslatorResult> Trans_Result { get; set; }

    /// <summary>
    /// 错误码
    /// </summary>
    public int Error_Code { get; set; }
}

/// <summary>
/// 翻译结果类
/// </summary>
public class TranslatorResult
{
    /// <summary>
    /// 原文
    /// </summary>
    public string Src { get; set; }

    /// <summary>
    /// 译文
    /// </summary>
    public string Dst { get; set; }
}