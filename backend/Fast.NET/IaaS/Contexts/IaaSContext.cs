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
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

// 只允许框架内部的类库访问
[assembly: InternalsVisibleTo("Fast.NET.Core")]
[assembly: InternalsVisibleTo("Fast.DependencyInjection")]
[assembly: InternalsVisibleTo("Fast.DynamicApplication")]
[assembly: InternalsVisibleTo("Fast.FriendlyException")]
[assembly: InternalsVisibleTo("Fast.DataValidation")]
[assembly: InternalsVisibleTo("Fast.UnifyResult")]
[assembly: InternalsVisibleTo("Fast.JwtBearer")]
[assembly: InternalsVisibleTo("Fast.Logging")]
[assembly: InternalsVisibleTo("Fast.Mapster")]
[assembly: InternalsVisibleTo("Fast.SpecificationProcessor")]
[assembly: InternalsVisibleTo("Fast.SqlSugar")]

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="IaaSContext"/> 框架内部的常量，公共方法配置类
/// </summary>
// ReSharper disable once PartialTypeWithSinglePart
internal static class IaaSContext
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

    /// <summary>
    /// 应用有效程序集
    /// </summary>
    public static readonly IEnumerable<Assembly> Assemblies;

    /// <summary>
    /// 有效程序集类型
    /// </summary>
    public static readonly IEnumerable<Type> EffectiveTypes;

    /// <summary>
    /// 未托管的对象集合
    /// </summary>
    public static ConcurrentBag<IDisposable> UnmanagedObjects { get; private set; }

    /// <summary>
    /// 控制器排序集合
    /// </summary>
    public static ConcurrentDictionary<string, (string, int, Type)> ControllerOrderCollection { get; set; }

    /// <summary>
    /// <see cref="IsApiController(Type)"/> 缓存集合
    /// </summary>
    private static readonly ConcurrentDictionary<Type, bool> IsApiControllerCached;

    /// <summary>
    /// IDynamicApplication 接口类型
    /// </summary>
    private static readonly Type IDynamicApplicationType;

    static IaaSContext()
    {
        // 未托管的对象
        UnmanagedObjects = new ConcurrentBag<IDisposable>();

        // 加载程序集
        Assemblies = AssemblyUtil.GetEntryAssembly();

        var suppressSnifferAttributeType = typeof(SuppressSnifferAttribute);

        // 获取有效的类型集合
        // ReSharper disable once PossibleMultipleEnumeration
        EffectiveTypes = Assemblies.SelectMany(s =>
            // 排除使用了 SuppressSnifferAttribute 特性的类型
            s.GetAssemblyTypes(wh => !wh.IsDefined(suppressSnifferAttributeType, false)));

        ControllerOrderCollection = new ConcurrentDictionary<string, (string, int, Type)>();

        IsApiControllerCached = new ConcurrentDictionary<Type, bool>();

        try
        {
            IDynamicApplicationType =
                AssemblyUtil.GetType("Fast.DynamicApplication", "Fast.DynamicApplication.IDynamicApplication");
        }
        catch
        {
            IDynamicApplicationType = null;
        }
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
    internal static string GetTraceId(IServiceProvider rootServices, HttpContext httpContext)
    {
        return Activity.Current?.Id ?? (rootServices == null ? default : httpContext?.TraceIdentifier);
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
    /// 解析服务提供器
    /// </summary>
    /// <param name="serviceType"></param>
    /// <param name="RootServices"></param>
    /// <param name="internalServices"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static IServiceProvider GetServiceProvider(Type serviceType, IServiceProvider RootServices,
        IServiceCollection internalServices, HttpContext httpContext)
    {
        // 第一选择，判断是否是单例注册且单例服务不为空，如果是直接返回根服务提供器
        if (RootServices != null && internalServices
                .Where(u => u.ServiceType == (serviceType.IsGenericType ? serviceType.GetGenericTypeDefinition() : serviceType))
                .Any(u => u.Lifetime == ServiceLifetime.Singleton))
            return RootServices;

        // 第二选择是获取 HttpContext 对象的 RequestServices
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
        var serviceProvider = internalServices.BuildServiceProvider();
        UnmanagedObjects.Add(serviceProvider);
        return serviceProvider;
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
    /// 是否是Api控制器
    /// </summary>
    /// <param name="type">type</param>
    /// <returns></returns>
    public static bool IsApiController(Type type)
    {
        return IsApiControllerCached.GetOrAdd(type, Function);

        // 本地静态方法
        static bool Function(Type type)
        {
            if (type == null)
            {
                return false;
            }

            // 排除 OData 控制器
            if (type.Assembly.GetName().Name?.StartsWith("Microsoft.AspNetCore.OData") == true)
            {
                return false;
            }

            // 不能是非公开，基元类型，值类型，抽象类，接口，泛型类
            if (!type.IsPublic || type.IsPrimitive || type.IsValueType || type.IsAbstract || type.IsInterface ||
                type.IsGenericType)
            {
                return false;
            }

            // 继承 ControllerBase 或 实现 IApplication 的类型
            if ((!typeof(Controller).IsAssignableFrom(type) && typeof(ControllerBase).IsAssignableFrom(type)) ||
                IDynamicApplicationType?.IsAssignableFrom(type) == true)
            {
                return true;
            }

            return false;
        }
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