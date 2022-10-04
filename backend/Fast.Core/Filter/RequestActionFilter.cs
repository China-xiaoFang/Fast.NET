using System.Diagnostics;
using Fast.Core.EventSubscriber;
using Fast.Core.Filter.RequestLimit;
using Fast.Core.Filter.RequestLimit.Internal;
using Furion.EventBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
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

        // 请求参数
        var requestParam = context.ActionArguments.Count < 1 ? null : context.ActionArguments;

        // UA Info
        var userAgentInfo = HttpNewUtil.UserAgentInfo();
        var wanInfo = await HttpNewUtil.WanInfo(HttpNewUtil.Ip);

        var tenantId = GlobalContext.GetTenantId(false);
        var userId = GlobalContext.UserId;

        // 接口限流
        var requestLimitContext =
            await OnActionRequestLimitAsync(httpRequest, actionDescriptor, requestParam, tenantId, userId, wanInfo.Ip);

        var sw = new Stopwatch();
        sw.Start();
        var actionContext = await next();
        sw.Stop();

        // 限流次数增加
        await _requestLimitFilter.AfterCheckAsync(requestLimitContext);

        // 请求日志
        await OnActionRequestLogAsync(context, actionContext, httpRequest, actionDescriptor, requestParam, tenantId, sw,
            userAgentInfo, wanInfo);
    }

    /// <summary>
    /// 请求限制
    /// </summary>
    private async Task<RequestLimitContext> OnActionRequestLimitAsync(HttpRequest httpRequest, ActionDescriptor actionDescriptor,
        IDictionary<string, object> requestParam, long tenantId, long userId, string ip)
    {
        // 是否被允许访问
        var isAllowed = true;
        // 是否检查限流限制
        var isCheck = true;

        var limitCount = _defaultLimit;
        var limitSecond = _defaultSecond;
        var limitKey = $"{httpRequest.Method}{httpRequest.Path}";

        // 接口限流，判断是否存在特性，如果不存在则使用默认的配置
        if (actionDescriptor?.EndpointMetadata.FirstOrDefault(metadata => metadata.GetType() == typeof(RequestLimitAttribute)) is
            RequestLimitAttribute requestLimitAttribute)
        {
            isCheck = requestLimitAttribute.IsCheck;
            if (isCheck)
            {
                limitCount = requestLimitAttribute.Count;
                limitSecond = requestLimitAttribute.Second;
                switch (requestLimitAttribute.RequestLimitType)
                {
                    case RequestLimitTypeEnum.Tenant:
                        limitKey += $"_{tenantId}";
                        break;
                    case RequestLimitTypeEnum.User:
                        limitKey += $"_{tenantId}_{userId}";
                        break;
                    case RequestLimitTypeEnum.Other:
                        // 判断限制Key是否为接口请求参数
                        if (requestLimitAttribute.Key != null)
                        {
                            // 接口请求参数中的Key如果是空，则直接用Ip
                            var requestKey = requestParam?.FirstOrDefault(f => f.Key == requestLimitAttribute.Key);
                            if (requestKey != null)
                            {
                                limitKey = requestKey.Value.ToString();
                            }
                            else
                            {
                                limitKey += $"_{ip}";
                            }
                        }

                        break;
                    case RequestLimitTypeEnum.Ip:
                    default:
                        limitKey += $"_{ip}";
                        break;
                }
            }
        }
        else
        {
            // 如果没有配置规则，则默认根据Ip来
            limitKey += $"_{ip}";
        }

        // 检查接口限流上下文参数
        var requestLimitContext = new RequestLimitContext(limitSecond, limitCount, limitKey, isCheck);

        // 检查接口限流
        isAllowed = await _requestLimitFilter.InvokeAsync(requestLimitContext);

        if (!isAllowed)
            // 抛出StatusCode为429的异常
            throw Oops.Oh(ErrorCode.ApiLimitError).StatusCode(429);

        return requestLimitContext;
    }

    /// <summary>
    /// 记录请求日志
    /// </summary>
    private async Task OnActionRequestLogAsync(ActionExecutingContext context, ActionExecutedContext actionContext,
        HttpRequest httpRequest, ControllerActionDescriptor actionDescriptor, IDictionary<string, object> requestParam,
        long tenantId, Stopwatch sw, UserAgentInfoModel userAgentInfo, WhoisIPInfoModel wanInfo)
    {
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

        await _eventPublisher.PublishAsync(new FastChannelEventSource("Create:OpLog", tenantId,
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