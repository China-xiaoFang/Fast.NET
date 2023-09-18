namespace Fast.Sugar.BaseModel.Interface;

/// <summary>
/// IBaseRecordEntity 接口定义
/// </summary>
public interface IBaseLogEntity : IDbEntity
{
    /// <summary>
    /// 手机型号
    /// </summary>
    string PhoneModel { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    string OS { get; set; }

    /// <summary>
    /// 浏览器
    /// </summary>
    string Browser { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    string Province { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    string City { get; set; }

    /// <summary>
    /// 运营商
    /// </summary>
    string Operator { get; set; }

    /// <summary>
    /// Ip
    /// </summary>
    string Ip { get; set; }

    /// <summary>
    /// 记录表创建
    /// </summary>
    void RecordCreate();
}