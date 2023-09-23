using System.IO.Compression;
using System.Reflection;
using Fast.Core.Diagnostics;
using Fast.Core.Reflection;
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
        // 非 Web 环境跳过注册
        if (App.WebHostEnvironment == default)
            return services;

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
        // 非 Web 环境跳过注册
        if (App.WebHostEnvironment == default)
            return services;

        services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add(filter);

            // 其他额外配置
            configure?.Invoke(options);
        });

        return services;
    }

    /// <summary>
    /// 添加JSON序列化配置
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    internal static IServiceCollection AddJsonOptions(this IServiceCollection services)
    {
        // 判断是否安装了 Json 程序集
        var cacheAssembly = App.Assemblies.FirstOrDefault(f => f.GetName().Name?.Equals("Fast.Serialization") == true);
        if (cacheAssembly != null)
        {
            Debugging.Info("正在注册 JSON 序列化配置......");
            // 加载 Cache 拓展类和拓展方法
            var cacheIServiceCollectionExtensionType = Reflect.GetType(cacheAssembly,
                "Fast.Serialization.Extensions.SerializationIServiceCollectionExtension");
            var addCacheMethod = cacheIServiceCollectionExtensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(f => f.Name == "AddJsonOptions");

            return addCacheMethod.Invoke(null, new object[] {services}) as IServiceCollection;
        }

        return services;
    }

    /// <summary>
    /// 添加对象映射
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    internal static IServiceCollection AddObjectMapper(this IServiceCollection services)
    {
        // 判断是否安装了 Mapster 程序集
        var objectMapperAssembly = App.Assemblies.FirstOrDefault(u => u.GetName().Name?.Equals("Fast.Mapster") == true);
        if (objectMapperAssembly != null)
        {
            Debugging.Info("正在注册 Mapster 对象映射......");
            // 加载 ObjectMapper 拓展类型和拓展方法
            var objectMapperServiceCollectionExtensionsType =
                Reflect.GetType(objectMapperAssembly, "Fast.Mapster.Extensions.ObjectMapperExtension");
            var addObjectMapperMethod = objectMapperServiceCollectionExtensionsType
                .GetMethods(BindingFlags.Public | BindingFlags.Static).First(u => u.Name == "AddObjectMapper");

            return addObjectMapperMethod.Invoke(null, new object[] {services, App.Assemblies.ToArray()}) as IServiceCollection;
        }

        return services;
    }

    /// <summary>
    /// 添加日志服务
    /// /// 197001/01/24.log
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    internal static IServiceCollection AddLogging(this IServiceCollection services)
    {
        // 判断是否安装了 Logging 程序集
        var cacheAssembly = App.Assemblies.FirstOrDefault(f => f.GetName().Name?.Equals("Fast.Logging") == true);
        if (cacheAssembly != null)
        {
            Debugging.Info("正在注册 Logging......");
            // 加载 Cache 拓展类和拓展方法
            var cacheIServiceCollectionExtensionType =
                Reflect.GetType(cacheAssembly, "Fast.Logging.Extensions.LoggingIServiceCollectionExtension");
            var addCacheMethod = cacheIServiceCollectionExtensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(f => f.Name == "AddLogging");

            /**
             *
             *Debugging.Info("正在注册 Logging......");
// 加载 Cache 拓展类和拓展方法
var cacheIServiceCollectionExtensionType = Reflect.GetType(cacheAssembly, "Fast.Logging.Extensions.LoggingIServiceCollectionExtension");
var addCacheMethod = cacheIServiceCollectionExtensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
    .First(f => f.Name == "AddLogging");

// 获取 AddLogging 方法的参数信息
ParameterInfo[] parameters = addCacheMethod.GetParameters();

// 创建一个与参数个数相匹配的空参数数组
object[] arguments = new object[parameters.Length];

// 遍历参数，如果是 fileSizeLimitBytes 参数，则保持默认值；否则设置为 null
for (int i = 0; i < parameters.Length; i++)
{
    if (parameters[i].Name == "fileSizeLimitBytes")
    {
        arguments[i] = Type.Missing;
    }
    else
    {
        arguments[i] = null;
    }
}

// 调用 AddLogging 方法，并传递参数数组
addCacheMethod.Invoke(null, arguments);

return services;
             *
             */

            // TODO:这里要设置默认值
            return addCacheMethod.Invoke(null, new object[] {services, 10 * 1024 * 1024}) as IServiceCollection;
        }

        return services;
    }

    /// <summary>
    /// 添加缓存
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    internal static IServiceCollection AddCache(this IServiceCollection services)
    {
        // 判断是否安装了 Cache 程序集
        var cacheAssembly = App.Assemblies.FirstOrDefault(f => f.GetName().Name?.Equals("Fast.Cache") == true);
        if (cacheAssembly != null)
        {
            Debugging.Info("正在注册 Cache......");
            // 加载 Cache 拓展类和拓展方法
            var cacheIServiceCollectionExtensionType =
                Reflect.GetType(cacheAssembly, "Fast.Cache.Extensions.CacheIServiceCollectionExtension");
            var addCacheMethod = cacheIServiceCollectionExtensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(f => f.Name == "AddCache");

            return addCacheMethod.Invoke(null, new object[] {services}) as IServiceCollection;
        }

        return services;
    }

    /// <summary>
    /// 添加鉴权用户
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    internal static IServiceCollection AddAuthentication(this IServiceCollection services)
    {
        // 判断是否安装了 Authentication 程序集
        var cacheAssembly = App.Assemblies.FirstOrDefault(f => f.GetName().Name?.Equals("Fast.Authentication") == true);
        if (cacheAssembly != null)
        {
            Debugging.Info("正在注册 Authentication......");
            // 加载 Cache 拓展类和拓展方法
            var cacheIServiceCollectionExtensionType =
                Reflect.GetType(cacheAssembly, "Fast.Authentication.Extensions.AddAuthentication");
            var addCacheMethod = cacheIServiceCollectionExtensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(f => f.Name == "AddAuthentication");

            return addCacheMethod.Invoke(null, new object[] {services}) as IServiceCollection;
        }

        return services;
    }

    /// <summary>
    /// 添加 SqlSugar
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    internal static IServiceCollection AddSqlSugar(this IServiceCollection services)
    {
        // 判断是否安装了 Cache 程序集
        var cacheAssembly = App.Assemblies.FirstOrDefault(f => f.GetName().Name?.Equals("Fast.Sugar") == true);
        if (cacheAssembly != null)
        {
            Debugging.Info("正在注册 SqlSugar......");
            // 加载 Cache 拓展类和拓展方法
            var cacheIServiceCollectionExtensionType =
                Reflect.GetType(cacheAssembly, "Fast.Sugar.Extensions.SqlSugarIServiceCollectionExtension");
            var addCacheMethod = cacheIServiceCollectionExtensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(f => f.Name == "AddSqlSugar");

            return addCacheMethod.Invoke(null, new object[] {services}) as IServiceCollection;
        }

        return services;
    }

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
}