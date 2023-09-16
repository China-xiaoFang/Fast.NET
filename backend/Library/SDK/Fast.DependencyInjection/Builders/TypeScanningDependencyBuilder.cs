
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Furion.DependencyInjection;

/// <summary>
/// 类型扫描依赖关系构建器
/// </summary>
public sealed class TypeScanningDependencyBuilder
{
    /// <summary>
    /// 待扫描的程序集集合
    /// </summary>
    internal readonly HashSet<Assembly> _assemblies;

    /// <summary>
    /// 黑名单类型服务集合
    /// </summary>
    internal readonly HashSet<Type> _blacklistServiceTypes;

    /// <summary>
    /// 类型扫描依赖关系模型过滤器
    /// </summary>
    internal Func<TypeScanningDependencyModel, bool>? _filterConfigure;

    /// <summary>
    /// 类型扫描过滤器
    /// </summary>
    internal Func<Type, bool>? _typeFilterConfigure;

    /// <summary>
    /// <inheritdoc cref="TypeScanningDependencyBuilder"/>
    /// </summary>
    public TypeScanningDependencyBuilder()
    {
        _assemblies = new();

        _blacklistServiceTypes = new()
        {
            typeof(IDisposable), typeof(IAsyncDisposable),
            typeof(IDependency), typeof(IEnumerator),
            typeof(IEnumerable), typeof(ICollection),
            typeof(IDictionary), typeof(IComparable),
            typeof(object), typeof(DynamicObject)
        };
    }

    /// <summary>
    /// 禁用程序集扫描
    /// </summary>
    public bool SuppressAssemblyScanning { get; set; }

    /// <summary>
    /// 禁用非公开类型
    /// </summary>
    public bool SuppressNonPublicType { get; set; }

    /// <summary>
    /// 添加类型扫描依赖关系模型过滤器
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    public void AddFilter(Func<TypeScanningDependencyModel, bool> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        _filterConfigure = configure;
    }

    /// <summary>
    /// 添加类型扫描过滤器
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    public void AddTypeFilter(Func<Type, bool> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        _typeFilterConfigure = configure;
    }

    /// <summary>
    /// 添加程序集
    /// </summary>
    /// <param name="assemblies"><see cref="Assembly"/>[]</param>
    /// <returns><see cref="TypeScanningDependencyBuilder"/></returns>
    public TypeScanningDependencyBuilder AddAssemblies(params Assembly[] assemblies)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(assemblies);

        Array.ForEach(assemblies, assembly =>
        {
            // 空检查
            ArgumentNullException.ThrowIfNull(assembly);

            _assemblies.Add(assembly);
        });

        return this;
    }

    /// <summary>
    /// 添加程序集
    /// </summary>
    /// <param name="assemblies"><see cref="IEnumerable{T}"/></param>
    /// <returns><see cref="TypeScanningDependencyBuilder"/></returns>
    public TypeScanningDependencyBuilder AddAssemblies(IEnumerable<Assembly> assemblies)
    {
        return AddAssemblies(assemblies.ToArray());
    }

    /// <summary>
    /// 添加黑名单服务类型
    /// </summary>
    /// <param name="types"><see cref="Type"/>[]</param>
    /// <returns><see cref="TypeScanningDependencyBuilder"/></returns>
    public TypeScanningDependencyBuilder AddBlacklistTypes(params Type[] types)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(types);

        // 逐条添加黑名单服务类型到集合中
        Array.ForEach(types, type =>
        {
            // 空检查
            ArgumentNullException.ThrowIfNull(type);

            _blacklistServiceTypes.Add(type);
        });

        return this;
    }

    /// <summary>
    /// 添加黑名单服务类型
    /// </summary>
    /// <param name="types"><see cref="IEnumerable{T}"/></param>
    /// <returns><see cref="TypeScanningDependencyBuilder"/></returns>
    public TypeScanningDependencyBuilder AddBlacklistTypes(IEnumerable<Type> types)
    {
        return AddBlacklistTypes(types.ToArray());
    }

    /// <summary>
    /// 构建模块服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    internal void Build(IServiceCollection services)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(services);

        // 初始化类型扫描依赖关系扫描器并执行扫描
        new TypeScanningDependencyScanner(services, this)
            .ScanToAddServices();
    }
}