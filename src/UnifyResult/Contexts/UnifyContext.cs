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

using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Fast.DynamicApplication")]

// ReSharper disable once CheckNamespace
namespace Fast.UnifyResult;

/// <summary>
/// <see cref="UnifyContext"/> 规范化结果上下文
/// </summary>
[SuppressSniffer]
public static class UnifyContext
{
    /// <summary>
    /// 是否启用规范化结果
    /// </summary>
    public static bool EnabledUnifyHandler = false;

    /// <summary>
    /// 统一返回类型
    /// </summary>
    public static Type UnifyResultType => typeof(RestfulResult<>);

    /// <summary>
    /// 处理 Restful 响应状态码
    /// </summary>
    /// <param name="code"><see cref="int"/> 状态码</param>
    /// <param name="data"><see cref="object"/> 数据</param>
    /// <param name="message"><see cref="string"/> 错误信息</param>
    /// <param name="httpContext"><see cref="HttpContext"/> 请求上下文</param>
    /// <returns></returns>
    public static RestfulResult<object> HandleRestfulStatusCode(int code, object data, string message, HttpContext httpContext)
    {
        return code switch
        {
            // 处理 400 状态码
            StatusCodes.Status400BadRequest => GetRestfulResult(StatusCodes.Status400BadRequest, false, data,
                message ?? "400 请求无效", httpContext),
            // 处理 401 状态码
            StatusCodes.Status401Unauthorized => GetRestfulResult(StatusCodes.Status401Unauthorized, false, data,
                message ?? "401 未经授权", httpContext),
            // 处理 403 状态码
            StatusCodes.Status403Forbidden => GetRestfulResult(StatusCodes.Status403Forbidden, false, data,
                message ?? "403 无操作权限", httpContext),
            // 处理 404 状态码
            StatusCodes.Status404NotFound => GetRestfulResult(StatusCodes.Status404NotFound, false, data, message ?? "404 无效的地址",
                httpContext),
            // 处理 405 状态码
            StatusCodes.Status405MethodNotAllowed => GetRestfulResult(StatusCodes.Status405MethodNotAllowed, false, data,
                message ?? "405 方法不被允许", httpContext),
            // 处理 409 状态码
            StatusCodes.Status409Conflict => GetRestfulResult(StatusCodes.Status409Conflict, false, data, message ?? "409 请求冲突",
                httpContext),
            // 处理 410 状态码
            StatusCodes.Status410Gone => GetRestfulResult(StatusCodes.Status410Gone, false, data, message ?? "410 资源已失效或永久删除",
                httpContext),
            // 处理 415 状态码
            StatusCodes.Status415UnsupportedMediaType => GetRestfulResult(StatusCodes.Status415UnsupportedMediaType, false, data,
                message ?? "415 不支持的媒体类型", httpContext),
            // 处理 422 状态码
            StatusCodes.Status422UnprocessableEntity => GetRestfulResult(StatusCodes.Status422UnprocessableEntity, false, data,
                message ?? "422 请求语义错误，参数验证失败", httpContext),
            // 处理 429 状态码
            StatusCodes.Status429TooManyRequests => GetRestfulResult(StatusCodes.Status429TooManyRequests, false, data,
                message ?? "429 频繁请求", httpContext),
            // 处理 500 状态码
            StatusCodes.Status500InternalServerError => GetRestfulResult(StatusCodes.Status500InternalServerError, false, data,
                message ?? "500 服务器内部错误", httpContext),
            // 处理 502 状态码
            StatusCodes.Status502BadGateway => GetRestfulResult(StatusCodes.Status502BadGateway, false, data,
                message ?? "502 网关错误", httpContext),
            // 处理 503 状态码
            StatusCodes.Status503ServiceUnavailable => GetRestfulResult(StatusCodes.Status503ServiceUnavailable, false, data,
                message ?? "503 服务不可用", httpContext),
            // 处理 504 状态码
            StatusCodes.Status504GatewayTimeout => GetRestfulResult(StatusCodes.Status504GatewayTimeout, false, data,
                message ?? "504 网关超时", httpContext),
            _ => GetRestfulResult(StatusCodes.Status500InternalServerError, false, data, message, httpContext)
        };
    }

    /// <summary>
    /// 获取规范化RESTful风格返回值
    /// </summary>
    /// <param name="code"><see cref="int"/> 状态码</param>
    /// <param name="success"><see cref="bool"/> 执行成功</param>
    /// <param name="data"><see cref="object"/> 数据</param>
    /// <param name="message"><see cref="string"/> 错误信息</param>
    /// <param name="httpContext"><see cref="HttpContext"/> 请求上下文</param>
    /// <returns></returns>
    public static RestfulResult<object> GetRestfulResult(int code, bool success, object data, object message,
        HttpContext httpContext)
    {
        // 从请求响应头部中获取时间戳
        var timestamp = httpContext.UnifyResponseTimestamp();

        return new RestfulResult<object>
        {
            Code = code,
            Success = success,
            Data = data,
            Message = message,
            Timestamp = timestamp
        };
    }

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
        var isSkip = method.GetRealReturnType()
            .HasImplementedRawGeneric(UnifyResultType);

