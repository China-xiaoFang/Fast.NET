namespace Fast.Core.ServiceCollection;

/// <summary>
/// 雪花Id
/// </summary>
public static class SnowflakeId
{
    /// <summary>
    /// 添加雪花Id
    /// 需要在JSON文件中配置获取传入WorkerId
    /// </summary>
    /// <param name="services"></param>
    /// <param name="snowIdWorkerId"></param>
    public static void AddSnowflakeId(this IServiceCollection services, string snowIdWorkerId = "1")
    {
        // 设置雪花Id的workerId，确保每个实例workerId都应不同
        var workerId = ushort.Parse(Configuration["SnowId:WorkerId"] ?? snowIdWorkerId);
        YitIdHelper.SetIdGenerator(new IdGeneratorOptions {WorkerId = workerId});
    }
}