namespace Fast.IaaS.Extensions;

/// <summary>
/// <see cref="decimal"/> 拓展类
/// </summary>
public static class DecimalExtension
{
    /// <summary>
    /// 得到百分比
    /// </summary>
    /// <param name="data"><see cref="decimal"/></param>
    /// <returns><see cref="string"/></returns>
    public static string GetPercentage(this decimal data)
    {
        var result = data * 100;
        return result == 100 ? "100%" : $"{Math.Round(result, 2)}%";
    }

    /// <summary>
    /// 得到百分比
    /// </summary>
    /// <param name="num1"><see cref="decimal"/></param>
    /// <param name="num2"><see cref="decimal"/></param>
    /// <returns><see cref="string"/></returns>
    public static string GetPercentage(this decimal num1, decimal num2)
    {
        var result = num2 == 0 ? 0 : Math.Round(num1 / num2, 4);
        return result.GetPercentage();
    }

    /// <summary>
    /// 获取 decimal，小数点后面有几位就保留几位
    /// </summary>
    /// <param name="data"><see cref="decimal"/></param>
    /// <returns><see cref="decimal"/></returns>
    public static decimal GetDecimal(this decimal data)
    {
        return (decimal) (double) data;
    }
}