// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
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
    /// 获取当前请求 TraceId
    /// </summary>
    /// <returns></returns>
    internal static string GetTraceId()
    {
        return IaaSContext.GetTraceId(RootServices, HttpContext);
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
    /// 获取Web主机环境
    /// </summary>
    public static IWebHostEnvironment WebHostEnvironment { get; internal set; }

    /// <summary>
    /// 获取主机环境
    /// </summary>
    public static IHostEnvironment HostEnvironment { get; internal set; }

    /// <summary>
    /// 应用服务
    /// </summary>
    public static IServiceCollection InternalServices { get; internal set; }

    /// <summary>
    /// 存储根服务，可能为空
    /// </summary>
    public static IServiceProvider RootServices { get; internal set; }

    private static IConfiguration _configuration { get; set; }

    /// <summary>
    /// 配置
    /// </summary>
    public static IConfiguration Configuration
    {
        get => CatchOrDefault(() => _configuration.Reload(), new ConfigurationBuilder().Build());
        internal set => _configuration = value;
    }

    /// <summary>
    /// 请求上下文
    /// </summary>
    public static HttpContext HttpContext => CatchOrDefault(() => RootServices?.GetService<IHttpContextAccessor>()?.HttpContext);

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
        return (serviceProvider ?? IaaSContext.GetServiceProvider(type, RootServices, InternalServices, HttpContext))
            .GetService(type);
    }

    /// <summary>
    /// 获取请求生存周期的服务集合
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static IEnumerable<TService> GetServices<TService>(IServiceProvider serviceProvider = default) where TService : class
    {
        return (serviceProvider ?? IaaSContext.GetServiceProvider(typeof(TService), RootServices, InternalServices, HttpContext))
            .GetServices<TService>();
    }

    /// <summary>
    /// 获取请求生存周期的服务集合
    /// </summary>
    /// <param name="type"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static IEnumerable<object> GetServices(Type type, IServiceProvider serviceProvider = default)
    {
        return (serviceProvider ?? IaaSContext.GetServiceProvider(type, RootServices, InternalServices, HttpContext))
            .GetServices(type);
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
        return (serviceProvider ?? IaaSContext.GetServiceProvider(type, RootServices, InternalServices, HttpContext))
            .GetRequiredService(type);
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
}