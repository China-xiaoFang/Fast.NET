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

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

// 只允许框架内部的类库访问
[assembly: InternalsVisibleTo("Fast.NET.Core")]
[assembly: InternalsVisibleTo("Fast.DependencyInjection")]
[assembly: InternalsVisibleTo("Fast.JwtBearer")]
[assembly: InternalsVisibleTo("Fast.Logging")]
[assembly: InternalsVisibleTo("Fast.Mapster")]
[assembly: InternalsVisibleTo("Fast.SpecificationProcessor")]
[assembly: InternalsVisibleTo("Fast.SqlSugar")]

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="FastContext"/> 框架内部的常量，公共方法配置类
/// </summary>
// ReSharper disable once PartialTypeWithSinglePart
internal static class FastContext
{
    /// <summary>
    /// 应用有效程序集
    /// </summary>
    public static readonly IEnumerable<Assembly> Assemblies;

    /// <summary>
    /// 有效程序集类型
    /// </summary>
    public static readonly IEnumerable<Type> EffectiveTypes;

    static FastContext()
    {
        // 加载程序集
        Assemblies = AssemblyUtil.GetEntryAssembly();

        var suppressSnifferAttributeType = typeof(SuppressSnifferAttribute);

        // 获取有效的类型集合
        // ReSharper disable once PossibleMultipleEnumeration
        EffectiveTypes = Assemblies.SelectMany(s =>
            // 排除使用了 SuppressSnifferAttribute 特性的类型
            AssemblyUtil.GetAssemblyTypes(s, wh => !wh.IsDefined(suppressSnifferAttributeType, false)));
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
}