        var nonUnifyAttributeType = typeof(NonUnifyAttribute);

        // 这是不使用 method.GetCustomAttribute<NonUnifyAttribute>() != null 的原因是，避免直接继承了 NonUnifyAttribute 使用自定义的特性
        var producesResponseTypeAttributeType = typeof(ProducesResponseTypeAttribute);
        var iApiResponseMetadataProviderType = typeof(IApiResponseMetadataProvider);
        if (!isSkip
            && method.CustomAttributes.Any(a =>
                // 判断方法头部是否贴有 NonUnifyAttribute 特性
                nonUnifyAttributeType.IsAssignableFrom(a.AttributeType)
                ||
                // 判断方法头部是否贴有 原生的 HTTP 响应类型的特性 ProducesResponseTypeAttribute
                producesResponseTypeAttributeType.IsAssignableFrom(a.AttributeType)
                ||
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
        if (!isSkip
            && method.ReflectedType?.Assembly.GetName()
                .Name?.StartsWith("Microsoft.AspNetCore.OData")
            == true)
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
                         nonUnifyAttributeType.IsAssignableFrom(a.AttributeType)
                         ||
                         // 判断方法头部是否贴有 原生的 HTTP 响应类型的特性 ProducesResponseTypeAttribute
                         producesResponseTypeAttributeType.IsAssignableFrom(a.AttributeType)
                         ||
                         // 判断方法头部是否贴有 IApiResponseMetadataProvider 特性
                         iApiResponseMetadataProviderType.IsAssignableFrom(a.AttributeType))
                     &&
                     // 判断方法所在的类是否贴有 NonUnifyAttribute 特性
                     method.ReflectedType?.IsDefined(nonUnifyAttributeType, true) == true;

        // 判断方法所属类型的程序集的名称以 "Microsoft.AspNetCore.OData" 
        if (!isSkip
            && method.ReflectedType?.Assembly.GetName()
                .Name?.StartsWith("Microsoft.AspNetCore.OData")
            == true)
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
                         nonUnifyAttributeType.IsAssignableFrom(a.AttributeType)
                         ||
                         // 判断方法头部是否贴有 原生的 HTTP 响应类型的特性 ProducesResponseTypeAttribute
                         producesResponseTypeAttributeType.IsAssignableFrom(a.AttributeType)
                         ||
                         // 判断方法头部是否贴有 IApiResponseMetadataProvider 特性
                         iApiResponseMetadataProviderType.IsAssignableFrom(a.AttributeType))
                     &&
                     // 判断方法所在的类是否贴有 NonUnifyAttribute 特性
                     method.ReflectedType?.IsDefined(nonUnifyAttributeType, true) == true;

        // 判断方法所属类型的程序集的名称以 "Microsoft.AspNetCore.OData" 
        if (!isSkip
            && method.ReflectedType?.Assembly.GetName()
                .Name?.StartsWith("Microsoft.AspNetCore.OData")
            == true)
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
        if (!isSkip && endpointFeature.Endpoint?.Metadata.GetMetadata(nonUnifyAttributeType) != null)
        {
            isSkip = true;
        }

        // 判断请求头部是否包含 odata.metadata=
        if (!isSkip
            && httpContext.Request.Headers["accept"]
                .ToString()
                .Contains("odata.metadata=", StringComparison.OrdinalIgnoreCase))
        {
            isSkip = true;
        }

        // 判断请求头部是否包含 odata.streaming=
        if (!isSkip
            && httpContext.Request.Headers["accept"]
                .ToString()
                .Contains("odata.streaming=", StringComparison.OrdinalIgnoreCase))
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
        data = null;

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
            _ => true
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
                _ => null
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
        string message, firstErrorMessage, firstErrorProperty = null;

        // 判断是否是集合类型
        if (errors is IEnumerable and not string)
        {
            // 如果是模型验证字典类型
            if (errors is ModelStateDictionary modelState)
            {
                _modelState = modelState;
                // 将验证错误信息转换成字典并序列化成 Json
                validationResults = modelState.Where(u => modelState[u.Key]!.ValidationState == ModelValidationState.Invalid)
                    .ToDictionary(u => u.Key, u => modelState[u.Key]
                        ?.Errors.Select(c => c.ErrorMessage)
                        .ToArray());
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
            firstErrorMessage = ((Dictionary<string, string[]>) validationResults).First()
                .Value[0];
            firstErrorProperty = ((Dictionary<string, string[]>) validationResults).First()
                .Key;
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
        object errorCode = null;
        object originErrorCode = null;
        object errors = null;
        object data = null;

        var statusCode = StatusCodes.Status500InternalServerError;
        // 判断是否是验证异常
        var isValidationException = false;

        Exception exception = null;

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