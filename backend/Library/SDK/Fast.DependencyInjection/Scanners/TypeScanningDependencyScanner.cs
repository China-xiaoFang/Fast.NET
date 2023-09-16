
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fast.Iaas.Extension;
using Microsoft.Extensions.DependencyInjection;

namespace Furion.DependencyInjection;

/// <summary>
/// 类型扫描依赖关系扫描器
/// </summary>
internal sealed class TypeScanningDependencyScanner
{
    /// <inheritdoc cref="IServiceCollection"/>
    internal readonly IServiceCollection _services;

    /// <inheritdoc cref="TypeScanningDependencyBuilder"/>
    internal readonly TypeScanningDependencyBuilder _typeScanningDependencyBuilder;

    /// <summary>
    /// <inheritdoc cref="TypeScanningDependencyScanner"/>
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="typeScanningDependencyBuilder"><see cref="TypeScanningDependencyBuilder"/></param>
    internal TypeScanningDependencyScanner(IServiceCollection services
        , TypeScanningDependencyBuilder typeScanningDependencyBuilder)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(typeScanningDependencyBuilder);

        _services = services;
        _typeScanningDependencyBuilder = typeScanningDependencyBuilder;

        // 初始化
        Initialize();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    internal void Initialize()
    {
        // 默认添加启动程序集作为扫描
        _typeScanningDependencyBuilder.AddAssemblies(Assembly.GetEntryAssembly()!);
    }

    /// <summary>
    /// 扫描并添加服务
    /// </summary>
    internal void ScanToAddServices()
    {
        // 检查是否禁用程序集扫描
        if (_typeScanningDependencyBuilder.SuppressAssemblyScanning)
        {
            // 输出调试事件
            Debugging.Warn("Type scanning has been disabled.");

            return;
        }

        // 创建类型扫描依赖关系模型集合并排序
        var typeScanningDependencyModels = CreateModels()
            .OrderBy(model => model.Order);

        // 逐条添加服务
        foreach (var typeScanningDependencyModel in typeScanningDependencyModels)
        {
            AddService(typeScanningDependencyModel);
        }
    }

    /// <summary>
    /// 创建类型扫描依赖关系模型集合
    /// </summary>
    /// <returns><see cref="IEnumerable{T}"/></returns>
    internal IEnumerable<TypeScanningDependencyModel> CreateModels()
    {
        // 扫描所有程序集类型
        var types = _typeScanningDependencyBuilder._assemblies
            .SelectMany(assembly => assembly.GetTypes(_typeScanningDependencyBuilder.SuppressNonPublicType))
            .Where(type => typeof(IDependency).IsAssignableFrom(type) && type.IsInstantiable())
            .Where(type => _typeScanningDependencyBuilder._typeFilterConfigure is null || _typeScanningDependencyBuilder._typeFilterConfigure.Invoke(type));

        // 遍历所有类型创建类型扫描依赖关系模型集合
        foreach (var type in types)
        {
            // 获取 [Dependency] 特性对象
            var dependencyAttribute = type.GetDefinedCustomAttribute<DependencyAttribute>(true);

            // 检查 Ignore 属性
            if (dependencyAttribute is { Ignore: true })
            {
                continue;
            }

            // 获取类型服务集合
            var serviceTypes = GetServiceTypes(type, out var serviceLifetime);

            // 检查 IncludeBase 属性
            if (dependencyAttribute is { IncludeBase: true })
            {
                // 获取基类型
                var baseType = type.BaseType;

                // 检查基类型是否和类型相兼容
                if (baseType is not null
                    && type.IsCompatibilityTo(baseType))
                {
                    serviceTypes.Insert(0, GetTypeDefinition(type, baseType));
                }
            }

            // 获取实现服务类型
            var implementationType = GetTypeDefinition(type, type);

            // 检查 IncludeSelf 属性
            if (dependencyAttribute is { IncludeSelf: true }
                || serviceTypes.Count == 0)
            {
                serviceTypes.Insert(0, implementationType);
            }

            // 初始化类型扫描依赖关系模型集合
            var typeScanningDependencyModels = serviceTypes
                .Select(serviceType => new TypeScanningDependencyModel(serviceType, implementationType
                    , serviceLifetime, dependencyAttribute?.RegistrationType ?? RegistrationTypeEnum.Add)
                {
                    Order = dependencyAttribute?.Order ?? 0
                })
                .Where(model => _typeScanningDependencyBuilder._filterConfigure is null || _typeScanningDependencyBuilder._filterConfigure.Invoke(model));

            // 遍历所有服务类型创建类型扫描依赖关系模型集合
            foreach (var typeScanningDependencyModel in typeScanningDependencyModels)
            {
                yield return typeScanningDependencyModel;
            }
        }
    }

