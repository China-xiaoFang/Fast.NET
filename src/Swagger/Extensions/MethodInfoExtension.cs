// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System.Reflection;

namespace Fast.Swagger;

/// <summary>
/// 为 <see cref="MethodInfo"/> 提供扩展方法
/// </summary>
internal static class MethodInfoExtension
{
    /// <summary>
    /// 查找方法指定特性，如果没找到则继续查找声明类
    /// </summary>
    /// <param name="methodInfo">目标方法的反射元数据</param>
    /// <param name="inherit">是否在基类型中继续查找成员或特性</param>
    /// <typeparam name="TAttribute">要查找的特性类型</typeparam>
    /// <returns>查找方法指定特性，如果没找到则继续查找声明类</returns>
    public static TAttribute GetFoundAttribute<TAttribute>(this MethodInfo methodInfo, bool inherit) where TAttribute : Attribute
    {
        Type declaringType = methodInfo.DeclaringType;

        Type attributeType = typeof(TAttribute);

        if (methodInfo.IsDefined(attributeType, inherit))
        {
            return methodInfo.GetCustomAttribute<TAttribute>(inherit);
        }

        // 方法未声明特性时，继续检查其声明类型
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
}
