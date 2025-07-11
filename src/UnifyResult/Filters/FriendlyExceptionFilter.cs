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
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Fast.UnifyResult;

/// <summary>
/// <see cref="FriendlyExceptionFilter"/> 友好异常拦截器
/// </summary>
internal sealed class FriendlyExceptionFilter : IAsyncExceptionFilter
{
    /// <summary>
    /// 异常拦截
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        var isUserFriendlyException = false;
        var isValidationException = false;

        // 判断是否为友好异常
        if (context.Exception is UserFriendlyException userFriendlyException)
        {
            isUserFriendlyException = true;

            // 判断是否为验证异常
            if (userFriendlyException.ValidationException)
            {
                isValidationException = true;
            }
        }

        // 解析异常处理服务，实现自定义操作
        var globalExceptionHandler = context.HttpContext.RequestServices.GetService<IGlobalExceptionHandler>();

        if (globalExceptionHandler != null)
        {
            await globalExceptionHandler.OnExceptionAsync(context, isUserFriendlyException, isValidationException);
        }

        // 排除 WebStock 请求处理
        if (context.HttpContext.IsWebSocketRequest())
        {
            return;
        }

        // 如果异常在其他地方被标记处理，那么这里不再处理
        if (context.ExceptionHandled)
        {
            return;
        }

        // 解析异常信息
        var exceptionMetadata = UnifyContext.GetExceptionMetadata(context);

        // 判断是否是 Razor Pages
        var isPageDescriptor = context.ActionDescriptor is CompiledPageActionDescriptor;

        // 判断是否是验证异常，如果是，则不处理
        if (isValidationException)
        {
            // 从 HttpContext 上下文中读取验证执行结果
            var resultHttpContext = context.HttpContext.Items[nameof(DataValidationFilter) + nameof(UserFriendlyException)];

            if (resultHttpContext != null)
            {
                var result = (resultHttpContext as ActionExecutedContext)?.Result;

                // 直接将验证结果设置为异常结果
                context.Result = result
                                 ?? new BadPageResult(StatusCodes.Status400BadRequest)
                                 {
                                     Code = UnifyContext
                                         .GetValidationMetadata((context.Exception as UserFriendlyException)?.ErrorMessage)
                                         .Message
                                 };

                // 标记验证异常已被处理
                context.ExceptionHandled = true;
                return;
            }
        }

        // 处理 Razor Pages
        if (isPageDescriptor)
        {
            // 返回自定义错误页面
            context.Result =
                new BadPageResult(isValidationException ? StatusCodes.Status400BadRequest : exceptionMetadata.StatusCode)
                {
                    Title = isValidationException ? "ModelState Invalid" : "Internal Server: " + exceptionMetadata.Errors,
                    Code = isValidationException
                        ? UnifyContext.GetValidationMetadata((context.Exception as UserFriendlyException)?.ErrorMessage)
                            .Message
                        : context.Exception.ToString()
                };
        }
        // 处理 Mvc/WebApi
        else
        {
            // 获取控制器信息
            if (context.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor)
            {
                return;
            }

            // 判断是否跳过规范化结果，如果是，则只处理友好异常消息
            if (UnifyContext.CheckFailedNonUnify(context.HttpContext, controllerActionDescriptor.MethodInfo, out var unifyResult))
            {
                // 返回 JsonResult
                context.Result = new JsonResult(exceptionMetadata.Errors) {StatusCode = exceptionMetadata.StatusCode};
            }
            else
            {
                int? statusCode = null;
                string message = null;
                // 判断是否跳过规范化响应数据处理
                if (!UnifyContext.CheckResponseNonUnify(context.HttpContext, controllerActionDescriptor!.MethodInfo,
                        out var unifyResponse))
                {
                    // 处理规范化响应数据
                    (statusCode, message) =
                        await unifyResponse.ResponseExceptionAsync(context, exceptionMetadata, context.HttpContext);
                }

                // 执行规范化异常处理
                context.Result = unifyResult.OnException(context, exceptionMetadata, statusCode, message);
            }
        }
    }
}