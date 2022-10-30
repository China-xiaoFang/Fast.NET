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
        // System config.
        SysGlobalContext.SystemSettingsOptions = GetConfig<SystemSettingsOptions>("SystemSettings");
        // Upload file config.
        SysGlobalContext.UploadFileOptions = GetConfig<UploadFileOptions>("UploadFile");

        // Check
        if (SysGlobalContext.SystemSettingsOptions.Environment.IsNullOrZero())
            throw Oops.Oh(ErrorCode.ConfigError);
    }
}