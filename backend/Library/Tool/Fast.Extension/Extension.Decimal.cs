namespace Fast.Extensions;

/// <summary>
/// DateTime扩展
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// 得到百分比
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string GetPercentage(this decimal data)
    {
        var result = data * 100;
        return result == 100 ? "100%" : $"{Math.Round(result, 2)}%";
    }

    /// <summary>
    /// 得到百分比
    /// </summary>
    /// <param name="num1"></param>
    /// <param name="num2"></param>
    /// <returns></returns>
    public static string GetPercentage(this int num1, int num2)
    {
        var result = num2 == 0 ? 0 : Math.Round(num1.ParseToDecimal() / num2.ParseToDecimal(), 4);
        return result.GetPercentage();
    }

    /// <summary>
    /// 获取 decimal，小数点后面有几位就保留几位
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static decimal GetDecimal(this decimal data)
    {
        return (decimal) (double) data;
    }
}