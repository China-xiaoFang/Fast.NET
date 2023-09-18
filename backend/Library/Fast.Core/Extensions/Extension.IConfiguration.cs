using Fast.Core.App;
using Microsoft.Extensions.Configuration;

namespace Fast.Core.Extensions;

/// <summary>
/// IConfiguration 扩展
/// </summary>
// ReSharper disable once PartialTypeWithSinglePart
public static partial class Extensions
{
    /// <summary>
    /// 刷新配置对象
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IConfiguration Reload(this IConfiguration configuration)
    {
        if (App.App.RootServices == null)
            return configuration;

        var newConfiguration = App.App.GetService<IConfiguration>(App.App.RootServices);
        InternalApp.Configuration = newConfiguration;

        return newConfiguration;
    }
}