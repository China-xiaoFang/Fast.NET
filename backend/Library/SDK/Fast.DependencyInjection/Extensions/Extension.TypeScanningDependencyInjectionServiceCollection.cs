
using System;
using Furion.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 依赖注入模块 <see cref="IServiceCollection"/> 拓展类
/// </summary>
// ReSharper disable once PartialTypeWithSinglePart
public static partial class Extensions
{
    /// <summary>
    /// 添加类型扫描依赖关系服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddTypeScanning(this IServiceCollection services, Action<TypeScanningDependencyBuilder>? configure = null)
    {
        // 初始化类型扫描依赖关系构建器
        var typeScanningDependencyBuilder = new TypeScanningDependencyBuilder();

        // 调用自定义配置委托
        configure?.Invoke(typeScanningDependencyBuilder);

        return services.AddTypeScanning(typeScanningDependencyBuilder);
    }

    /// <summary>
    /// 添加类型扫描依赖关系服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="typeScanningDependencyBuilder"><see cref="TypeScanningDependencyBuilder"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddTypeScanning(this IServiceCollection services, TypeScanningDependencyBuilder typeScanningDependencyBuilder)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(typeScanningDependencyBuilder);

        // 构建模块服务
        typeScanningDependencyBuilder.Build(services);

        return services;
    }
}