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

using System.Collections.Concurrent;
using Fast.IaaS;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.DependencyInjection.Extensions;

/// <summary>
/// <see cref="IServiceCollection"/> 依赖注入拓展类
/// </summary>
[SuppressSniffer]
public static class DependencyInjectionIServiceCollectionExtension
{
    /// <summary>
    /// 类型名称集合
    /// </summary>
    private static readonly ConcurrentDictionary<string, Type> TypeNamedCollection;

    static DependencyInjectionIServiceCollectionExtension()
    {
        TypeNamedCollection = new ConcurrentDictionary<string, Type>();
    }

    /// <summary>
    /// 添加依赖注入
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
    {
        // 添加内部依赖注入扫描拓展
        services.AddInnerDependencyInjection();

        // 注册命名服务
        services.AddTransient(typeof(INamedServiceProvider<>), typeof(NamedServiceProvider<>));

        return services;
    }

    /// <summary>
    /// 添加扫描注入
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    private static IServiceCollection AddInnerDependencyInjection(this IServiceCollection services)
    {
        // 查找所有需要依赖注入的类型
        var injectTypes = FastContext.EffectiveTypes.Where(u =>
            typeof(IDependency).IsAssignableFrom(u) && u.IsClass && !u.IsInterface && !u.IsAbstract);

        var projectAssemblies = FastContext.Assemblies;
        var lifetimeInterfaces = new[] {typeof(ITransientDependency), typeof(IScopedDependency), typeof(ISingletonDependency)};

        // 执行依赖注入
        foreach (var type in injectTypes)
        {
            var interfaces = type.GetInterfaces();

            // 获取所有能注册的接口
            var canInjectInterfaces = interfaces.Where(u =>
                u != typeof(IDisposable) && u != typeof(IAsyncDisposable) && u != typeof(IDependency) &&
                !lifetimeInterfaces.Contains(u) && projectAssemblies.Contains(u.Assembly) &&
                (!type.IsGenericType && !u.IsGenericType || type.IsGenericType && u.IsGenericType &&
                    type.GetGenericArguments().Length == u.GetGenericArguments().Length));

            // 获取生存周期类型
            var dependencyType = interfaces.Last(u => lifetimeInterfaces.Contains(u));

            // 注册服务
            RegisterService(services, dependencyType, type, canInjectInterfaces);

            // 缓存类型注册
            TypeNamedCollection.TryAdd(type.Name, type);
        }

        // 注册命名服务（接口多实现）
        RegisterNamedService<ITransientDependency>(services);
        RegisterNamedService<IScopedDependency>(services);
        RegisterNamedService<ISingletonDependency>(services);

        return services;
    }

    /// <summary>
    /// 注册服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="dependencyType"></param>
    /// <param name="type">类型</param>
    /// <param name="canInjectInterfaces">能被注册的接口</param>
    private static void RegisterService(IServiceCollection services, Type dependencyType, Type type,
        IEnumerable<Type> canInjectInterfaces)
    {
        // 这里默认注册多个接口
        foreach (var inter in canInjectInterfaces)
        {
            Register(services, dependencyType, type, inter);
        }
    }

    /// <summary>
    /// 注册类型
    /// </summary>
    /// <param name="services">服务</param>
    /// <param name="dependencyType"></param>
    /// <param name="type">类型</param>
    /// <param name="inter">接口</param>
    private static void Register(IServiceCollection services, Type dependencyType, Type type, Type inter = null)
    {
        // 修复泛型注册类型
        var fixedType = FixedGenericType(type);
        var fixedInter = inter == null ? null : FixedGenericType(inter);
        var lifetime = TryGetServiceLifetime(dependencyType);

        if (fixedInter == null)
        {
            services.Add(ServiceDescriptor.Describe(fixedType, fixedType, lifetime));
        }
        else
        {
            services.Add(ServiceDescriptor.Describe(fixedInter, fixedType, lifetime));
        }
    }

    /// <summary>
    /// 修复泛型类型注册类型问题
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns></returns>
    private static Type FixedGenericType(Type type)
    {
        if (!type.IsGenericType)
            return type;

        return type.Assembly.GetType($"{type.Namespace}.{type.Name}");
    }

    /// <summary>
    /// 注册命名服务（接口多实现）
    /// </summary>
    /// <typeparam name="TDependency"></typeparam>
    /// <param name="services"></param>
    private static void RegisterNamedService<TDependency>(IServiceCollection services) where TDependency : IDependency
    {
        var lifetime = TryGetServiceLifetime(typeof(TDependency));

        // 注册命名服务
        services.Add(ServiceDescriptor.Describe(typeof(Func<string, TDependency, object>), provider =>
        {
            object ResolveService(string named, TDependency _)
            {
                var isRegister = TypeNamedCollection.TryGetValue(named, out var serviceType);
                return isRegister ? provider.GetService(serviceType) : null;
            }

            return (Func<string, TDependency, object>) ResolveService;
        }, lifetime));
    }

    /// <summary>
    /// 根据依赖接口类型解析 ServiceLifetime 对象
    /// </summary>
    /// <param name="dependencyType"></param>
    /// <returns></returns>
    private static ServiceLifetime TryGetServiceLifetime(Type dependencyType)
    {
        if (dependencyType == typeof(ITransientDependency))
        {
            return ServiceLifetime.Transient;
        }

        if (dependencyType == typeof(IScopedDependency))
        {
            return ServiceLifetime.Scoped;
        }

        if (dependencyType == typeof(ISingletonDependency))
        {
            return ServiceLifetime.Singleton;
        }

        throw new InvalidCastException("Invalid service registration lifetime.");
    }
}