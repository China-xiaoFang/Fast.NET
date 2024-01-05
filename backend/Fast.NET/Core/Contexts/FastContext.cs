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
using System.Reflection;
using Fast.IaaS;
using Fast.NET.Core.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Fast.NET.Core;

/// <summary>
/// <see cref="FastContext"/> App 上下文
/// </summary>
public static class FastContext
{
    #region 内部属性

    /// <summary>
    /// GC 回收默认间隔
    /// </summary>
    internal const int GC_COLLECT_INTERVAL_SECONDS = 5;

    /// <summary>
    /// 记录最近 GC 回收时间
    /// </summary>
    internal static DateTime? LastGCCollectTime { get; set; }

    #endregion

    #region IaaS 映射过来的一些属性和方法

    /// <summary>
    /// 应用有效程序集
    /// </summary>
    public static IEnumerable<Assembly> Assemblies => IaaSContext.Assemblies;

    /// <summary>
    /// 有效程序集类型
    /// </summary>
    public static IEnumerable<Type> EffectiveTypes => IaaSContext.EffectiveTypes;

    /// <summary>
    /// 处理获取对象异常问题
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="action">获取对象委托</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>T</returns>
    public static T CatchOrDefault<T>(Func<T> action, T defaultValue = null) where T : class
    {
        return IaaSContext.CatchOrDefault(action, defaultValue);
    }

    /// <summary>
    /// 获取当前线程 Id
    /// </summary>
    /// <returns></returns>
    public static int GetThreadId()
    {
        return IaaSContext.GetThreadId();
    }

    /// <summary>
    /// 获取一段代码执行耗时
    /// </summary>
    /// <param name="action">委托</param>
    /// <returns><see cref="long"/></returns>
    public static long GetExecutionTime(Action action)
    {
        return IaaSContext.GetExecutionTime(action);
    }

    #endregion

    /// <summary>
    /// 配置
    /// </summary>
    public static IConfiguration Configuration =>
        CatchOrDefault(() => InternalContext.Configuration.Reload(), new ConfigurationBuilder().Build());

    /// <summary>
    /// 获取Web主机环境
    /// </summary>
    public static IWebHostEnvironment WebHostEnvironment => InternalContext.WebHostEnvironment;

    /// <summary>
    /// 获取主机环境
    /// </summary>
    public static IHostEnvironment HostEnvironment => InternalContext.HostEnvironment;

    /// <summary>
    /// 应用服务
    /// </summary>
    public static IServiceCollection InternalServices => InternalContext.InternalServices;

    /// <summary>
    /// 存储根服务，可能为空
    /// </summary>
    public static IServiceProvider RootServices => InternalContext.RootServices;

    /// <summary>
    /// 请求上下文
    /// </summary>
    public static HttpContext HttpContext => CatchOrDefault(() => RootServices?.GetService<IHttpContextAccessor>()?.HttpContext);

    /// <summary>
    /// 未托管的对象集合
    /// </summary>
    public static ConcurrentBag<IDisposable> UnmanagedObjects => InternalContext.UnmanagedObjects;

    /// <summary>
    /// 解析服务提供器
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public static IServiceProvider GetServiceProvider(Type serviceType)
    {
        // 第一选择，判断是否是单例注册且单例服务不为空，如果是直接返回根服务提供器
        if (RootServices != null && InternalServices
                .Where(u => u.ServiceType == (serviceType.IsGenericType ? serviceType.GetGenericTypeDefinition() : serviceType))
                .Any(u => u.Lifetime == ServiceLifetime.Singleton))
            return RootServices;

        // 第二选择是获取 HttpContext 对象的 RequestServices
        var httpContext = HttpContext;
        if (httpContext?.RequestServices != null)
            return httpContext.RequestServices;

        // 第三选择，创建新的作用域并返回服务提供器
        if (RootServices != null)
        {
            var scoped = RootServices.CreateScope();
            UnmanagedObjects.Add(scoped);
            return scoped.ServiceProvider;
        }

        // 第四选择，构建新的服务对象（性能最差）
        var serviceProvider = InternalServices.BuildServiceProvider();
        UnmanagedObjects.Add(serviceProvider);
        return serviceProvider;
    }

