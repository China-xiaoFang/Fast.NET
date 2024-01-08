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

using System.Collections;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using Fast.IaaS;
using Fast.UnifyResult.Metadatas;
using Fast.UnifyResult.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.UnifyResult.Contexts;

/// <summary>
/// <see cref="UnifyContext"/> 规范化结果上下文
/// </summary>
internal static class UnifyContext
{
    /// <summary>
    /// 统一返回类型
    /// </summary>
    internal static Type UnifyResultType => typeof(RestfulResult<>);

    /// <summary>
    /// 检查请求成功是否进行规范化处理
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="method"><see cref="MethodInfo"/></param>
    /// <param name="unifyResult"><see cref="IUnifyResultProvider"/></param>
    /// <param name="isWebRequest"><see cref="bool"/></param>
    /// <returns>返回 true 跳过处理，否则进行规范化处理</returns>
    /// <returns><see cref="bool"/></returns>
    internal static bool CheckSucceededNonUnify(HttpContext httpContext, MethodInfo method, out IUnifyResultProvider unifyResult,
        bool isWebRequest = true)
    {
        // 判断返回类型是否包含了规范化处理的返回类型
        var isSkip = method.GetRealReturnType().HasImplementedRawGeneric(UnifyResultType);

        var nonUnifyAttributeType = typeof(NonUnifyAttribute);

        // 这是不使用 method.GetCustomAttribute<NonUnifyAttribute>() != null 的原因是，避免直接继承了 NonUnifyAttribute 使用自定义的特性
        var producesResponseTypeAttributeType = typeof(ProducesResponseTypeAttribute);
        var iApiResponseMetadataProviderType = typeof(IApiResponseMetadataProvider);
        if (!isSkip && method.CustomAttributes.Any(a =>
                // 判断方法头部是否贴有 NonUnifyAttribute 特性
                nonUnifyAttributeType.IsAssignableFrom(a.AttributeType) ||
                // 判断方法头部是否贴有 原生的 HTTP 响应类型的特性 ProducesResponseTypeAttribute
                producesResponseTypeAttributeType.IsAssignableFrom(a.AttributeType) ||
                // 判断方法头部是否贴有 IApiResponseMetadataProvider 特性
                iApiResponseMetadataProviderType.IsAssignableFrom(a.AttributeType)))
        {
            isSkip = true;
        }

        // 判断方法所在的类是否贴有 NonUnifyAttribute 特性
        if (!isSkip && method.ReflectedType?.IsDefined(nonUnifyAttributeType, true) == true)
        {
            isSkip = true;
        }

        // 判断方法所属类型的程序集的名称以 "Microsoft.AspNetCore.OData" 
        if (!isSkip && method.ReflectedType?.Assembly?.GetName()?.Name?.StartsWith("Microsoft.AspNetCore.OData") == true)
        {
            isSkip = true;
        }

        // 判断是否为 Web 请求
        if (!isWebRequest)
        {
            unifyResult = null;
            return isSkip;
        }

        if (isSkip)
        {
            unifyResult = null;
        }
        else
        {
            unifyResult = httpContext?.RequestServices.GetService<IUnifyResultProvider>();
        }

        return unifyResult == null || isSkip;
    }

    /// <summary>
    /// 检查请求失败（验证失败、抛异常）是否进行规范化处理
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="method"><see cref="MethodInfo"/></param>
    /// <param name="unifyResult"><see cref="IUnifyResultProvider"/></param>
    /// <returns>返回 true 跳过处理，否则进行规范化处理</returns>
    /// <returns><see cref="bool"/></returns>
    internal static bool CheckFailedNonUnify(HttpContext httpContext, MethodInfo method, out IUnifyResultProvider unifyResult)
    {
        // 这是不使用 method.GetCustomAttribute<NonUnifyAttribute>() != null 的原因是，避免直接继承了 NonUnifyAttribute 使用自定义的特性
        var nonUnifyAttributeType = typeof(NonUnifyAttribute);

        var producesResponseTypeAttributeType = typeof(ProducesResponseTypeAttribute);
        var iApiResponseMetadataProviderType = typeof(IApiResponseMetadataProvider);

        var isSkip = !method.CustomAttributes.Any(a =>
                         // 判断方法头部是否贴有 NonUnifyAttribute 特性
                         nonUnifyAttributeType.IsAssignableFrom(a.AttributeType) ||
                         // 判断方法头部是否贴有 原生的 HTTP 响应类型的特性 ProducesResponseTypeAttribute
                         producesResponseTypeAttributeType.IsAssignableFrom(a.AttributeType) ||
                         // 判断方法头部是否贴有 IApiResponseMetadataProvider 特性
                         iApiResponseMetadataProviderType.IsAssignableFrom(a.AttributeType)) &&
                     // 判断方法所在的类是否贴有 NonUnifyAttribute 特性
                     method.ReflectedType?.IsDefined(nonUnifyAttributeType, true) == true;

        // 判断方法所属类型的程序集的名称以 "Microsoft.AspNetCore.OData" 
        if (!isSkip && method.ReflectedType?.Assembly?.GetName()?.Name?.StartsWith("Microsoft.AspNetCore.OData") == true)
        {
            isSkip = true;
        }

        if (isSkip)
        {
            unifyResult = null;
        }
        else
        {
            unifyResult = httpContext.RequestServices.GetService<IUnifyResultProvider>();
        }

        return unifyResult == null || isSkip;
    }

