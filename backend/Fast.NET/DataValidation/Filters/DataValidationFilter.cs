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

using System.Reflection;
using Fast.DataValidation;
using Fast.IaaS;
using Fast.SpecificationProcessor.DataValidation.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Fast.SpecificationProcessor.DataValidation.Filters;

/// <summary>
/// <see cref="DataValidationFilter"/> 数据验证拦截器
/// </summary>
internal sealed class DataValidationFilter : IAsyncActionFilter, IOrderedFilter
{
    /// <summary>
    /// Api 行为配置选项
    /// </summary>
    private readonly ApiBehaviorOptions _apiBehaviorOptions;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options"></param>
    public DataValidationFilter(IOptions<ApiBehaviorOptions> options)
    {
        _apiBehaviorOptions = options.Value;
    }

    /// <summary>
    /// 过滤器排序
    /// </summary>
    private const int FilterOrder = -1000;

    /// <summary>
    /// 排序属性
    /// </summary>
    public int Order => FilterOrder;

    /// <summary>
    /// 拦截请求
    /// </summary>
    /// <param name="context">动作方法上下文</param>
    /// <param name="next">中间件委托</param>
    /// <returns></returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 排除 WebSocket 请求处理
        if (context.HttpContext.IsWebSocketRequest())
        {
            await next();
            return;
        }

        // 获取控制器/方法信息
        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

        // 跳过验证类型
        var nonValidationAttributeType = typeof(NonValidationAttribute);
        var method = actionDescriptor?.MethodInfo;

        // 获取验证状态
        var modelState = context.ModelState;

        // 如果参数数量为 0 或贴了 [NonValidation] 特性 或所在类型贴了 [NonValidation] 特性或验证成功或已经设置了结果，则跳过验证
        if (actionDescriptor?.Parameters?.Count == 0 || method?.IsDefined(nonValidationAttributeType, true) == true ||
            method?.DeclaringType?.IsDefined(nonValidationAttributeType, true) == true || modelState.IsValid ||
            method?.DeclaringType?.Assembly?.GetName()?.Name?.StartsWith("Microsoft.AspNetCore.OData") == true ||
            context.Result != null)
        {
            await CallUnHandleResult(context, next, actionDescriptor, method);
            return;
        }

        // 处理执行前验证信息
        var handledResult = HandleValidation(context, method, actionDescriptor, modelState);

        // 处理 Mvc 未处理结果情况
        if (!handledResult)
        {
            await CallUnHandleResult(context, next, actionDescriptor, method);
        }
    }

    /// <summary>
    /// 调用未处理的结果类型
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <param name="actionDescriptor"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    private async Task CallUnHandleResult(ActionExecutingContext context, ActionExecutionDelegate next,
        ControllerActionDescriptor actionDescriptor, MethodInfo method)
    {
        // 处理执行后验证信息
        var resultContext = await next();

        // 如果异常不为空且属于友好验证异常
        if (resultContext.Exception != null && resultContext.Exception is UserFriendlyException userFriendlyException &&
            userFriendlyException.ValidationException)
        {
            // 存储验证执行结果
            context.HttpContext.Items[nameof(DataValidationFilter) + nameof(UserFriendlyException)] = resultContext;

            // 处理验证信息
            _ = HandleValidation(context, method, actionDescriptor, userFriendlyException.ErrorMessage, resultContext,
                userFriendlyException);
        }
    }

    /// <summary>
    /// 内部处理异常
    /// </summary>
    /// <param name="context"></param>
    /// <param name="method"></param>
    /// <param name="actionDescriptor"></param>
    /// <param name="errors"></param>
    /// <param name="resultContext"></param>
    /// <param name="userFriendlyException"></param>
    /// <returns>返回 false 表示结果没有处理</returns>
    private bool HandleValidation(ActionExecutingContext context, MethodInfo method, ControllerActionDescriptor actionDescriptor,
        object errors, ActionExecutedContext resultContext = default, UserFriendlyException userFriendlyException = default)
    {
        dynamic finalContext = resultContext != null ? resultContext : context;

        // 解析验证消息
        var validationMetadata = ValidatorContext.GetValidationMetadata(errors);
        validationMetadata.ErrorCode = userFriendlyException?.ErrorCode;
        validationMetadata.OriginErrorCode = userFriendlyException?.OriginErrorCode;
        validationMetadata.StatusCode = userFriendlyException?.StatusCode;
        validationMetadata.Data = userFriendlyException?.Data;

        // 存储验证信息
        context.HttpContext.Items[nameof(DataValidationFilter) + nameof(ValidationMetadata)] = validationMetadata;

        // 判断是否跳过规范化结果，如果跳过，返回 400 BadRequestResult
        if (UnifyContext.CheckFailedNonUnify(context.HttpContext, actionDescriptor.MethodInfo, out var unifyResult))
        {
            // WebAPI 情况
            if (IaaSContext.IsApiController(method.DeclaringType))
            {
                // 如果不启用 SuppressModelStateInvalidFilter，则跳过，理应手动验证
                if (!_apiBehaviorOptions.SuppressModelStateInvalidFilter)
                {
                    finalContext.Result = _apiBehaviorOptions.InvalidModelStateResponseFactory(context);
                }
                else
                {
                    // 返回 JsonResult
                    finalContext.Result = new JsonResult(validationMetadata.ValidationResult)
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
            }
            else
            {
                // 返回自定义错误页面
                finalContext.Result = new BadPageResult(StatusCodes.Status400BadRequest) {Code = validationMetadata.Message};
            }
        }
        else
        {
            // 执行规范化异常处理
            finalContext.Result = unifyResult.OnValidateFailed(context, validationMetadata);
        }

        return true;
    }
}