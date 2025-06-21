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

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Fast.Runtime;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace

namespace System;

/// <summary>
/// <see cref="MAppContext"/> Microsoft App 上下文
/// </summary>
[SuppressSniffer]
public static class MAppContext
{
    #region 内部属性

    /// <summary>
    /// GC 回收默认间隔
    /// </summary>
    private const int GC_COLLECT_INTERVAL_SECONDS = 5;

    /// <summary>
    /// 记录最近 GC 回收时间
    /// </summary>
    private static DateTime? LastGCCollectTime { get; set; }

    #endregion

    /// <summary>
    /// 入口程序集版本号
    /// </summary>
    public static readonly string AssemblyVersion;

    /// <summary>
    /// 应用有效程序集
    /// </summary>
    public static readonly IEnumerable<Assembly> Assemblies;

    /// <summary>
    /// 有效程序集类型
    /// </summary>
    /// <remarks>所有公共的类型</remarks>
    public static readonly IEnumerable<Type> Types;

    /// <summary>
    /// 有效程序集类型
    /// </summary>
    /// <remarks>排除使用了 <see cref="SuppressSnifferAttribute"/> 特性的类型</remarks>
    public static readonly IEnumerable<Type> EffectiveTypes;

    /// <summary>
    /// 未托管的对象集合
    /// </summary>
    public static ConcurrentBag<IDisposable> UnmanagedObjects { get; private set; }

    static MAppContext()
    {
        // 未托管的对象
        UnmanagedObjects = new ConcurrentBag<IDisposable>();

        // 加载入口程序集
        var entryAssembly = Assembly.GetEntryAssembly();

        // 获取入口程序集版本号
        AssemblyVersion = entryAssembly?.GetName()
            .Version?.ToString();

        // 获取所有程序集
        Assemblies = entryAssembly.GetEntryReferencedAssembly();

        // 获取有效的类型集合
        // ReSharper disable once PossibleMultipleEnumeration
        Types = Assemblies.SelectMany(assembly => assembly.GetAssemblyTypes());

        // 获取排除使用了 SuppressSnifferAttribute 特性的类型
        var suppressSnifferAttributeType = typeof(SuppressSnifferAttribute);
        // ReSharper disable once PossibleMultipleEnumeration
        EffectiveTypes = Assemblies.SelectMany(assembly =>
            assembly.GetAssemblyTypes(wh => !wh.IsDefined(suppressSnifferAttributeType, false)));
    }

    /// <summary>
    /// 处理获取对象异常问题
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="action">获取对象委托</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>T</returns>
    public static T CatchOrDefault<T>(Func<T> action, T defaultValue = null) where T : class
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

    /// <summary>
    /// 获取选项名称
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <returns></returns>
    public static string GetOptionName<TOptions>() where TOptions : class, new()
    {
        // 默认后缀
        const string defaultSuffix = "Options";

        var optionsType = typeof(TOptions);

        // 判断是否已 “Options” 结尾
        return optionsType.Name.EndsWith(defaultSuffix) ? optionsType.Name[..^defaultSuffix.Length] : optionsType.Name;
    }

    /// <summary>
    /// 解析服务提供器
    /// </summary>
    /// <param name="serviceType"></param>
    /// <param name="rootServices"></param>
    /// <param name="internalServices"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static IServiceProvider GetServiceProvider(Type serviceType, IServiceProvider rootServices,
        IServiceCollection internalServices, HttpContext httpContext)
    {
        // 第一选择，判断是否是单例注册且单例服务不为空，如果是直接返回根服务提供器
        if (rootServices != null
            && internalServices.Where(u =>
                    u.ServiceType == (serviceType.IsGenericType ? serviceType.GetGenericTypeDefinition() : serviceType))
                .Any(u => u.Lifetime == ServiceLifetime.Singleton))
            return rootServices;

        // 第二选择是获取 HttpContext 对象的 RequestServices
        if (httpContext?.RequestServices != null)
            return httpContext.RequestServices;

        // 第三选择，创建新的作用域并返回服务提供器
        if (rootServices != null)
        {
            var scoped = rootServices.CreateScope();
            UnmanagedObjects.Add(scoped);
            return scoped.ServiceProvider;
        }

        // 第四选择，构建新的服务对象（性能最差）
        var serviceProvider = internalServices.BuildServiceProvider();
        UnmanagedObjects.Add(serviceProvider);
        return serviceProvider;
    }

    /// <summary>
    /// 获取当前程序启动Uri信息
    /// </summary>
    /// <remarks>默认获取第一个地址，可能为空，请勿在程序启动过程中使用</remarks>
    /// <param name="server"><see cref="IServer"/></param>
    /// <returns><see cref="Uri"/></returns>
    public static Uri GetCurrentStartupUri(IServer server)
    {
        var addresses = server?.Features.Get<IServerAddressesFeature>()
            ?.Addresses.FirstOrDefault();

        if (string.IsNullOrEmpty(addresses))
        {
            return null;
        }

        return new Uri(addresses);
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
    public static string GetTraceId(IServiceProvider rootServices, HttpContext httpContext)
    {
        return Activity.Current?.Id ?? (rootServices == null ? null : httpContext?.TraceIdentifier);
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
    /// 添加未托管的对象
    /// </summary>
    /// <param name="dsp"></param>
    public static void AddUnmanagedObjects(IDisposable dsp)
    {
        UnmanagedObjects.Add(dsp);
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
            if (LastGCCollectTime == null || (nowTime - LastGCCollectTime.Value).TotalSeconds > GC_COLLECT_INTERVAL_SECONDS)
            {
                LastGCCollectTime = nowTime;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        UnmanagedObjects.Clear();
    }
}