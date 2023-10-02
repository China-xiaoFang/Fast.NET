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