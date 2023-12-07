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

using Fast.NET;
using Fast.SpecificationProcessor.FriendlyException.Metadatas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fast.SpecificationProcessor.FriendlyException.Contexts;

/// <summary>
/// <see cref="ExceptorContext"/> 异常上下文
/// </summary>
internal static class ExceptorContext
{
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