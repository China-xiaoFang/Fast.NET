namespace Hello.NET.Core.BaseFactory;

/// <summary>
/// 记录表通用Model
/// </summary>
public class RecordBaseModel : DBEntityTenant
{
    /// <summary>
    /// 手机型号
    /// </summary>
    [SugarColumn(ColumnDescription = "手机型号", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string PhoneModel { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    [SugarColumn(ColumnDescription = "操作系统", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string OS { get; set; }

    /// <summary>
    /// 浏览器
    /// </summary>
    [SugarColumn(ColumnDescription = "浏览器", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string Browser { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    [SugarColumn(ColumnDescription = "省份", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string Province { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    [SugarColumn(ColumnDescription = "城市", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string City { get; set; }

    /// <summary>
    /// 运营商
    /// </summary>
    [SugarColumn(ColumnDescription = "运营商", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string Operator { get; set; }

    /// <summary>
    /// Ip
    /// </summary>
    [SugarColumn(ColumnDescription = "Ip", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string Ip { get; set; }

    /// <summary>
    /// 记录表创建
    /// </summary>
    public void RecordCreate()
    {
        var userAgentInfo = HttpNewUtil.UserAgentInfo();
        var wanInfo = HttpNewUtil.WanInfo(HttpNewUtil.Ip).Result;

        PhoneModel = userAgentInfo.PhoneModel;
        OS = userAgentInfo.OS;
        Browser = userAgentInfo.Browser;
        Province = wanInfo.Pro;
        City = wanInfo.City;
        Operator = wanInfo.Operator;
        Ip = wanInfo.Ip;
        Create();
    }
}

public class DRecordBaseModel : DEntityBase
{
    /// <summary>
    /// 手机型号
    /// </summary>
    [SugarColumn(ColumnDescription = "手机型号", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string PhoneModel { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    [SugarColumn(ColumnDescription = "操作系统", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string OS { get; set; }

    /// <summary>
    /// 浏览器
    /// </summary>
    [SugarColumn(ColumnDescription = "浏览器", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string Browser { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    [SugarColumn(ColumnDescription = "省份", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string Province { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    [SugarColumn(ColumnDescription = "城市", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string City { get; set; }

    /// <summary>
    /// 运营商
    /// </summary>
    [SugarColumn(ColumnDescription = "运营商", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string Operator { get; set; }

    /// <summary>
    /// Ip
    /// </summary>
    [SugarColumn(ColumnDescription = "Ip", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string Ip { get; set; }

    /// <summary>
    /// 记录表创建
    /// </summary>
    public void RecordCreate()
    {
        var userAgentInfo = HttpNewUtil.UserAgentInfo();
        var wanInfo = HttpNewUtil.WanInfo(HttpNewUtil.Ip).Result;

        PhoneModel = userAgentInfo.PhoneModel;
        OS = userAgentInfo.OS;
        Browser = userAgentInfo.Browser;
        Province = wanInfo.Pro;
        City = wanInfo.City;
        Operator = wanInfo.Operator;
        Ip = wanInfo.Ip;
        Create();
    }
}