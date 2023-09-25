using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Core.Extensions;

/// <summary>
/// <see cref="IServiceCollection"/> 拓展类
/// </summary>
public static class IServiceCollectionExtension
{
    /// <summary>
    /// 添加Gzip Brotli 压缩
    /// </summary>
    /// <param name="service"></param>
    internal static void AddGzipBrotliCompression(this IServiceCollection service)
    {
        service.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
        service.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
        service.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
            {
                "text/html; charset=utf-8", "application/xhtml+xml", "application/atom+xml", "image/svg+xml"
            });
        });
    }

    /// <summary>
    /// 注册 Mvc 过滤器
    /// </summary>
    /// <typeparam name="TFilter"></typeparam>
    /// <param name="mvcBuilder"><see cref="IMvcBuilder"/></param>
    /// <param name="configure"></param>
    /// <returns><see cref="IMvcBuilder"/></returns>
    public static IMvcBuilder AddMvcFilter<TFilter>(this IMvcBuilder mvcBuilder, Action<MvcOptions> configure = default)
        where TFilter : IFilterMetadata
    {
        mvcBuilder.Services.AddMvcFilter<TFilter>(configure);

        return mvcBuilder;
    }

    /// <summary>
    /// 注册 Mvc 过滤器
    /// </summary>
    /// <typeparam name="TFilter"></typeparam>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="configure"></param>
    /// <returns>&lt;see cref="IServiceCollection"/&gt;</returns>
    public static IServiceCollection AddMvcFilter<TFilter>(this IServiceCollection services,
        Action<MvcOptions> configure = default) where TFilter : IFilterMetadata
    {
        services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add<TFilter>();

            // 其他额外配置
            configure?.Invoke(options);
        });

        return services;
    }

    /// <summary>
    /// 注册 Mvc 过滤器
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="filter"></param>
    /// <param name="configure"></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddMvcFilter(this IServiceCollection services, IFilterMetadata filter,
        Action<MvcOptions> configure = default)
    {
        services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add(filter);

            // 其他额外配置
            configure?.Invoke(options);
        });

        return services;
    }
}