namespace Fast.IaaS.Utils;

/// <summary>
/// Guid 工具类
/// </summary>
public static class GuidUtil
{
    /// <summary>
    /// 生成一个Guid
    /// N ece4f4a60b764339b94a07c84e338a27
    /// D 5bf99df1-dc49-4023-a34a-7bd80a42d6bb
    /// B {2280f8d7-fd18-4c72-a9ab-405de3fcfbc9}
    /// P (25e6e09f-fb66-4cab-b4cd-bfb429566549)
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public static string GetGuid(string format = "N")
    {
        return Guid.NewGuid().ToString(format);
    }

    /// <summary>
    /// 生成一个短的Guid
    /// </summary>
    /// <returns></returns>
    public static string GetShortGuid()
    {
        var i = Guid.NewGuid().ToByteArray().Aggregate<byte, long>(1, (current, b) => current * (b + 1));

        return $"{i - DateTime.Now.Ticks:x}";
    }
}