    /// <summary>
    /// 检查请求响应数据是否进行规范化处理
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="method"><see cref="MethodInfo"/></param>
    /// <param name="unifyResponse"><see cref="IUnifyResponseProvider"/></param>
    /// <returns>返回 true 跳过处理，否则进行规范化处理</returns>
    /// <returns><see cref="bool"/></returns>
    internal static bool CheckResponseNonUnify(HttpContext httpContext, MethodInfo method,
        out IUnifyResponseProvider unifyResponse)
    {
        // 这是不使用 method.GetCustomAttribute<NonUnifyAttribute>() != null 的原因是，避免直接继承了 NonUnifyAttribute 使用自定义的特性
        var nonUnifyAttributeType = typeof(NonUnifyAttribute);

        var producesResponseTypeAttributeType = typeof(ProducesResponseTypeAttribute);
        var iApiResponseMetadataProviderType = typeof(IApiResponseMetadataProvider);

        var isSkip = !method.CustomAttributes.Any(a =>
                         // 判断方法头部是否贴有 NonUnifyAttribute 特性
                         nonUnifyAttributeType.IsAssignableFrom(a.AttributeType) ||
                         // 判断方法头部是否贴有 原生的 HTTP 响应类型的特性 ProducesResponseTypeAttribute
                         producesResponseTypeAttributeType.IsAssignableFrom(a.AttributeType) ||
                         // 判断方法头部是否贴有 IApiResponseMetadataProvider 特性
                         iApiResponseMetadataProviderType.IsAssignableFrom(a.AttributeType)) &&
                     // 判断方法所在的类是否贴有 NonUnifyAttribute 特性
                     method.ReflectedType?.IsDefined(nonUnifyAttributeType, true) == true;

        // 判断方法所属类型的程序集的名称以 "Microsoft.AspNetCore.OData" 
        if (!isSkip && method.ReflectedType?.Assembly?.GetName()?.Name?.StartsWith("Microsoft.AspNetCore.OData") == true)
        {
            isSkip = true;
        }

        if (isSkip)
        {
            unifyResponse = null;
        }
        else
        {
            unifyResponse = httpContext.RequestServices.GetService<IUnifyResponseProvider>();
        }

        return unifyResponse == null || isSkip;
    }

    /// <summary>
    /// 检查短路状态码（>=400）是否进行规范化处理
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    /// <param name="unifyResult"><see cref="IUnifyResultProvider"/></param>
    /// <returns>返回 true 跳过处理，否则进行规范化处理</returns>
    internal static bool CheckStatusCodeNonUnify(HttpContext httpContext, out IUnifyResultProvider unifyResult)
    {
        // 获取终点路由特性
        var endpointFeature = httpContext.Features.Get<IEndpointFeature>();
        if (endpointFeature == null)
        {
            unifyResult = null;
            return true;
        }

        var nonUnifyAttributeType = typeof(NonUnifyAttribute);

        // 判断终点路由是否存在 NonUnifyAttribute 特性
        var isSkip = httpContext.GetMetadata(nonUnifyAttributeType) != null;

        // 判断终点路由是否存在 NonUnifyAttribute 特性
        if (!isSkip && endpointFeature?.Endpoint?.Metadata?.GetMetadata(nonUnifyAttributeType) != null)
        {
            isSkip = true;
        }

        // 判断请求头部是否包含 odata.metadata=
        if (!isSkip && httpContext?.Request?.Headers["accept"].ToString()
                ?.Contains("odata.metadata=", StringComparison.OrdinalIgnoreCase) == true)
        {
            isSkip = true;
        }

        // 判断请求头部是否包含 odata.streaming=
        if (!isSkip && httpContext?.Request?.Headers["accept"].ToString()
                ?.Contains("odata.streaming=", StringComparison.OrdinalIgnoreCase) == true)
        {
            isSkip = true;
        }

        if (isSkip)
        {
            unifyResult = null;
        }
        else
        {
            unifyResult = httpContext.RequestServices.GetService<IUnifyResultProvider>();
        }

        return unifyResult == null || isSkip;
    }

