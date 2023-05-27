using System.Diagnostics;
using Fast.Core.AdminEnum;
using Fast.Core.AdminModel.Sys.Log;
using Fast.Core.RabbitMQ.EventSubscriber;
using Fast.Iaas.Attributes;
using Fast.Iaas.Extension;
using Fast.Iaas.Internal;
using Fast.Iaas.Util.Http;
using Furion.EventBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fast.Core.Filter;

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
        var userAgentInfo = HttpUtil.UserAgentInfo();
        //var wanInfo = await HttpUtil.WanInfo(HttpUtil.Ip);

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
        string operationName;
        HttpRequestActionEnum? operationAction = null;
        switch (httpRequest.Method)
        {
            case "GET":
                var httpGetAttribute =
                    (actionDescriptor?.EndpointMetadata.FirstOrDefault(metadata => metadata.GetType() == typeof(HttpGetAttribute))
                        as HttpGetAttribute);
                operationName = httpGetAttribute?.OperationName;
                operationAction = httpGetAttribute?.Action;
                break;
            case "POST":
                var httpPostAttribute =
                    (actionDescriptor?.EndpointMetadata.FirstOrDefault(
                        metadata => metadata.GetType() == typeof(HttpPostAttribute)) as HttpPostAttribute);
                operationName = httpPostAttribute?.OperationName;
                operationAction = httpPostAttribute?.Action;
                break;
            case "PUT":
                var httpPutAttribute =
                    (actionDescriptor?.EndpointMetadata.FirstOrDefault(metadata => metadata.GetType() == typeof(HttpPutAttribute))
                        as HttpPutAttribute);
                operationName = httpPutAttribute?.OperationName;
                operationAction = httpPutAttribute?.Action;
                break;
            case "DELETE":
                var httpDeleteAttribute =
                    (actionDescriptor?.EndpointMetadata.FirstOrDefault(metadata =>
                        metadata.GetType() == typeof(HttpDeleteAttribute)) as HttpDeleteAttribute);
                operationName = httpDeleteAttribute?.OperationName;
                operationAction = httpDeleteAttribute?.Action;
                break;
            default:
                operationName = $"{context.Controller} - {actionDescriptor?.ActionName}";
                break;
        }

        //记录请求日志
        await _eventPublisher.PublishAsync(new FastChannelEventSource("Create:OpLog", UserContext.TenantId, new SysLogOpModel
        {
            Account = UserContext.UserAccount,
            UserName = UserContext.UserName,
            Success = isRequestSucceed ? YesOrNotEnum.Y : YesOrNotEnum.N,
            OperationAction = operationAction,
            OperationName = operationName,
            ClassName = context.Controller.ToString(),
            MethodName = actionDescriptor?.ActionName,
            Url = httpRequest.Path,
            ReqMethod = httpRequest.Method,
            Param = requestParam == null ? "" : requestParam.ToJsonString(),
            Result = isRequestSucceed
                ? actionContext.Result?.GetType() == typeof(JsonResult) ? actionContext.Result.ToJsonString() : ""
                : actionContext.Exception.Message,
            //Location = HttpUtil.Url,
            ElapsedTime = sw.ElapsedMilliseconds,
            OpTime = DateTime.Now,
            PhoneModel = userAgentInfo.PhoneModel,
            OS = userAgentInfo.OS,
            Browser = userAgentInfo.Browser,
            //Province = wanInfo.Pro,
            //City = wanInfo.City,
            //Operator = wanInfo.Operator,
            //Ip = wanInfo.Ip,
        }));
    }
}