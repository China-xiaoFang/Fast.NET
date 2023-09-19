namespace Fast.Extensions;

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
    /// 检查 Object 或者 集合 是否为 NULL 或者 空集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsEmpty<T>(this T value)
    {
        if (value == null)
        {
            return true;
        }

        if (string.IsNullOrEmpty(value.ParseToString()))
        {
            return true;
        }

        var type = typeof(T);

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            if (!(value is IList<object> list) || list.Count == 0)
            {
                return true;
            }

            return false;
        }

        if (value is IEnumerable<T> collection && !collection.Any())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 检查 Object 是否为 NULL 或者 0
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNullOrZero(this object value)
    {
        if (value == null)
        {
            return true;
        }

        // 判断是否为枚举类型
        if (value.GetType().IsEnum)
        {
            return value.ParseToLong() == 0;
        }

        return value.ParseToString().Trim() == "0";
    }
}