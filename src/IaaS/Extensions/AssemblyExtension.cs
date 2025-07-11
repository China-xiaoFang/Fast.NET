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

using System;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="Assembly"/> 拓展类
/// </summary>
public static class AssemblyExtension
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
    public static string GetDescription(this Assembly assembly)
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
    public static Version GetVersion(this Assembly assembly)
    {
        return assembly.GetName()
            .Version;
    }

    /// <summary>
    /// 获取程序集名称
    /// </summary>
    /// <param name="assembly"><see cref="Assembly"/></param>
    /// <returns><see cref="string"/></returns>
    public static string GetAssemblyName(this Assembly assembly)
    {
        return assembly.GetName()
            .Name;
    }

    /// <summary>
    /// 根据程序集和类型完整限定名获取运行时类型
    /// </summary>
    /// <param name="assembly"><see cref="Assembly"/> 程序集</param>
    /// <param name="typeFullName"><see cref="string"/> 类型完整限定名称</param>
    /// <returns><see cref="Type"/></returns>
    public static Type GetType(Assembly assembly, string typeFullName)
    {
        return assembly.GetType(typeFullName);
    }
}