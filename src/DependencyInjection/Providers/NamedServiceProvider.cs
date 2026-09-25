// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using Microsoft.Extensions.DependencyInjection;

namespace Fast.DependencyInjection;

/// <summary>
/// 命名服务提供器默认实现
/// </summary>
/// <typeparam name="TService">目标服务接口</typeparam>
internal sealed class NamedServiceProvider<TService> : INamedServiceProvider<TService> where TService : class
{
    /// <summary>
    /// 服务提供器
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// 初始化命名服务提供器
    /// </summary>
    /// <param name="serviceProvider">用于解析命名服务工厂的服务提供器</param>
    public NamedServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public TService GetService<ILifetime>(string serviceName) where ILifetime : IDependency
    {
        Func<string, ILifetime, object> resolveNamed = _serviceProvider.GetService<Func<string, ILifetime, object>>();
        if (resolveNamed?.Invoke(serviceName, default) is TService result)
        {
            return result;
        }

        throw new InvalidOperationException($"Named service `{serviceName}` is not registered in container.");
    }

    /// <inheritdoc />
    public TService GetRequiredService<ILifetime>(string serviceName) where ILifetime : IDependency
    {
        Func<string, ILifetime, object> resolveNamed = _serviceProvider.GetRequiredService<Func<string, ILifetime, object>>();
        var service = resolveNamed?.Invoke(serviceName, default) as TService;

        // 如果服务不存在，抛出异常
        return service ?? throw new InvalidOperationException($"Named service `{serviceName}` is not registered in container.");
    }
}