    /// <summary>
    /// 获取类型服务集合
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <param name="serviceLifetime"><see cref="ServiceLifetime"/></param>
    /// <returns><see cref="List{T}"/></returns>
    internal List<Type> GetServiceTypes(Type type, out ServiceLifetime serviceLifetime)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(type);

        // 获取类型接口集合
        var interfaces = type.GetInterfaces();

        // 解析服务注册生存期
        serviceLifetime = GetServiceLifetime(
            interfaces.Last(i => i.IsAlienAssignableTo(typeof(IDependency))));

        // 获取导出的类型服务集合
        var limitServiceTypes = type.GetDefinedCustomAttribute<ExposeServicesAttribute>(true)
            ?.ServiceTypes;

        // 获取黑名单类型服务集合
        var blacklistServiceTypes = _typeScanningDependencyBuilder._blacklistServiceTypes;

        // 服务类型过滤器
        bool TypeFilter(Type? serviceType)
        {
            // 检查服务类型是否在黑名单类型服务集合中
            if (blacklistServiceTypes.Any(type => type.IsDefinitionEqual(serviceType)))
            {
                return false;
            }

            // 检查服务类型是否在导出的类型服务集合中
            if (limitServiceTypes is not null
                && !limitServiceTypes.Any(type => type.IsDefinitionEqual(serviceType)))
            {
                return false;
            }

            // 检查服务类型是否派生自依赖关系接口类型
            return !typeof(IDependency).IsAssignableFrom(serviceType) &&
                   // 检查服务类型是否和类型相兼容
                   type.IsCompatibilityTo(serviceType);
        }

        // 获取类型服务集合
        var serviceTypes = interfaces.Where(TypeFilter)
            .Select(serviceType => GetTypeDefinition(type, serviceType))
            .ToList();

        return serviceTypes;
    }

    /// <summary>
    /// 添加服务
    /// </summary>
    /// <param name="typeScanningDependencyModel"><see cref="TypeScanningDependencyModel"/></param>
    /// <exception cref="NotSupportedException"></exception>
    internal void AddService(TypeScanningDependencyModel typeScanningDependencyModel)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(typeScanningDependencyModel);

        // 获取服务描述器
        var serviceDescriptor = typeScanningDependencyModel.Descriptor;

        // 注册服务
        switch (typeScanningDependencyModel.RegistrationType)
        {
            // 添加服务
            case RegistrationTypeEnum.Add:
                _services.Add(serviceDescriptor);
                break;

            // TODO：.NET 8新特性，待集成
            //// 尝试添加服务
            //case RegistrationTypeEnum.TryAdd:
            //    _services.TryAdd(serviceDescriptor);
            //    break;

            //// 尝试添加服务集合
            //case RegistrationTypeEnum.TryAddEnumerable:
            //    _services.TryAddEnumerable(serviceDescriptor);
            //    break;

            //// 替换服务
            //case RegistrationTypeEnum.Replace:
            //    _services.Replace(serviceDescriptor);
            //    break;

            default: throw new NotSupportedException();
        }
    }

    /// <summary>
    /// 获取服务注册生存期
    /// </summary>
    /// <param name="dependencyType">依赖关系接口类型</param>
    /// <returns><see cref="ServiceLifetime"/></returns>
    /// <exception cref="NotSupportedException"></exception>
    internal static ServiceLifetime GetServiceLifetime(Type dependencyType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(dependencyType);

        return dependencyType switch
        {
            // 瞬时服务生存期
            _ when dependencyType == typeof(ITransient) => ServiceLifetime.Transient,
            // 范围服务生存期
            _ when dependencyType == typeof(IScoped) => ServiceLifetime.Scoped,
            // 单例服务生存期
            _ when dependencyType == typeof(ISingleton) => ServiceLifetime.Singleton,

            _ => throw new NotSupportedException()
        };
    }

    /// <summary>
    /// 获取类型定义
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <param name="serviceType">服务类型</param>
    /// <returns><see cref="Type"/></returns>
    internal static Type GetTypeDefinition(Type type, Type serviceType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(serviceType);

        return !type.IsGenericType
            ? serviceType
            : serviceType.GetGenericTypeDefinition();
    }
}