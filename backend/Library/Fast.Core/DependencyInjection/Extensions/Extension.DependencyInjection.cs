using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Fast.Core.DependencyInjection.Dependencies;
using Fast.Core.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Core.DependencyInjection.Extensions;

/// <summary>
/// 依赖注入 扩展
/// </summary>
// ReSharper disable once PartialTypeWithSinglePart
public static partial class Extensions
{
    /// <summary>
    /// 类型名称集合
    /// </summary>
    private static readonly ConcurrentDictionary<string, Type> TypeNamedCollection;

    static Extensions()
    {
        TypeNamedCollection = new ConcurrentDictionary<string, Type>();
    }

    /// <summary>
    /// 添加扫描注入
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddInnerDependencyInjection(this IServiceCollection services)
    {
        // 查找所有需要依赖注入的类型
        var injectTypes = App.App.EffectiveTypes.Where(u =>
            typeof(IDependency).IsAssignableFrom(u) && u.IsClass && !u.IsInterface && !u.IsAbstract);

        var projectAssemblies = App.App.Assemblies;
        var lifetimeInterfaces = new[] {typeof(ITransientDependency), typeof(IScopedDependency), typeof(ISingletonDependency)};

        // 执行依赖注入
        foreach (var type in injectTypes)
        {
            var interfaces = type.GetInterfaces();

            // 获取所有能注册的接口
            var canInjectInterfaces = interfaces.Where(u =>
                //!DependencyAttribute.ExceptInterfaces.Contains(u) && 
                u != typeof(IDisposable) && u != typeof(IAsyncDisposable) && u != typeof(IDependency)
                //&& u != typeof(IDynamicApiController)
                && !lifetimeInterfaces.Contains(u) && projectAssemblies.Contains(u.Assembly) &&
                ((!type.IsGenericType && !u.IsGenericType) || (type.IsGenericType && u.IsGenericType &&
                                                               type.GetGenericArguments().Length ==
                                                               u.GetGenericArguments().Length)));

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
            services.Add(ServiceDescriptor.Describe(fixedType, fixedType, lifetime));
        else
        {
            services.Add(ServiceDescriptor.Describe(fixedInter, fixedType, lifetime));
            AddDispatchProxy(services, dependencyType, fixedType, fixedInter);
        }
    }

    /// <summary>
    /// 创建服务代理
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="dependencyType"></param>
    /// <param name="type">拦截的类型</param>
    /// <param name="inter">代理接口</param>
    private static void AddDispatchProxy(IServiceCollection services, Type dependencyType, Type type, Type inter)
    {
        var lifetime = TryGetServiceLifetime(dependencyType);

        // 注册服务
        services.Add(ServiceDescriptor.Describe(type, inter, lifetime));
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

        return Reflect.GetType(type.Assembly, $"{type.Namespace}.{type.Name}");
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