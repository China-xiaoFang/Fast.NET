// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using System.Reflection;
using Fast.NET.Core.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.NET.Core.Extensions;

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
    /// 添加全局依赖注入
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    internal static IServiceCollection AddInnerDependencyInjection(this IServiceCollection services)
    {
        // 判断是否安装了 DependencyInjection 程序集
        var assembly = App.Assemblies.FirstOrDefault(u => u.GetName().Name?.Equals("Fast.DependencyInjection") == true);
        if (assembly != null)
        {
            // 加载 ObjectMapper 拓展类型和拓展方法
            var iServiceCollectionExtensionType = Reflect.GetType(assembly,
                "Fast.DependencyInjection.Extensions.DependencyInjectionIServiceCollectionExtension");
            var method = iServiceCollectionExtensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(u => u.Name == "AddInnerDependencyInjection");

            return method.Invoke(null, new object[] {services, App.Assemblies.ToArray()}) as IServiceCollection;
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