// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
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
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.UnifyResult.Filters;

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
                context.Result = result ?? new BadPageResult(StatusCodes.Status400BadRequest)
                {
                    Code = UnifyContext.GetValidationMetadata((context.Exception as UserFriendlyException)?.ErrorMessage)
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
                    Title = isValidationException ? "ModelState Invalid" : ("Internal Server: " + exceptionMetadata.Errors),
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
                // WebAPI 情况
                if (IaaSContext.IsApiController(controllerActionDescriptor.MethodInfo.DeclaringType))
                {
                    // 返回 JsonResult
                    context.Result = new JsonResult(exceptionMetadata.Errors) {StatusCode = exceptionMetadata.StatusCode,};
                }
                else
                {
                    // 返回自定义错误页面
                    context.Result = new BadPageResult(exceptionMetadata.StatusCode)
                    {
                        Title = "Internal Server: " + exceptionMetadata.Errors, Code = context.Exception.ToString()
                    };
                }
            }
            else
            {
                // 执行规范化异常处理
                context.Result = unifyResult.OnException(context, exceptionMetadata);
            }
        }
    }
}