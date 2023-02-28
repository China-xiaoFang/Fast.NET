namespace Fast.Core.Internal.Options;

/// <summary>
/// 版权信息配置
/// </summary>
public class CopyrightInfoOptions
{
    /// <summary>
    /// 备案编号
    /// </summary>
    public string ICPCode { get; set; }

    /// <summary>
    /// 备案Url
    /// </summary>
    public string ICPUrl { get; set; }

    /// <summary>
    /// 公安备案编号
    /// </summary>
    public string PublicCode { get; set; }

    /// <summary>
    /// 公安备案Url
    /// </summary>
    public string PublicUrl { get; set; }

    /// <summary>
    /// 版权信息
    /// </summary>
    public string CRCode { get; set; }

    /// <summary>
    /// 版权Url
    /// </summary>
    public string CRUrl { get; set; }
}