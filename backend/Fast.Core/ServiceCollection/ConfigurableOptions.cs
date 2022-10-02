namespace Fast.Core.ServiceCollection;

/// <summary>
/// 可配置选项
/// 一般用于读取JSON文件中的数据
/// </summary>
public static class ConfigurableOptions
{
    /// <summary>
    /// 添加可配置选项
    /// 读取JSON数据信息
    /// </summary>
    /// <param name="service"></param>
    public static void AddConfigurableOptions(this IServiceCollection service)
    {
        // Database config.
        service.AddConfigurableOptions<ConnectionStringsOptions>();
        // Cache config.
        service.AddConfigurableOptions<CacheOptions>();
        // System config.
        service.AddConfigurableOptions<SystemSettingsOptions>();
        // Upload file config.
        service.AddConfigurableOptions<UploadFileOptions>();
    }
}