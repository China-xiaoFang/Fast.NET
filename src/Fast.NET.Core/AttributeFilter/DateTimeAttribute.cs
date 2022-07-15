namespace Fast.NET.Core.AttributeFilter;

/// <summary>
/// DateTime属性
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DateTimeAttribute : Attribute
{
    /// <summary>
    /// 时间格式
    /// </summary>
    public string Format { get; set; } = "yyyy-MM-dd";

    /// <summary>
    /// 输出格式
    /// </summary>
    public string ResultFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";
}