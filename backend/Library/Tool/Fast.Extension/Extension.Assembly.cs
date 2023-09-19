using System.Reflection;

namespace Fast.Extensions;

/// <summary>
/// 程序集扩展
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// 获取所有类型
    /// </summary>
    /// <param name="assembly"><see cref="Assembly"/></param>
    /// <param name="exported">类型导出设置</param>
    /// <returns><see cref="Type"/>[]</returns>
    public static Type[] GetTypes(this Assembly assembly, bool exported)
    {
        return exported ? assembly.GetExportedTypes() : assembly.GetTypes();
    }

    /// <summary>
    /// 获取程序集描述
    /// </summary>
    /// <param name="assembly"><see cref="Assembly"/></param>
    /// <returns><see cref="string"/></returns>
    public static string? GetDescription(this Assembly assembly)
    {
        var descriptionAttribute =
            Attribute.GetCustomAttribute(assembly, typeof(AssemblyDescriptionAttribute)) as AssemblyDescriptionAttribute;

        return descriptionAttribute?.Description;
    }

    /// <summary>
    /// 获取程序集版本
    /// </summary>
    /// <param name="assembly"><see cref="Assembly"/></param>
    /// <returns><see cref="string"/></returns>
    public static Version? GetVersion(this Assembly assembly)
    {
        return assembly.GetName().Version;
    }
}