using Fast.Cache.Service;
using Fast.Cache.Service.Options;
using Fast.Core.Internal.Options;
using Furion.FriendlyException;
using Microsoft.Extensions.DependencyInjection;

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
        // Service collection config.
        GlobalContext.ServiceCollectionOptions = App.GetConfig<ServiceCollectionOptions>("ServiceCollection");
        // Cache config.
        CacheContext.CacheOptions = App.GetConfig<CacheOptions>("Cache");
        // System config.
        GlobalContext.SystemSettingsOptions = App.GetConfig<SystemSettingsOptions>("SystemSettings");
        // Copyright Info
        GlobalContext.CopyrightInfoOptions = App.GetConfig<CopyrightInfoOptions>("CopyrightInfo");
        // Upload file config.
        GlobalContext.UploadFileOptions = App.GetConfig<UploadFileOptions>("UploadFile");

        // Check
        if (GlobalContext.SystemSettingsOptions.Environment.IsNullOrZero())
            throw Oops.Oh("系统配置错误，请检查系统配置！");
    }
}