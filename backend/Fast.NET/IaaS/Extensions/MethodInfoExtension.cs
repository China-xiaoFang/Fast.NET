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
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="MethodInfo"/> 拓展类
/// </summary>
[SuppressSniffer]
public static class MethodInfoExtension
{
    /// <summary>
    /// 判断方法是否是异步
    /// </summary>
    /// <param name="methodInfo"><see cref="MemberInfo"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsAsync(this MethodInfo methodInfo)
    {
        return methodInfo.GetCustomAttribute<AsyncMethodBuilderAttribute>() != null ||
               methodInfo.ReturnType.ToString().StartsWith(typeof(Task).FullName);
    }

    /// <summary>
    /// 获取方法真实返回类型
    /// </summary>
    /// <param name="methodInfo"><see cref="MethodInfo"/></param>
    /// <returns><see cref="Type"/></returns>
    public static Type GetRealReturnType(this MethodInfo methodInfo)
    {
        // 判断是否是异步方法
        var isAsyncMethod = methodInfo.IsAsync();

        // 获取类型返回值并处理 Task 和 Task<T> 类型返回值
        var returnType = methodInfo.ReturnType;
        return isAsyncMethod ? (returnType.GenericTypeArguments.FirstOrDefault() ?? typeof(void)) : returnType;
    }

    /// <summary>
    /// 查找方法指定特性，如果没找到则继续查找声明类
    /// </summary>
    /// <typeparam name="TAttribute"></typeparam>
    /// <param name="methodInfo"></param>
    /// <param name="inherit"></param>
    /// <returns></returns>
    public static TAttribute GetFoundAttribute<TAttribute>(this MethodInfo methodInfo, bool inherit) where TAttribute : Attribute
    {
        // 获取方法所在类型
        var declaringType = methodInfo.DeclaringType;

        var attributeType = typeof(TAttribute);

        // 判断方法是否定义了指定特性
        if (methodInfo.IsDefined(attributeType, inherit))
        {
            // 直接返回
            return methodInfo.GetCustomAttribute<TAttribute>(inherit);
        }

        // 没有找到，查找方法所在的类型，是否定义了特性
        if (declaringType == null)
        {
            return default;
        }

        if (declaringType.IsDefined(attributeType, inherit))
        {
            return declaringType.GetCustomAttribute<TAttribute>(inherit);
        }

        return default;
    }

    /// <summary>
    /// 查找方法指定特性，如果没找到则继续查找声明类
    /// </summary>
    /// <param name="methodInfo"></param>
    /// <param name="attributeType"></param>
    /// <param name="inherit"></param>
    /// <returns></returns>
    public static Attribute GetFoundAttribute(this MethodInfo methodInfo, Type attributeType, bool inherit)
    {
        // 获取方法所在类型
        var declaringType = methodInfo.DeclaringType;

        // 判断方法是否定义了指定特性
        if (methodInfo.IsDefined(attributeType, inherit))
        {
            // 直接返回
            return methodInfo.GetCustomAttribute(attributeType, inherit);
        }

        // 没有找到，查找方法所在的类型，是否定义了特性
        if (declaringType == null)
        {
            return default;
        }

        if (declaringType.IsDefined(attributeType, inherit))
        {
            return declaringType.GetCustomAttribute(attributeType, inherit);
        }

        return default;
    }

    /// <summary>
    /// 获取方法参数数量
    /// </summary>
    /// <param name="methodInfo"><see cref="MemberInfo"/></param>
    /// <returns><see cref="int"/></returns>
    public static int GetMethodParameterCount(this MethodInfo methodInfo)
    {
        return methodInfo.GetParameters().Length;
    }
}