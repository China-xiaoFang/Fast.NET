using Microsoft.Extensions.Configuration;

namespace Fast.Core.Extensions;

/// <summary>
/// <see cref="IConfiguration"/> 拓展类
/// </summary>
public static class IConfigurationExtension
{
    /// <summary>
    /// 刷新配置对象
    /// </summary>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    /// <returns><see cref="IConfiguration"/></returns>
    public static IConfiguration Reload(this IConfiguration configuration)
    {
        if (App.RootServices == null)
            return configuration;

        var newConfiguration = App.GetService<IConfiguration>(App.RootServices);
        InternalApp.Configuration = newConfiguration;

        return newConfiguration;
    }
}