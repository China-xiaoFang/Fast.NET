using Furion.EventBus;
using Furion.Logging;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fast.Core.Filter;

/// <summary>
/// 全局异常处理
/// </summary>
public class LogExceptionHandler : IGlobalExceptionHandler, ISingleton
{
    private readonly IEventPublisher _eventPublisher;

    public LogExceptionHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        // 判断Code是否为400，400为业务错误，非系统异常不记录
        var isValidationException = context.Exception is AppFriendlyException {StatusCode: 400};

        if (!isValidationException)
        {
            await _eventPublisher.PublishAsync(new ChannelEventSource("Create:ExLog",
                new SysLogExModel
                {
                    Account = GlobalContext.UserAccount,
                    Name = GlobalContext.UserName,
                    ClassName = context.Exception.TargetSite?.DeclaringType?.FullName,
                    MethodName = context.Exception.TargetSite?.Name,
                    ExceptionName = context.Exception.Message,
                    ExceptionMsg = context.Exception.Message,
                    ExceptionSource = context.Exception.Source,
                    StackTrace = context.Exception.StackTrace,
                    ParamsObj = context.Exception.TargetSite?.GetParameters().ToString(),
                    ExceptionTime = DateTime.Now,
                    TenantId = GlobalContext.TenantId
                }));

            // 写日志文件
            Log.Error(context.Exception.ToString());
        }
    }
}