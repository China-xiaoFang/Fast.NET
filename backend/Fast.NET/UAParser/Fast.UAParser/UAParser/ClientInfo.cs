// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
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

namespace Fast.UAParser.UAParser;

/// <summary>
/// Represents the user agent client information resulting from parsing
/// a user agent string
/// </summary>
public class ClientInfo : IUAParserOutput
{
    /// <summary>The user agent string, the input for the UAParser</summary>
    public string String { get; }

    /// <summary>The OS parsed from the user agent string</summary>
    public OS OS { get; }

    /// <summary>The Device parsed from the user agent string</summary>
    public Device Device { get; }

    /// <summary>The User Agent parsed from the user agent string</summary>
    [Obsolete("Mirrors the value of the UA property. Will be removed in future versions")]
    public UserAgent UserAgent => UA;

    /// <summary>The User Agent parsed from the user agent string</summary>
    public UserAgent UA { get; }

    /// <summary>
    /// Constructs an instance of the ClientInfo with results of the user agent string parsing
    /// </summary>
    public ClientInfo(string inputString, OS os, Device device, UserAgent userAgent)
    {
        String = inputString;
        OS = os;
        Device = device;
        UA = userAgent;
    }

    /// <summary>
    /// A readable description of the user agent client information
    /// </summary>
    /// <returns></returns>
    public override string ToString() => string.Format("{0} {1} {2}", OS, Device, UA);
}