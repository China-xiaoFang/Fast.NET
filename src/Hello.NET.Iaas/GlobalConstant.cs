namespace Hello.NET.Iaas;

/// <summary>
/// 常用常量
/// </summary>
public class GlobalConstant
{
    /// <summary>
    /// 默认DateTime
    /// </summary>
    public static DateTime DefaultTime => TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);

    /// <summary>
    /// 时间戳
    /// </summary>
    public static long TimeStamp => Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
}