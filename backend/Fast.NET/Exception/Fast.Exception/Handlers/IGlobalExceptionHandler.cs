using Microsoft.AspNetCore.Mvc.Filters;

namespace Fast.Core.FriendlyException.Handlers;

/// <summary>
/// 全局异常处理
/// </summary>
public interface IGlobalExceptionHandler
{
    /// <summary>
    /// 异常拦截
    /// </summary>
    /// <param name="context"></param>
    /// <param name="isFriendlyException">是否友好异常</param>
    /// <returns></returns>
    Task OnExceptionAsync(ExceptionContext context, bool isFriendlyException);
}