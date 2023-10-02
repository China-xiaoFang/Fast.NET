using Fast.Exception.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Exception.Extensions;

/// <summary>
/// 友好异常服务拓展类
/// </summary>
public static class ExceptionIServiceCollectionExtension
{
    /// <summary>
    /// 添加友好异常服务拓展服务
    /// </summary>
    /// <param name="mvcBuilder"><see cref="IMvcBuilder"/>Mvc构建器</param>
    /// <returns><see cref="IMvcBuilder"/></returns>
    public static IMvcBuilder AddFriendlyException(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.Services.AddFriendlyException();

        return mvcBuilder;
    }

    /// <summary>
    /// 添加友好异常服务拓展服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddFriendlyException(this IServiceCollection services)
    {
        services.Configure<MvcOptions>(options => { options.Filters.Add<ExceptionFilter>(); });

        return services;
    }
}