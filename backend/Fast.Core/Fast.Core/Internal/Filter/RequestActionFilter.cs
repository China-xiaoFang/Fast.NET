using System.Diagnostics;
using System.Text;
using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.AdminFactory.ModelFactory.Sys;
using Fast.Core.AdminFactory.ServiceFactory.Auth.Dto;
using Fast.Core.Const;
using Fast.Core.Internal.AttributeFilter;
using Fast.Core.Internal.EventSubscriber;
using Fast.Core.ServiceCollection.RequestLimit.AttributeFilter;
using Fast.Core.ServiceCollection.RequestLimit.Filter;
using Fast.Core.ServiceCollection.RequestLimit.Internal;
using Fast.Core.Util.Http;
using Fast.Core.Util.Json.Extension;
using Fast.Core.Util.Restful.Internal;
using Furion.EventBus;
using Furion.FriendlyException;
using Furion.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fast.Core.Internal.Filter;

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

        // 演示环境判断
        if (GlobalContext.SystemSettingsOptions.Environment == EnvironmentEnum.Demonstration)
        {
            if (GlobalContext.SystemSettingsOptions.DemoEnvReqDisable.Select(sl => sl.ToString())
                .Any(wh => actionDescriptor?.ActionName.Contains(wh) == true))
            {
                throw Oops.Bah(ErrorCode.DemoEnvNoOperate);
            }
        }

        // 请求参数
        var requestParam = context.ActionArguments.Count < 1 ? null : context.ActionArguments;

        // UA Info
        var userAgentInfo = HttpNewUtil.UserAgentInfo();
        var wanInfo = await HttpNewUtil.WanInfo(HttpNewUtil.Ip);

        var tenantId = GlobalContext.GetTenantId(false);
        var userId = GlobalContext.UserId;

        // 接口限流
        var requestLimitContext = await OnActionRequestLimitAsync(context.HttpContext.Response, httpRequest, actionDescriptor,
            requestParam, tenantId, userId, wanInfo.Ip);

        var sw = new Stopwatch();
        sw.Start();
        var actionContext = await next();
        sw.Stop();

        // 响应报文头增加环境变量
        context.HttpContext.Response.Headers[ClaimConst.EnvironmentCode] =
            $"{GlobalContext.SystemSettingsOptions.Environment.ParseToInt()}";
        context.HttpContext.Response.Headers[ClaimConst.EnvironmentName] =
            GlobalContext.SystemSettingsOptions.Environment.ParseToString();

        // 响应报文头增加接口版本
        context.HttpContext.Response.Headers[ClaimConst.ApiVersion] = GlobalContext.SystemSettingsOptions.ApiVersion;

        // 限流次数增加
        await _requestLimitFilter.AfterCheckAsync(requestLimitContext);

        // 请求日志
        await OnActionRequestLogAsync(context, actionContext, httpRequest, actionDescriptor, requestParam, tenantId, sw,
            userAgentInfo, wanInfo);
    }

    /// <summary>
    /// 请求限制
    /// </summary>
    private async Task<RequestLimitContext> OnActionRequestLimitAsync(HttpResponse httpResponse, HttpRequest httpRequest,
        ActionDescriptor actionDescriptor, IDictionary<string, object> requestParam, long tenantId, long userId, string ip)
    {
        // 是否检查限流限制
        var isCheck = GlobalContext.SystemSettingsOptions.RequestLimit;

        var limitCount = _defaultLimit;
        var limitSecond = _defaultSecond;
        var limitKey = $"{httpRequest.Method}{httpRequest.Path}:";

        // 接口限流，判断是否存在特性，如果不存在则使用默认的配置，这里采用就近原则
        if (actionDescriptor?.EndpointMetadata.LastOrDefault(metadata => metadata.GetType() == typeof(RequestLimitAttribute)) is
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
                        limitKey += $"{tenantId}";
                        break;
                    case RequestLimitTypeEnum.User:
                        limitKey += $"{tenantId}:{userId}";
                        break;
                    case RequestLimitTypeEnum.Other:
                        // 判断限制Key是否为接口请求参数
                        if (requestLimitAttribute.Key == null)
                        {
                            // 接口请求参数中的Key如果是空，则直接用Ip或者UUID，这里建议用UUID
                            var requestKey = requestParam?.FirstOrDefault(f => f.Key == requestLimitAttribute.Key);
                            if (requestKey != null)
                            {
                                limitKey = requestKey.Value.ToString();
                            }
                            else
                            {
                                limitKey += $":{GlobalContext.UUID}";
                            }
                        }

                        break;
                    case RequestLimitTypeEnum.Ip:
                        limitKey += $"{ip}";
                        break;
                    default:
                        limitKey += $"{GlobalContext.UUID}";
                        break;
                }
            }
        }
        else
        {
            // 如果没有配置规则，则默认根据UUID来
            limitKey += $"{GlobalContext.UUID}";
        }

        // 检查接口限流上下文参数
        var requestLimitContext = new RequestLimitContext(limitSecond, limitCount, limitKey, isCheck);

        // 检查接口限流
        var isAllowed = await _requestLimitFilter.InvokeAsync(requestLimitContext);

        if (isAllowed)
            return requestLimitContext;

        // 抛出StatusCode为429的异常
        httpResponse.StatusCode = StatusCodes.Status429TooManyRequests;
        throw Oops.Bah(ErrorCode.ApiLimitError);
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