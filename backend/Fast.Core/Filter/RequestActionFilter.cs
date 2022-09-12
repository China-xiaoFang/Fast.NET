using System.Diagnostics;
using Fast.Core.EventSubscriber;
using Fast.Core.Filter.RequestLimit;
using Fast.Core.Filter.RequestLimit.Internal;
using Furion.EventBus;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fast.Core.Filter;

/// <summary>
/// 请求日志拦截
/// </summary>
public class RequestActionFilter : IAsyncActionFilter
{
    private readonly IEventPublisher _eventPublisher;
    private readonly IRequestLimitFilter _requestLimitFilter;

    /// <summary>
    /// 默认限制秒
    /// </summary>
    public const int _defaultSecond = 1;

    /// <summary>
    /// 默认限制次数
    /// </summary>
    private const int _defaultLimit = 1;

    public RequestActionFilter(IEventPublisher eventPublisher, IRequestLimitFilter requestLimitFilter)
    {
        _eventPublisher = eventPublisher;
        _requestLimitFilter = requestLimitFilter;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpRequest = context.HttpContext.Request;
        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

        var limitCount = _defaultLimit;
        var limitSecond = _defaultSecond;
        var limitKey = $"{httpRequest.Method}{httpRequest.Path}";

        // 接口限流，判断是否存在特性，如果不存在则使用默认的配置
        if (actionDescriptor?.EndpointMetadata.FirstOrDefault(metadata => metadata.GetType() == typeof(RequestLimitAttribute)) is
            RequestLimitAttribute requestLimitAttribute)
        {
            limitCount = requestLimitAttribute.Count;
            limitSecond = requestLimitAttribute.Second;
            // 判断限制Key是否为接口请求参数
            if (requestLimitAttribute.Key != null)
            {
                // 接口请求参数
                limitKey = "请求参数";
            }
        }

        // 检查接口限流上下文参数
        var requestLimitContext = new RequestLimitContext(limitKey, limitSecond, limitCount);

        // 检查接口限流
        var isAllowed = await _requestLimitFilter.InvokeAsync(requestLimitContext);

        if (!isAllowed)
            throw Oops.Bah(ErrorCode.ApiLimitError);

        var sw = new Stopwatch();
        sw.Start();
        var actionContext = await next();
        sw.Stop();

        // 限流次数增加
        await _requestLimitFilter.AfterCheckAsync(requestLimitContext);

        // 判断是否请求成功（没有异常就是请求成功）
        var isRequestSucceed = actionContext.Exception == null;

        // 操作名称
        var operationName = "";

        // 特性判断
        if (actionDescriptor?.EndpointMetadata != null)
        {
            // 判断是否具有禁用操作日志属性
            if (actionDescriptor.EndpointMetadata.Any(metadata => metadata.GetType() == typeof(DisableOpLogAttribute)))
                return;

            // 根据请求方式获取操作名称，如果没有，默认为类名-方法名
            operationName = httpRequest.Method switch
            {
                "GET" => (actionDescriptor.EndpointMetadata.FirstOrDefault(metadata =>
                    metadata.GetType() == typeof(HttpGetAttribute)) as HttpGetAttribute)?.OperationName,
                "POST" => (actionDescriptor.EndpointMetadata.FirstOrDefault(metadata =>
                    metadata.GetType() == typeof(HttpPostAttribute)) as HttpPostAttribute)?.OperationName,
                "PUT" => (actionDescriptor.EndpointMetadata.FirstOrDefault(metadata =>
                    metadata.GetType() == typeof(HttpPutAttribute)) as HttpPutAttribute)?.OperationName,
                "DELETE" => (actionDescriptor.EndpointMetadata.FirstOrDefault(metadata =>
                    metadata.GetType() == typeof(HttpDeleteAttribute)) as HttpDeleteAttribute)?.OperationName,
                _ => $"{context.Controller} - {actionDescriptor?.ActionName}"
            };
        }

        // UA Info
        var userAgentInfo = HttpNewUtil.UserAgentInfo();
        var wanInfo = HttpNewUtil.WanInfo(HttpNewUtil.Ip).Result;

        await _eventPublisher.PublishAsync(new FastChannelEventSource("Create:OpLog", 123,
            new SysLogOpModel
            {
                Account = GlobalContext.UserAccount,
                Name = GlobalContext.UserName,
                Success = isRequestSucceed ? YesOrNotEnum.Y : YesOrNotEnum.N,
                OperationName = operationName,
                ClassName = context.Controller.ToString(),
                MethodName = actionDescriptor?.ActionName,
                Url = httpRequest.Path,
                ReqMethod = httpRequest.Method,
                Param = context.ActionArguments.Count < 1 ? "" : context.ActionArguments.ToJsonString(),
                Result = isRequestSucceed
                    ? actionContext.Result?.GetType() == typeof(JsonResult) ? actionContext.Result.ToJsonString() : ""
                    : actionContext.Exception.Message,
                Location = HttpNewUtil.Url,
                ElapsedTime = sw.ElapsedMilliseconds,
                OpTime = DateTime.Now,
                PhoneModel = userAgentInfo.PhoneModel,
                OS = userAgentInfo.OS,
                Browser = userAgentInfo.Browser,
                Province = wanInfo.Pro,
                City = wanInfo.City,
                Operator = wanInfo.Operator,
                Ip = wanInfo.Ip,
            }));
    }
}