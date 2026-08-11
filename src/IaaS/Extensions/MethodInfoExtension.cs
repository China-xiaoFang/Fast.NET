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

namespace Fast.IaaS;

/// <summary>
/// 为 <see cref="MethodInfo"/> 提供扩展方法。
/// </summary>
public static class MethodInfoExtension
{
    /// <summary>
    /// 判断方法是否是异步。
    /// </summary>
    /// <param name="methodInfo">目标方法的反射元数据。</param>
    /// <returns>满足条件时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
    public static bool IsAsync(this MethodInfo methodInfo)
    {
        var returnType = methodInfo.ReturnType;
        return methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>() != null
               || typeof(Task).IsAssignableFrom(returnType)
               || returnType == typeof(ValueTask)
               || (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ValueTask<>));
    }

    /// <summary>
    /// 获取方法真实返回类型。
    /// </summary>
    /// <param name="methodInfo">目标方法的反射元数据。</param>
    /// <returns>获取到的方法真实返回类型。</returns>
    public static Type GetRealReturnType(this MethodInfo methodInfo)
    {
        var isAsyncMethod = methodInfo.IsAsync();

        var returnType = methodInfo.ReturnType;
        return isAsyncMethod ? returnType.GenericTypeArguments.FirstOrDefault() ?? typeof(void) : returnType;
    }

    /// <summary>
    /// 查找方法指定特性，如果没找到则继续查找声明类。
    /// </summary>
    /// <param name="methodInfo">目标方法的反射元数据。</param>
    /// <param name="inherit">是否在基类型中继续查找成员或特性。</param>
    /// <typeparam name="TAttribute">要查找的特性类型。</typeparam>
    /// <returns>查找方法指定特性，如果没找到则继续查找声明类。</returns>
    public static TAttribute GetFoundAttribute<TAttribute>(this MethodInfo methodInfo, bool inherit) where TAttribute : Attribute
    {
        var declaringType = methodInfo.DeclaringType;

        var attributeType = typeof(TAttribute);

        if (methodInfo.IsDefined(attributeType, inherit))
        {
            return methodInfo.GetCustomAttribute<TAttribute>(inherit);
        }

        // 方法未声明特性时，继续检查其声明类型。
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
    /// 查找方法指定特性，如果没找到则继续查找声明类。
    /// </summary>
    /// <param name="methodInfo">目标方法的反射元数据。</param>
    /// <param name="attributeType">要读取的特性类型。</param>
    /// <param name="inherit">是否在基类型中继续查找成员或特性。</param>
    /// <returns>查找方法指定特性，如果没找到则继续查找声明类。</returns>
    public static Attribute GetFoundAttribute(this MethodInfo methodInfo, Type attributeType, bool inherit)
    {
        var declaringType = methodInfo.DeclaringType;

        if (methodInfo.IsDefined(attributeType, inherit))
        {
            return methodInfo.GetCustomAttribute(attributeType, inherit);
        }

        // 方法未声明特性时，继续检查其声明类型。
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
    /// 获取方法参数数量。
    /// </summary>
    /// <param name="methodInfo">目标方法的反射元数据。</param>
    /// <returns>满足条件的项数。</returns>
    public static int GetMethodParameterCount(this MethodInfo methodInfo)
    {
        return methodInfo.GetParameters()
            .Length;
    }
}
