// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using System.Reflection;
using System.Runtime.CompilerServices;

namespace Fast.DynamicApplication;

/// <summary>
/// 为 <see cref="MethodInfo"/> 提供扩展方法
/// </summary>
internal static class MethodInfoExtension
{
    /// <summary>
    /// 判断方法是否是异步
    /// </summary>
    /// <param name="methodInfo">要检查的方法</param>
    /// <returns>方法由异步状态机生成或返回任务类型时返回 <see langword="true"/>；否则返回 <see langword="false"/></returns>
    public static bool IsAsync(this MethodInfo methodInfo)
    {
        Type returnType = methodInfo.ReturnType;
        return methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>() != null
               || typeof(Task).IsAssignableFrom(returnType)
               || returnType == typeof(ValueTask)
               || (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ValueTask<>));
    }

    /// <summary>
    /// 获取方法真实返回类型
    /// </summary>
    /// <param name="methodInfo">目标方法</param>
    /// <returns>同步方法的声明返回类型；任务方法返回其任务结果类型</returns>
    public static Type GetRealReturnType(this MethodInfo methodInfo)
    {
        bool isAsyncMethod = methodInfo.IsAsync();

        Type returnType = methodInfo.ReturnType;
        return isAsyncMethod ? returnType.GenericTypeArguments.FirstOrDefault() ?? typeof(void) : returnType;
    }
}
