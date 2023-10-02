using System.Reflection;
using Fast.Core.ConfigurableOptions.Attributes;
using Fast.Core.ConfigurableOptions.Options;

namespace Fast.Core.ConfigurableOptions.Internal;

/// <summary>
/// 常量、公共方法配置类
/// </summary>
internal static class Penetrates
{
    /// <summary>
    /// 获取选项配置
    /// </summary>
    /// <param name="optionsType">选项类型</param>
    /// <returns></returns>
    internal static (OptionsSettingsAttribute, string) GetOptionsConfiguration(Type optionsType)
    {
        var optionsSettings = optionsType.GetCustomAttribute<OptionsSettingsAttribute>(false);

        // 默认后缀
        var defaultEnd = nameof(Options);

        return (optionsSettings, optionsSettings switch
        {
            // // 没有贴 [OptionsSettings]，如果选项类以 `Options` 结尾，则移除，否则返回类名称
            null => optionsType.Name.EndsWith(defaultEnd) ? optionsType.Name[..^defaultEnd.Length] : optionsType.Name,
            // 如果贴有 [OptionsSettings] 特性，但未指定 Path 参数，则直接返回类名，否则返回 Path
            _ => string.IsNullOrWhiteSpace(optionsSettings.Path) ? optionsType.Name : optionsSettings.Path,
        });
    }

    /// <summary>
    /// 在主机启动时获取选项
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <returns></returns>
    internal static TOptions GetOptionsOnStarting<TOptions>() where TOptions : class, new()
    {
        if (App.RootServices == null && typeof(IConfigurableOptions).IsAssignableFrom(typeof(TOptions)))
        {
            var (_, path) = GetOptionsConfiguration(typeof(TOptions));
            return App.GetConfig<TOptions>(path, true);
        }

        return null;
    }
}