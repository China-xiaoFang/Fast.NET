namespace Fast.Core.Util.SnowflakeId.Extension;

/// <summary>
/// 雪花id
/// </summary>
public static class SnowflakeIdServiceExtension
{
    /// <summary>
    /// 添加雪花Id
    /// </summary>
    /// <param name="services"></param>
    public static void AddSnowflakeId(this IServiceCollection services)
    {
        // 设置雪花Id的workerId，确保每个实例workerId都应不同
        var workerId = ushort.Parse(Configuration["SnowId:WorkerId"] ?? "1");
        YitIdHelper.SetIdGenerator(new IdGeneratorOptions {WorkerId = workerId});
    }
}