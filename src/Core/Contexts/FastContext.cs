// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using Fast.Runtime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
[SuppressSniffer]
public static class FastContext
{
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
    /// 应用程序构建器
    /// </summary>
    public static IApplicationBuilder WebApplication { get; set; }

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
        get => MAppContext.CatchOrDefault(() => _configuration.Reload(), new ConfigurationBuilder().Build());
        internal set => _configuration = value;
    }

    /// <summary>
    /// 请求上下文
    /// </summary>
    public static HttpContext HttpContext =>
        MAppContext.CatchOrDefault(() => RootServices?.GetService<IHttpContextAccessor>()
            ?.HttpContext);

    /// <summary>
    /// 获取请求生存周期的服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static TService GetService<TService>(IServiceProvider serviceProvider = null) where TService : class
    {
        return GetService(typeof(TService), serviceProvider) as TService;
    }

    /// <summary>
    /// 获取请求生存周期的服务
    /// </summary>
    /// <param name="type"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static object GetService(Type type, IServiceProvider serviceProvider = null)
    {
        return (serviceProvider ?? MAppContext.GetServiceProvider(type, RootServices, InternalServices, HttpContext))
            .GetService(type);
    }

    /// <summary>
    /// 获取请求生存周期的服务集合
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static IEnumerable<TService> GetServices<TService>(IServiceProvider serviceProvider = null) where TService : class
    {
        return (serviceProvider ?? MAppContext.GetServiceProvider(typeof(TService), RootServices, InternalServices, HttpContext))
            .GetServices<TService>();
    }

    /// <summary>
    /// 获取请求生存周期的服务集合
    /// </summary>
    /// <param name="type"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static IEnumerable<object> GetServices(Type type, IServiceProvider serviceProvider = null)
    {
        return (serviceProvider ?? MAppContext.GetServiceProvider(type, RootServices, InternalServices, HttpContext))
            .GetServices(type);
    }

    /// <summary>
    /// 获取请求生存周期的服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static TService GetRequiredService<TService>(IServiceProvider serviceProvider = null) where TService : class
    {
        return GetRequiredService(typeof(TService), serviceProvider) as TService;
    }

    /// <summary>
    /// 获取请求生存周期的服务
    /// </summary>
    /// <param name="type"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static object GetRequiredService(Type type, IServiceProvider serviceProvider = null)
    {
        return (serviceProvider ?? MAppContext.GetServiceProvider(type, RootServices, InternalServices, HttpContext))
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
        path ??= MAppContext.GetOptionName<TOptions>();

        var options = Configuration.GetSection(path)
            .Get<TOptions>();

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
        return GetService<IOptions<TOptions>>()
            ?.Value;
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