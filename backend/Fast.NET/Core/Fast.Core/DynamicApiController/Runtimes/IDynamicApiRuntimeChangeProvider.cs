using System.Reflection;

namespace Fast.Core.DynamicApiController.Runtimes;

/// <summary>
/// 动态 WebAPI 运行时感知提供器
/// </summary>
public interface IDynamicApiRuntimeChangeProvider
{
    /// <summary>
    /// 添加程序集
    /// </summary>
    /// <param name="assemblies">程序集</param>
    void AddAssemblies(params Assembly[] assemblies);

    /// <summary>
    /// 添加程序集并立即感知变化
    /// </summary>
    /// <param name="assemblies">程序集</param>
    void AddAssembliesWithNotifyChanges(params Assembly[] assemblies);

    /// <summary>
    /// 移除程序集
    /// </summary>
    /// <param name="assemblyNames">程序集名称</param>
    void RemoveAssemblies(params string[] assemblyNames);

    /// <summary>
    /// 移除程序集
    /// </summary>
    /// <param name="assemblies">程序集</param>
    void RemoveAssemblies(params Assembly[] assemblies);

    /// <summary>
    /// 移除程序集并立即感知变化
    /// </summary>
    /// <param name="assemblyNames">程序集名称</param>
    void RemoveAssembliesWithNotifyChanges(params string[] assemblyNames);

    /// <summary>
    /// 移除程序集并立即感知变化
    /// </summary>
    /// <param name="assemblies">程序集</param>
    void RemoveAssembliesWithNotifyChanges(params Assembly[] assemblies);

    /// <summary>
    /// 感知变化
    /// </summary>
    void NotifyChanges();
}