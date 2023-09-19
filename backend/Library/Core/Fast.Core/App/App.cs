// ReSharper disable once CheckNamespace

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Fast.Core.ConfigurableOptions.Internal;
using Fast.Core.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Fast.Core;

/// <summary>
/// App 上下文
/// </summary>
public static class App
{
    /// <summary>
    /// 配置
    /// </summary>
    public static IConfiguration Configuration =>
        CatchOrDefault(() => InternalApp.Configuration.Reload(), new ConfigurationBuilder().Build());

    /// <summary>
    /// 获取Web主机环境
    /// </summary>
    public static IWebHostEnvironment WebHostEnvironment => InternalApp.WebHostEnvironment;

    /// <summary>
    /// 获取泛型主机环境
    /// </summary>
    public static IHostEnvironment HostEnvironment => InternalApp.HostEnvironment;

    /// <summary>
    /// 存储根服务，可能为空
    /// </summary>
    public static IServiceProvider RootServices => InternalApp.RootServices;

    /// <summary>
    /// 应用有效程序集
    /// </summary>
    public static readonly IEnumerable<Assembly> Assemblies;

    /// <summary>
    /// 有效程序集类型
    /// </summary>
    public static readonly IEnumerable<Type> EffectiveTypes;

    /// <summary>
    /// 请求上下文
    /// </summary>
    public static HttpContext HttpContext => CatchOrDefault(() => RootServices?.GetService<IHttpContextAccessor>()?.HttpContext);

    /// <summary>
    /// 未托管的对象集合
    /// </summary>
    public static readonly ConcurrentBag<IDisposable> UnmanagedObjects;

    static App()
    {
        // 未托管的对象
        UnmanagedObjects = new ConcurrentBag<IDisposable>();

        // 加载程序集
        Assemblies = GetAssemblies();

        // 获取有效的类型集合
        EffectiveTypes = Assemblies.SelectMany(GetTypes);
    }

    #region private function

    private static IEnumerable<Assembly> GetAssemblies()
    {
        // 获取入口程序集
        var entryAssembly = Assembly.GetEntryAssembly();

        // 获取入口程序集所引用的所有程序集
        var referencedAssemblies = entryAssembly?.GetReferencedAssemblies();

        // 加载引用的程序集
        var assemblies = referencedAssemblies.Select(Assembly.Load).ToList();

        // 将入口程序集也放入集合
        assemblies.Add(entryAssembly);

        return assemblies;
    }

    /// <summary>
    /// 加载程序集中的所有类型
    /// </summary>
    /// <param name="ass"></param>
    /// <returns></returns>
    private static IEnumerable<Type> GetTypes(Assembly ass)
    {
        var types = Array.Empty<Type>();

        try
        {
            types = ass.GetTypes();
        }
        catch
        {
            Console.WriteLine($"Error load `{ass.FullName}` assembly.");
        }

        return types.Where(u => u.IsPublic);
    }

    /// <summary>
    /// 处理获取对象异常问题
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="action">获取对象委托</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>T</returns>
    private static T CatchOrDefault<T>(Func<T> action, T defaultValue = null) where T : class
    {
        try
        {
            return action();
        }
        catch
        {
            return defaultValue;
        }
    }

    #endregion

    /// <summary>
    /// 解析服务提供器
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public static IServiceProvider GetServiceProvider(Type serviceType)
    {
        // 处理控制台应用程序
        if (HostEnvironment == default)
            return RootServices;

        // 第一选择，判断是否是单例注册且单例服务不为空，如果是直接返回根服务提供器
        if (RootServices != null && InternalApp.InternalServices
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

        var serviceProvider = InternalApp.InternalServices.BuildServiceProvider();
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
    /// <param name="path">配置中对应的Key</param>
    /// <param name="loadPostConfigure"></param>
    /// <returns>TOptions</returns>
    public static TOptions GetConfig<TOptions>(string path, bool loadPostConfigure = false)
    {
        return Configuration.GetSection(path).Get<TOptions>();
    }

    /// <summary>
    /// 获取当前线程 Id
    /// </summary>
    /// <returns></returns>
    public static int GetThreadId()
    {
        return Environment.CurrentManagedThreadId;
    }

    /// <summary>
    /// 获取当前请求 TraceId
    /// </summary>
    /// <returns></returns>
    public static string GetTraceId()
    {
        return Activity.Current?.Id ?? (InternalApp.RootServices == null ? default : HttpContext?.TraceIdentifier);
    }

    /// <summary>
    /// 获取一段代码执行耗时
    /// </summary>
    /// <param name="action">委托</param>
    /// <returns><see cref="long"/></returns>
    public static long GetExecutionTime(Action action)
    {
        // 空检查
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        // 计算接口执行时间
        var timeOperation = Stopwatch.StartNew();
        action();
        timeOperation.Stop();
        return timeOperation.ElapsedMilliseconds;
    }

    /// <summary>
    /// 获取服务注册的生命周期类型
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public static ServiceLifetime? GetServiceLifetime(Type serviceType)
    {
        var serviceDescriptor = InternalApp.InternalServices.FirstOrDefault(u =>
            u.ServiceType == (serviceType.IsGenericType ? serviceType.GetGenericTypeDefinition() : serviceType));

        return serviceDescriptor?.Lifetime;
    }

    /// <summary>
    /// 获取选项
    /// </summary>
    /// <typeparam name="TOptions">强类型选项类</typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns>TOptions</returns>
    public static TOptions GetOptions<TOptions>(IServiceProvider serviceProvider = default) where TOptions : class, new()
    {
        return Penetrates.GetOptionsOnStarting<TOptions>() ??
               GetService<IOptions<TOptions>>(serviceProvider ?? RootServices)?.Value;
    }

    /// <summary>
    /// 获取选项
    /// </summary>
    /// <typeparam name="TOptions">强类型选项类</typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns>TOptions</returns>
    public static TOptions GetOptionsMonitor<TOptions>(IServiceProvider serviceProvider = default) where TOptions : class, new()
    {
        return Penetrates.GetOptionsOnStarting<TOptions>() ??
               GetService<IOptionsMonitor<TOptions>>(serviceProvider ?? RootServices)?.CurrentValue;
    }

    /// <summary>
    /// 获取选项
    /// </summary>
    /// <typeparam name="TOptions">强类型选项类</typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns>TOptions</returns>
    public static TOptions GetOptionsSnapshot<TOptions>(IServiceProvider serviceProvider = default) where TOptions : class, new()
    {
        // 这里不能从根服务解析，因为是 Scoped 作用域
        return Penetrates.GetOptionsOnStarting<TOptions>() ?? GetService<IOptionsSnapshot<TOptions>>(serviceProvider)?.Value;
    }
}