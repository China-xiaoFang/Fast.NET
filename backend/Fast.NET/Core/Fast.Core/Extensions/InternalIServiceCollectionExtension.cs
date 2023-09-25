using System.Reflection;
using Fast.Core.Diagnostics;
using Fast.Core.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Core.Extensions;

/// <summary>
/// <see cref="IServiceCollection"/> 内部拓展类
/// </summary>
internal static class InternalIServiceCollectionExtension
{
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

            return addCacheMethod.Invoke(null, new[] {services, Type.Missing}) as IServiceCollection;
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
}