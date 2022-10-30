namespace Fast.Excel.AttributeFilter;

/// <summary>
/// MiniExcel 列验证
/// 默认验证是否为空，支持正则表达式验证
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class MiniExcelRequired : Attribute
{
    public MiniExcelRequired()
    {
    }

    public MiniExcelRequired(string ErrorMessage)
    {
        this.ErrorMessage = ErrorMessage;
    }

    public MiniExcelRequired(string ErrorMessage, string Regex)
    {
        this.ErrorMessage = ErrorMessage;
        this.Regex = Regex;
    }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// 正则表达式
    /// </summary>
    public string Regex { get; set; }
}