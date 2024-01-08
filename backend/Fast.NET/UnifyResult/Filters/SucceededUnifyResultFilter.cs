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

using Fast.IaaS;
using Fast.UnifyResult.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Fast.UnifyResult.Filters;

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
        if (actionExecutedContext.Result is IStatusCodeActionResult statusCodeActionResult &&
            statusCodeActionResult.StatusCode != null)
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
                    if (statusCodeActionResult.StatusCode.Value == StatusCodes.Status401Unauthorized &&
                        httpContext.Request.Headers.ContainsKey("access-token") &&
                        httpContext.Request.Headers.ContainsKey("x-access-token"))
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
            var validationMetadata = ValidatorContext.GetValidationMetadata(badRequestObjectResult.Value);

            var result = unifyResult.OnValidateFailed(context, validationMetadata);

            if (result != null)
            {
                actionExecutedContext.Result = result;
            }
        }
        else
        {
            IActionResult result = default;

            // 检查是否是有效的结果（可进行规范化的结果）
            if (UnifyContext.CheckValidResult(actionExecutedContext.Result, out var data))
            {
                // 判断是否跳过规范化响应数据处理
                if (!UnifyContext.CheckResponseNonUnify(context.HttpContext, controllerActionDescriptor!.MethodInfo,
                        out var unifyResponse))
                {
                    // 处理规范化响应数据
                    data = await unifyResponse.ResponseDataAsync(context.HttpContext.UnifyResponseTimestamp(), data,
                        context.HttpContext);
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