    /// <summary>
    /// 检查是否是有效的结果（可进行规范化的结果）
    /// </summary>
    /// <param name="result"><see cref="IActionResult"/></param>
    /// <param name="data"><see cref="object"/></param>
    /// <returns></returns>
    internal static bool CheckValidResult(IActionResult result, out object data)
    {
        data = default;

        // 排除以下结果，跳过规范化处理
        var isDataResult = result switch
        {
            ViewResult => false,
            PartialViewResult => false,
            FileResult => false,
            ChallengeResult => false,
            SignInResult => false,
            SignOutResult => false,
            RedirectToPageResult => false,
            RedirectToRouteResult => false,
            RedirectResult => false,
            RedirectToActionResult => false,
            LocalRedirectResult => false,
            ForbidResult => false,
            ViewComponentResult => false,
            PageResult => false,
            NotFoundResult => false,
            NotFoundObjectResult => false,
            _ => true,
        };

        // 目前支持返回值 ActionResult
        if (isDataResult)
            data = result switch
            {
                // 处理内容结果
                ContentResult content => content.Content,
                // 处理对象结果
                ObjectResult obj => obj.Value,
                // 处理 JSON 对象
                JsonResult json => json.Value,
                _ => null,
            };

        return isDataResult;
    }

    /// <summary>
    /// 获取验证错误信息
    /// </summary>
    /// <param name="errors"><see cref="object"/></param>
    /// <returns><see cref="ValidationMetadata"/></returns>
    internal static ValidationMetadata GetValidationMetadata(object errors)
    {
        ModelStateDictionary _modelState = null;
        object validationResults = null;
        string message, firstErrorMessage, firstErrorProperty = default;

        // 判断是否是集合类型
        if (errors is IEnumerable and not string)
        {
            // 如果是模型验证字典类型
            if (errors is ModelStateDictionary modelState)
            {
                _modelState = modelState;
                // 将验证错误信息转换成字典并序列化成 Json
                validationResults = modelState.Where(u => modelState[u.Key]!.ValidationState == ModelValidationState.Invalid)
                    .ToDictionary(u => u.Key, u => modelState[u.Key]?.Errors.Select(c => c.ErrorMessage).ToArray());
            }
            // 如果是 ValidationProblemDetails 特殊类型
            else if (errors is ValidationProblemDetails validation)
            {
                validationResults = validation.Errors.ToDictionary(u => u.Key, u => u.Value.ToArray());
            }
            // 如果是字典类型
            else if (errors is Dictionary<string, string[]> dicResults)
            {
                validationResults = dicResults;
            }

            message = JsonSerializer.Serialize(validationResults,
                new JsonSerializerOptions {Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true});
            firstErrorMessage = ((Dictionary<string, string[]>) validationResults).First().Value[0];
            firstErrorProperty = ((Dictionary<string, string[]>) validationResults).First().Key;
        }
        // 其他类型
        else
        {
            validationResults = firstErrorMessage = message = errors?.ToString();
        }

        return new ValidationMetadata
        {
            ValidationResult = validationResults,
            Message = message,
            ModelState = _modelState,
            FirstErrorProperty = firstErrorProperty,
            FirstErrorMessage = firstErrorMessage
        };
    }

    /// <summary>
    /// 获取异常元数据
    /// </summary>
    /// <param name="context"><see cref="ActionContext"/></param>
    /// <returns><see cref="ExceptionMetadata"/></returns>
    internal static ExceptionMetadata GetExceptionMetadata(ActionContext context)
    {
        object errorCode = default;
        object originErrorCode = default;
        object errors = default;
        object data = default;

        var statusCode = StatusCodes.Status500InternalServerError;
        // 判断是否是验证异常
        var isValidationException = false;

        Exception exception = default;

        // 判断是否是 ExceptionContext
        if (context is ExceptionContext exceptionContext)
        {
            exception = exceptionContext.Exception;
        }

        // 判断是否是 ActionExecutedContext
        if (context is ActionExecutedContext actionExecutedContext)
        {
            exception = actionExecutedContext.Exception;
        }

        // 判断是否是 用户友好异常
        if (exception is UserFriendlyException friendlyException)
        {
            errorCode = friendlyException.ErrorCode;
            originErrorCode = friendlyException.OriginErrorCode;
            statusCode = friendlyException.StatusCode;
            isValidationException = friendlyException.ValidationException;
            errors = friendlyException.ErrorMessage;
            data = friendlyException.Data;
        }

        // 处理非验证失败的错误对象
        if (!isValidationException)
        {
            errors = exception?.InnerException?.Message ?? exception?.Message ?? "Internal Server Error";
        }

        return new ExceptionMetadata
        {
            StatusCode = statusCode,
            ErrorCode = errorCode,
            OriginErrorCode = originErrorCode,
            Errors = errors,
            Data = data
        };
    }
}