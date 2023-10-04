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

using System.Reflection;
using System.Runtime.Loader;

namespace Fast.DependencyInjection.Reflection;

/// <summary>
/// 内部反射静态类
/// </summary>
internal static class Reflect
{
    /// <summary>
    /// 根据程序集名称获取运行时程序集
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    internal static Assembly GetAssembly(string assemblyName)
    {
        // 加载程序集
        return AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
    }

    /// <summary>
    /// 通过流加载程序集
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    internal static Assembly LoadAssembly(MemoryStream assembly)
    {
        return Assembly.Load(assembly.ToArray());
    }

    /// <summary>
    /// 根据程序集名称、类型完整限定名获取运行时类型
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="typeFullName"></param>
    /// <returns></returns>
    internal static Type GetType(string assemblyName, string typeFullName)
    {
        return GetAssembly(assemblyName).GetType(typeFullName);
    }

    /// <summary>
    /// 根据程序集和类型完全限定名获取运行时类型
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="typeFullName"></param>
    /// <returns></returns>
    internal static Type GetType(Assembly assembly, string typeFullName)
    {
        return assembly.GetType(typeFullName);
    }

    /// <summary>
    /// 获取程序集名称
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    internal static string GetAssemblyName(Assembly assembly)
    {
        return assembly.GetName().Name;
    }

    /// <summary>
    /// 获取程序集名称
    /// </summary>
    /// <param name="typeInfo"></param>
    /// <returns></returns>
    internal static string GetAssemblyName(TypeInfo typeInfo)
    {
        return GetAssemblyName(typeInfo.Assembly);
    }
}