using Fast.Core.ConfigurableOptions.Extensions;
using Fast.Core.Extensions;
using Fast.Core.FriendlyException.Extensions.Options;
using Fast.Core.FriendlyException.Filters;
using Fast.Core.FriendlyException.Options;
using Fast.Core.FriendlyException.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Core.FriendlyException.Extensions;

/// <summary>
/// 友好异常服务拓展类
/// </summary>
public static class FriendlyExceptionServiceCollectionExtensions
{
    /// <summary>
    /// 添加友好异常服务拓展服务
    /// </summary>
    /// <typeparam name="TErrorCodeTypeProvider">异常错误码提供器</typeparam>
    /// <param name="mvcBuilder">Mvc构建器</param>
    /// <param name="configure">是否启用全局异常过滤器</param>
    /// <returns></returns>
    public static IMvcBuilder AddFriendlyException<TErrorCodeTypeProvider>(this IMvcBuilder mvcBuilder,
        Action<FriendlyExceptionOptions> configure = null) where TErrorCodeTypeProvider : class, IErrorCodeTypeProvider
    {
        mvcBuilder.Services.AddFriendlyException<TErrorCodeTypeProvider>(configure);

        return mvcBuilder;
    }

    /// <summary>
    /// 添加友好异常服务拓展服务
    /// </summary>
    /// <typeparam name="TErrorCodeTypeProvider">异常错误码提供器</typeparam>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddFriendlyException<TErrorCodeTypeProvider>(this IServiceCollection services,
        Action<FriendlyExceptionOptions> configure = null) where TErrorCodeTypeProvider : class, IErrorCodeTypeProvider
    {
        // 添加全局异常过滤器
        services.AddFriendlyException(configure);

        // 单例注册异常状态码提供器
        services.AddSingleton<IErrorCodeTypeProvider, TErrorCodeTypeProvider>();

        return services;
    }

    /// <summary>
    /// 添加友好异常服务拓展服务
    /// </summary>
    /// <param name="mvcBuilder">Mvc构建器</param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IMvcBuilder AddFriendlyException(this IMvcBuilder mvcBuilder, Action<FriendlyExceptionOptions> configure = null)
    {
        mvcBuilder.Services.AddFriendlyException(configure);

        return mvcBuilder;
    }

    /// <summary>
    /// 添加友好异常服务拓展服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddFriendlyException(this IServiceCollection services,
        Action<FriendlyExceptionOptions> configure = null)
    {
        // 添加友好异常配置文件支持
        services.AddConfigurableOptions<FriendlyExceptionSettingsOptions>();

        // 添加异常配置文件支持
        services.AddConfigurableOptions<ErrorCodeMessageSettingsOptions>();

        // 载入服务配置选项
        var configureOptions = new FriendlyExceptionOptions();
        configure?.Invoke(configureOptions);

        // 添加全局异常过滤器
        if (configureOptions.GlobalEnabled)
            services.AddMvcFilter<FriendlyExceptionFilter>();

        return services;
    }
}