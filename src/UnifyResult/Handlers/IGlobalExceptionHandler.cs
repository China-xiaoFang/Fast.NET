// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using Microsoft.AspNetCore.Mvc.Filters;

namespace Fast.UnifyResult;

/// <summary>
/// 全局异常处理
/// </summary>
public interface IGlobalExceptionHandler
{
    /// <summary>
    /// 异常拦截
    /// </summary>
    /// <param name="context">当前异常处理上下文</param>
    /// <param name="isUserFriendlyException">是否将异常作为可直接展示给用户的异常处理</param>
    /// <param name="isValidationException">是否将异常作为参数验证异常处理</param>
    /// <returns>表示异步“异常拦截”操作的任务</returns>
    Task OnExceptionAsync(ExceptionContext context, bool isUserFriendlyException, bool isValidationException);
}
