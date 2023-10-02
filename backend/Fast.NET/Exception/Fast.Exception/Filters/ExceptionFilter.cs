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

using Fast.Exception.Handlers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Exception.Filters;

/// <summary>
/// 友好异常拦截器
/// </summary>
public sealed class ExceptionFilter : IAsyncExceptionFilter
{
    /// <summary>
    /// 异常拦截
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        // 判断是否是异常验证
        UserFriendlyException friendlyException = null;
        if (context.Exception is UserFriendlyException exception)
        {
            friendlyException = exception;
        }

        // 解析异常处理服务，实现自定义异常额外操作，如记录日志等
        var globalExceptionHandler = context.HttpContext.RequestServices.GetService<IGlobalExceptionHandler>();
        if (globalExceptionHandler != null)
        {
            await globalExceptionHandler.OnExceptionAsync(context, isFriendlyException: friendlyException != null);
        }
    }
}