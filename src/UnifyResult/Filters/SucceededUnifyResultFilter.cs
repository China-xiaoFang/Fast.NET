// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Fast.UnifyResult;

/// <summary>
/// <see cref="SucceededUnifyResultFilter"/> 规范化结构（请求成功）过滤器
/// </summary>
internal class SucceededUnifyResultFilter : IAsyncActionFilter, IOrderedFilter
{
    /// <summary>
    /// 过滤器排序
    /// </summary>
    private const int FilterOrder = 8888;

    /// <summary>
    /// 排序属性
    /// </summary>
    public int Order => FilterOrder;

    /// <summary>
    /// 处理规范化结果
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 执行 Action 并获取结果
        var actionExecutedContext = await next();

        // 排除 WebSocket 请求处理
        if (actionExecutedContext.HttpContext.IsWebSocketRequest())
            return;

        // 处理已经含有状态码结果的 Result
        if (actionExecutedContext.Result is IStatusCodeActionResult statusCodeActionResult
            && statusCodeActionResult.StatusCode != null)
        {
            // 小于 200 或者 大于 299 都不是成功值，直接跳过
            if (statusCodeActionResult.StatusCode.Value < 200 || statusCodeActionResult.StatusCode.Value > 299)
            {
                // 处理规范化结果
                if (!UnifyContext.CheckStatusCodeNonUnify(context.HttpContext, out var failUnifyResult))
                {
                    var httpContext = context.HttpContext;
                    var statusCode = statusCodeActionResult.StatusCode.Value;

                    // 解决刷新 Token 时间和 Token 时间相近问题
                    if (statusCodeActionResult.StatusCode.Value == StatusCodes.Status401Unauthorized
                        && httpContext.Request.Headers.ContainsKey("access-token")
                        && httpContext.Request.Headers.ContainsKey("x-access-token"))
                    {
                        httpContext.Response.StatusCode = statusCode = StatusCodes.Status403Forbidden;
                    }

                    // 如果 Response 已经完成输出，则禁止写入
                    if (httpContext.Response.HasStarted)
                    {
                        return;
                    }

                    await failUnifyResult.OnResponseStatusCodes(httpContext, statusCode);
                }

                return;
            }
        }

        // 如果出现异常，则不会进入该过滤器
        if (actionExecutedContext.Exception != null)
        {
            return;
        }

        // 获取控制器信息
        var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

        // 判断是否跳过规范化处理
        if (UnifyContext.CheckSucceededNonUnify(context.HttpContext, controllerActionDescriptor!.MethodInfo, out var unifyResult))
        {
            return;
        }

        // 处理 BadRequestObjectResult 类型规范化处理
        if (actionExecutedContext.Result is BadRequestObjectResult badRequestObjectResult)
        {
            // 解析验证消息
            var validationMetadata = UnifyContext.GetValidationMetadata(badRequestObjectResult.Value);

            var result = unifyResult.OnValidateFailed(context, validationMetadata);

            if (result != null)
            {
                actionExecutedContext.Result = result;
            }
        }
        else
        {
            IActionResult result = null;

            // 检查是否是有效的结果（可进行规范化的结果）
            if (UnifyContext.CheckValidResult(actionExecutedContext.Result, out var data))
            {
                var timestamp = context.HttpContext.UnifyResponseTimestamp();

                // 判断是否跳过规范化响应数据处理
                if (!UnifyContext.CheckResponseNonUnify(context.HttpContext, controllerActionDescriptor!.MethodInfo,
                        out var unifyResponse))
                {
                    // 处理规范化响应数据
                    data = await unifyResponse.ResponseDataAsync(timestamp, data, context.HttpContext);
                }

                result = unifyResult.OnSucceeded(actionExecutedContext, data);
            }

            // 如果是不能规范化的结果类型，则跳过
            if (result == null)
            {
                return;
            }

            actionExecutedContext.Result = result;
        }
    }
}