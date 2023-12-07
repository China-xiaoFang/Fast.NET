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

// ReSharper disable once CheckNamespace

namespace Fast.DependencyInjection;

/// <summary>
/// <see cref="INamedServiceProvider{TService}"/> 命名服务提供器
/// </summary>
/// <typeparam name="TService">目标服务接口</typeparam>
public interface INamedServiceProvider<out TService> where TService : class
{
    /// <summary>
    /// 根据服务名称获取服务
    /// </summary>
    /// <typeparam name="ILifetime">服务生存周期接口，<see cref="ITransientDependency"/>，<see cref="IScopedDependency"/>，<see cref="IScopedDependency"/></typeparam>
    /// <param name="serviceName"><see cref="string"/> 服务名称</param>
    /// <returns></returns>
    TService GetService<ILifetime>(string serviceName) where ILifetime : IDependency;

    /// <summary>
    /// 根据服务名称获取服务
    /// </summary>
    /// <typeparam name="ILifetime">服务生存周期接口，<see cref="ITransientDependency"/>，<see cref="IScopedDependency"/>，<see cref="IScopedDependency"/></typeparam>
    /// <param name="serviceName"><see cref="string"/> 服务名称</param>
    /// <returns></returns>
    TService GetRequiredService<ILifetime>(string serviceName) where ILifetime : IDependency;
}