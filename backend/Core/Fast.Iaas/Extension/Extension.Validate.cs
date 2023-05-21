namespace Fast.ServiceCollection.Extension;

/// <summary>
/// 验证扩展类
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// 检查 Object 是否为 NULL
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsEmpty(this object value)
    {
        return value == null || string.IsNullOrEmpty(value.ParseToString());
    }

    /// <summary>
    /// 检查 Object 是否为 NULL 或者 0
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNullOrZero(this object value)
    {
        return value == null || value.ParseToString().Trim() == "0";
    }
}