    /// <summary>
    /// 获取请求生存周期的服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static TService GetService<TService>(IServiceProvider serviceProvider = default) where TService : class
    {
        return GetService(typeof(TService), serviceProvider) as TService;
    }

    /// <summary>
    /// 获取请求生存周期的服务
    /// </summary>
    /// <param name="type"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static object GetService(Type type, IServiceProvider serviceProvider = default)
    {
        return (serviceProvider ?? GetServiceProvider(type)).GetService(type);
    }

    /// <summary>
    /// 获取请求生存周期的服务集合
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static IEnumerable<TService> GetServices<TService>(IServiceProvider serviceProvider = default) where TService : class
    {
        return (serviceProvider ?? GetServiceProvider(typeof(TService))).GetServices<TService>();
    }

    /// <summary>
    /// 获取请求生存周期的服务集合
    /// </summary>
    /// <param name="type"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static IEnumerable<object> GetServices(Type type, IServiceProvider serviceProvider = default)
    {
        return (serviceProvider ?? GetServiceProvider(type)).GetServices(type);
    }

    /// <summary>
    /// 获取请求生存周期的服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static TService GetRequiredService<TService>(IServiceProvider serviceProvider = default) where TService : class
    {
        return GetRequiredService(typeof(TService), serviceProvider) as TService;
    }

    /// <summary>
    /// 获取请求生存周期的服务
    /// </summary>
    /// <param name="type"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static object GetRequiredService(Type type, IServiceProvider serviceProvider = default)
    {
        return (serviceProvider ?? GetServiceProvider(type)).GetRequiredService(type);
    }

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <typeparam name="TOptions">强类型选项类</typeparam>
    /// <param name="path"><see cref="string"/> 配置中对应的Key</param>
    /// <returns></returns>
    public static TOptions GetConfig<TOptions>(string path = null) where TOptions : class, new()
    {
        // 获取配置选项名称
        path ??= IaaSContext.GetOptionName<TOptions>();

        var options = Configuration.GetSection(path).Get<TOptions>();

        // 判断是否继承了 IPostConfigure
        if (typeof(IPostConfigure).IsAssignableFrom(typeof(TOptions)))
        {
            var postConfigureMethod = typeof(TOptions).GetMethod(nameof(IPostConfigure.PostConfigure));

            // 空值判断
            options ??= Activator.CreateInstance<TOptions>();

            // 加载后期配置
            postConfigureMethod!.Invoke(options, null);
        }

        return options;
    }

    /// <summary>
    /// 配置选项
    /// </summary>
    /// <typeparam name="TOptions">强类型选项类</typeparam>
    /// <returns></returns>
    public static TOptions GetOptions<TOptions>() where TOptions : class, new()
    {
        return GetService<IOptions<TOptions>>()?.Value;
    }

    /// <summary>
    /// 获取当前程序启动Uri信息
    /// <remarks>默认获取第一个地址，可能为空，请勿在程序启动过程中使用</remarks>
    /// </summary>
    /// <returns><see cref="Uri"/></returns>
    public static Uri GetCurrentStartupUri()
    {
        var addresses = GetService<IServer>()?.Features?.Get<IServerAddressesFeature>()?.Addresses?.FirstOrDefault();

        if (string.IsNullOrEmpty(addresses))
        {
            return default;
        }

        return new Uri(addresses);
    }

    /// <summary>
    /// 获取服务注册的生命周期类型
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public static ServiceLifetime? GetServiceLifetime(Type serviceType)
    {
        var serviceDescriptor = InternalServices.FirstOrDefault(u =>
            u.ServiceType == (serviceType.IsGenericType ? serviceType.GetGenericTypeDefinition() : serviceType));

        return serviceDescriptor?.Lifetime;
    }

    /// <summary>
    /// 释放所有未托管的对象
    /// </summary>
    public static void DisposeUnmanagedObjects()
    {
        foreach (var dsp in UnmanagedObjects)
        {
            dsp?.Dispose();
        }

        // 强制手动回收 GC 内存
        if (UnmanagedObjects.IsEmpty)
        {
            var nowTime = DateTime.UtcNow;
            if ((LastGCCollectTime == null || (nowTime - LastGCCollectTime.Value).TotalSeconds > GC_COLLECT_INTERVAL_SECONDS))
            {
                LastGCCollectTime = nowTime;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        UnmanagedObjects.Clear();
    }
}