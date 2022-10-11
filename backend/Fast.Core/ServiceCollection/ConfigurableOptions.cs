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
        GlobalContext.ConnectionStringsOptions = GetConfig<ConnectionStringsOptions>("ConnectionStrings");
        // Cache config.
        GlobalContext.CacheOptions = GetConfig<CacheOptions>("Cache");
        // System config.
        GlobalContext.SystemSettingsOptions = GetConfig<SystemSettingsOptions>("SystemSettings");
        // Upload file config.
        GlobalContext.UploadFileOptions = GetConfig<UploadFileOptions>("UploadFile");

        // Check
        if (GlobalContext.SystemSettingsOptions.Environment.IsNullOrZero())
            throw Oops.Oh(ErrorCode.ConfigError);
    }
}