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
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="MethodInfo"/> 拓展类
/// </summary>
public static class MethodInfoExtension
{
    /// <summary>
    /// 判断方法是否是异步
    /// </summary>
    /// <param name="methodInfo"><see cref="MemberInfo"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsAsync(this MethodInfo methodInfo)
    {
        return methodInfo.GetCustomAttribute<AsyncMethodBuilderAttribute>() != null
               || methodInfo.ReturnType.ToString()
                   .StartsWith(typeof(Task).FullName);
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
        return isAsyncMethod ? returnType.GenericTypeArguments.FirstOrDefault() ?? typeof(void) : returnType;
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
            return null;
        }

        if (declaringType.IsDefined(attributeType, inherit))
        {
            return declaringType.GetCustomAttribute<TAttribute>(inherit);
        }

        return null;
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
            return null;
        }

        if (declaringType.IsDefined(attributeType, inherit))
        {
            return declaringType.GetCustomAttribute(attributeType, inherit);
        }

        return null;
    }

    /// <summary>
    /// 获取方法参数数量
    /// </summary>
    /// <param name="methodInfo"><see cref="MemberInfo"/></param>
    /// <returns><see cref="int"/></returns>
    public static int GetMethodParameterCount(this MethodInfo methodInfo)
    {
        return methodInfo.GetParameters()
            .Length;
    }
}