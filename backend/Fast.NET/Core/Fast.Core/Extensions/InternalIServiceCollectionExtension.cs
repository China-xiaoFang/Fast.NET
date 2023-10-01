using System.Reflection;
using Fast.Core.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Core.Extensions;

/// <summary>
/// <see cref="IServiceCollection"/> 内部拓展类
/// </summary>
internal static class InternalIServiceCollectionExtension
{
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
            // 加载 Cache 拓展类和拓展方法
            var iServiceCollectionExtensionType = Reflect.GetType(assembly,
                "Fast.CorsAccessor.Extensions.CorsAccessorIServiceCollectionExtension");
            var method = iServiceCollectionExtensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
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
            // 加载 Cache 拓展类和拓展方法
            var iServiceCollectionExtensionType = Reflect.GetType(assembly,
                "Fast.Serialization.Extensions.SerializationIServiceCollectionExtension");
            var method = iServiceCollectionExtensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
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
            // 加载 ObjectMapper 拓展类型和拓展方法
            var iServiceCollectionExtensionType = Reflect.GetType(assembly, "Fast.Mapster.Extensions.ObjectMapperExtension");
            var method = iServiceCollectionExtensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(u => u.Name == "AddObjectMapper");

            return method.Invoke(null, new object[] {services, App.Assemblies.ToArray()}) as IServiceCollection;
        }

        return services;
    }
}