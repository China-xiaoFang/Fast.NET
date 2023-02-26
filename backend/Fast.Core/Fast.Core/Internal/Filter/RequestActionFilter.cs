using System.Diagnostics;
using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.AdminFactory.ModelFactory.Sys;
using Fast.Core.Internal.AttributeFilter;
using Fast.Core.Internal.EventSubscriber;
using Fast.Core.Util.Http;
using Fast.Core.Util.Json.Extension;
using Furion.EventBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fast.Core.Internal.Filter;

/// <summary>
/// 请求日志拦截
/// </summary>
public class RequestActionFilter : IAsyncActionFilter
{
    private readonly IEventPublisher _eventPublisher;

    public RequestActionFilter(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpRequest = context.HttpContext.Request;
        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

        // 请求参数
        var requestParam = context.ActionArguments.Count < 1 ? null : context.ActionArguments;

        // UA Info
        var userAgentInfo = HttpNewUtil.UserAgentInfo();
        var wanInfo = await HttpNewUtil.WanInfo(HttpNewUtil.Ip);

        var sw = new Stopwatch();
        sw.Start();
        var actionContext = await next();
        sw.Stop();

        // 判断是否请求成功（没有异常就是请求成功）
        var isRequestSucceed = actionContext.Exception == null;

        // 判断是否具有禁用操作日志属性
        if (actionDescriptor?.EndpointMetadata.Any(metadata => metadata.GetType() == typeof(DisableOpLogAttribute)) == true)
            return;

        // 根据请求方式获取操作名称，如果没有，默认为类名-方法名
        var operationName = httpRequest.Method switch
        {
            "GET" => (actionDescriptor?.EndpointMetadata.FirstOrDefault(
                metadata => metadata.GetType() == typeof(HttpGetAttribute)) as HttpGetAttribute)?.OperationName,
            "POST" => (actionDescriptor?.EndpointMetadata.FirstOrDefault(metadata =>
                metadata.GetType() == typeof(HttpPostAttribute)) as HttpPostAttribute)?.OperationName,
            "PUT" => (actionDescriptor?.EndpointMetadata.FirstOrDefault(
                metadata => metadata.GetType() == typeof(HttpPutAttribute)) as HttpPutAttribute)?.OperationName,
            "DELETE" => (actionDescriptor?.EndpointMetadata.FirstOrDefault(metadata =>
                metadata.GetType() == typeof(HttpDeleteAttribute)) as HttpDeleteAttribute)?.OperationName,
            _ => $"{context.Controller} - {actionDescriptor?.ActionName}"
        };

        //记录请求日志
        await _eventPublisher.PublishAsync(new FastChannelEventSource("Create:OpLog", GlobalContext.GetTenantId(false),
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
                Param = requestParam == null ? "" : requestParam.ToJsonString(),
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