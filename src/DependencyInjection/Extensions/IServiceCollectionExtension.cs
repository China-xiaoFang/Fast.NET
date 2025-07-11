// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Fast.DependencyInjection;

/// <summary>
/// <see cref="IServiceCollection"/> 依赖注入 拓展类
/// </summary>
[SuppressSniffer]
public static class IServiceCollectionExtension
{
    /// <summary>
    /// 类型名称集合
    /// </summary>
    private static readonly ConcurrentDictionary<string, Type> TypeNamedCollection = new();

    /// <summary>
    /// 添加依赖注入服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
    {
        Debugging.Info("Registering dependency injection......");

        var IDependencyType = typeof(IDependency);

        // 获取程序集需要依赖注入的类型
        var injectTypes = MAppContext.EffectiveTypes.Where(wh =>
            IDependencyType.IsAssignableFrom(wh) && wh.IsClass && !wh.IsInterface && !wh.IsAbstract);

        var lifetimeInterfaces = new[] {typeof(ITransientDependency), typeof(IScopedDependency), typeof(ISingletonDependency)};

        // 执行依赖注入
        foreach (var type in injectTypes)
        {
            var interfaces = type.GetInterfaces();

            // 获取所有能注册的接口
            var canInjectInterfaces = interfaces.Where(u => u != typeof(IDisposable)
                                                            && u != typeof(IAsyncDisposable)
                                                            && u != typeof(IDependency)
                                                            && !lifetimeInterfaces.Contains(u)
                                                            && MAppContext.Assemblies.Contains(u.Assembly)
                                                            && ((!type.IsGenericType && !u.IsGenericType)
                                                                || (type.IsGenericType
                                                                    && u.IsGenericType
                                                                    && type.GetGenericArguments()
                                                                        .Length
                                                                    == u.GetGenericArguments()
                                                                        .Length)));

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