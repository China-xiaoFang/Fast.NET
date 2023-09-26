using System.Reflection;
using Fast.Core.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fast.Core.Extensions;

/// <summary>
/// <see cref="IServiceCollection"/> 内部拓展类
/// </summary>
internal static class InternalIServiceCollectionExtension
{
    /// <summary>
    /// 添加日志服务
    /// 197001/01/24.log
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    internal static IServiceCollection AddLogging(this IServiceCollection services)
    {
        // 判断是否安装了 Logging 程序集
        var assembly = App.Assemblies.FirstOrDefault(f => f.GetName().Name?.Equals("Fast.Logging") == true);
        if (assembly != null)
        {
            InternalApp.LogInfo("Registering the Logging service......");

            // 加载 Cache 拓展类和拓展方法
            var cacheIServiceCollectionExtensionType =
                Reflect.GetType(assembly, "Fast.Logging.Extensions.LoggingIServiceCollectionExtension");
            var method = cacheIServiceCollectionExtensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(f => f.Name == "AddLogging");

            var result = method.Invoke(null, new[] {services, Type.Missing}) as IServiceCollection;

            // 创建一个新的 LoggerFactory 实例
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                // TODO：这里待优化，输出的格式不对
                builder.AddConsole(options => options.FormatterName = "console-format");
            });

            // 创建 ILogger 实例
            InternalApp.Logger = loggerFactory.CreateLogger("Fast.Core.App");

            return result;
        }

        return services;
    }

    /// <summary>
    /// 添加跨域配置
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    internal static IServiceCollection AddCorsAccessor(this IServiceCollection services, IConfiguration configuration)
    {
        // 判断是否安装了 CorsAccessor 程序集
        var assembly = App.Assemblies.FirstOrDefault(f => f.GetName().Name?.Equals("Fast.CorsAccessor") == true);
        if (assembly != null)
        {
            InternalApp.LogInfo("Registering for the CorsAccessor service......");
            // 加载 Cache 拓展类和拓展方法
            var cacheIServiceCollectionExtensionType = Reflect.GetType(assembly,
                "Fast.CorsAccessor.Extensions.CorsAccessorIServiceCollectionExtension");
            var method = cacheIServiceCollectionExtensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(f => f.Name == "AddCorsAccessor");

            return method.Invoke(null, new[] {services, configuration, Type.Missing, Type.Missing}) as IServiceCollection;
        }

        return services;
    }

    /// <summary>
    /// 添加JSON序列化配置
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    internal static IServiceCollection AddJsonOptions(this IServiceCollection services)
    {
        // 判断是否安装了 Serialization 程序集
        var assembly = App.Assemblies.FirstOrDefault(f => f.GetName().Name?.Equals("Fast.Serialization") == true);
        if (assembly != null)
        {
            InternalApp.LogInfo("Registering for the Serialization service......");
            // 加载 Cache 拓展类和拓展方法
            var cacheIServiceCollectionExtensionType = Reflect.GetType(assembly,
                "Fast.Serialization.Extensions.SerializationIServiceCollectionExtension");
            var method = cacheIServiceCollectionExtensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(f => f.Name == "AddJsonOptions");

            return method.Invoke(null, new object[] {services}) as IServiceCollection;
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
        var assembly = App.Assemblies.FirstOrDefault(u => u.GetName().Name?.Equals("Fast.Mapster") == true);
        if (assembly != null)
        {
            InternalApp.LogInfo("Registering the Mapster mapping service......");
            // 加载 ObjectMapper 拓展类型和拓展方法
            var objectMapperServiceCollectionExtensionsType =
                Reflect.GetType(assembly, "Fast.Mapster.Extensions.ObjectMapperExtension");
            var method = objectMapperServiceCollectionExtensionsType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(u => u.Name == "AddObjectMapper");

            return method.Invoke(null, new object[] {services, App.Assemblies.ToArray()}) as IServiceCollection;
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
        var assembly = App.Assemblies.FirstOrDefault(f => f.GetName().Name?.Equals("Fast.Cache") == true);
        if (assembly != null)
        {
            InternalApp.LogInfo("Registering the Cache service.......");
            // 加载 Cache 拓展类和拓展方法
            var cacheIServiceCollectionExtensionType =
                Reflect.GetType(assembly, "Fast.Cache.Extensions.CacheIServiceCollectionExtension");
            var method = cacheIServiceCollectionExtensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(f => f.Name == "AddCache");

            return method.Invoke(null, new object[] {services}) as IServiceCollection;
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
        var assembly = App.Assemblies.FirstOrDefault(f => f.GetName().Name?.Equals("Fast.Authentication") == true);
        if (assembly != null)
        {
            InternalApp.LogInfo("Registering the Authentication service......");
            // 加载 Cache 拓展类和拓展方法
            var cacheIServiceCollectionExtensionType =
                Reflect.GetType(assembly, "Fast.Authentication.Extensions.AddAuthentication");
            var method = cacheIServiceCollectionExtensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(f => f.Name == "AddAuthentication");

            return method.Invoke(null, new object[] {services}) as IServiceCollection;
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
        var assembly = App.Assemblies.FirstOrDefault(f => f.GetName().Name?.Equals("Fast.Sugar") == true);
        if (assembly != null)
        {
            InternalApp.LogInfo("Registering for the SqlSugar service......");
            // 加载 Cache 拓展类和拓展方法
            var cacheIServiceCollectionExtensionType =
                Reflect.GetType(assembly, "Fast.Sugar.Extensions.SqlSugarIServiceCollectionExtension");
            var method = cacheIServiceCollectionExtensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(f => f.Name == "AddSqlSugar");

            return method.Invoke(null, new object[] {services}) as IServiceCollection;
        }

        return services;
    }
}