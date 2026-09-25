// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

namespace Fast.DependencyInjection;

/// <summary>
/// 命名服务提供器
/// </summary>
/// <typeparam name="TService">目标服务接口</typeparam>
internal interface INamedServiceProvider<out TService> where TService : class
{
    /// <summary>
    /// 根据服务名称获取服务
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <typeparam name="ILifetime">服务生存周期接口，<see cref="ITransientDependency"/>，<see cref="IScopedDependency"/>，<see cref="IScopedDependency"/></typeparam>
    /// <returns>根据服务名称获取服务</returns>
    TService GetService<ILifetime>(string serviceName) where ILifetime : IDependency;

    /// <summary>
    /// 根据服务名称获取服务
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <typeparam name="ILifetime">服务生存周期接口，<see cref="ITransientDependency"/>，<see cref="IScopedDependency"/>，<see cref="IScopedDependency"/></typeparam>
    /// <returns>根据服务名称获取服务</returns>
    TService GetRequiredService<ILifetime>(string serviceName) where ILifetime : IDependency;
}
