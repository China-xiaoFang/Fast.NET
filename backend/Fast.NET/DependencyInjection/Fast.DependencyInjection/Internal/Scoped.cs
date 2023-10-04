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

using Fast.Core;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Fast.DependencyInjection;

/// <summary>
/// 创建作用域静态类
/// </summary>
public static class Scoped
{
    /// <summary>
    /// 创建一个作用域范围
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="scopeFactory"></param>
    public static void Create(Action<IServiceScopeFactory, IServiceScope> handler, IServiceScopeFactory scopeFactory = default)
    {
        CreateAsync(async (fac, scope) =>
        {
            handler(fac, scope);
            await Task.CompletedTask;
        }, scopeFactory).GetAwaiter().GetResult();
    }

    /// <summary>
    /// 创建一个作用域范围（异步）
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="scopeFactory"></param>
    public static async Task CreateAsync(Func<IServiceScopeFactory, IServiceScope, Task> handler,
        IServiceScopeFactory scopeFactory = default)
    {
        // 禁止空调用
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        // 创建作用域
        var (scoped, serviceProvider) = CreateScope(ref scopeFactory);

        try
        {
            // 执行方法
            await handler(scopeFactory, scoped);
        }
        finally
        {
            // 释放
            scoped.Dispose();
            if (serviceProvider != null)
                await serviceProvider.DisposeAsync();
        }
    }

    /// <summary>
    /// 创建一个作用域
    /// </summary>
    /// <param name="scopeFactory"></param>
    /// <returns></returns>
    private static (IServiceScope Scoped, ServiceProvider ServiceProvider) CreateScope(ref IServiceScopeFactory scopeFactory)
    {
        ServiceProvider unDisposeServiceProvider = default;

        if (scopeFactory == null)
        {
            // 默认返回根服务
            if (App.RootServices != null)
                scopeFactory = App.RootServices.GetService<IServiceScopeFactory>();
            else
            {
                // 这里创建了一个待释放服务提供器（这里会有性能小问题，如果走到这一步）
                unDisposeServiceProvider = App.InternalServices.BuildServiceProvider();
                scopeFactory = unDisposeServiceProvider.GetService<IServiceScopeFactory>();
            }
        }

        // 解析服务作用域工厂
        var scoped = scopeFactory.CreateScope();
        return (scoped, unDisposeServiceProvider);
    }
}