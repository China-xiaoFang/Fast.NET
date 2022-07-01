namespace Hello.NET.Infrastructure.Util.Dto;

/// <summary>
/// 枚举的Entity类
/// </summary>
public class EnumEntity
{
    /// <summary>  
    /// 枚举的描述  
    /// </summary>  
    public string Describe { set; get; }

    /// <summary>  
    /// 枚举名称  
    /// </summary>  
    public string Name { set; get; }

    /// <summary>  
    /// 枚举对象的值  
    /// </summary>  
    public int Value { set; get; }
}

/// <summary>
/// 小程序登录解密后的用户信息Model
/// </summary>
public class TelUserInfoModel
{
    /// <summary>
    /// 手机号
    /// </summary>
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; }

    /// <summary>
    /// 纯手机号
    /// </summary>
    [JsonPropertyName("purePhoneNumber")]
    public string PurePhoneNumber { get; set; }

    /// <summary>
    /// 区号
    /// </summary>
    [JsonPropertyName("countryCode")]
    public string CountryCode { get; set; }
}