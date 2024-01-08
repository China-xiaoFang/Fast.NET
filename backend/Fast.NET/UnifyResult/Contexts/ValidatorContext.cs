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

using System.Collections;
using System.Text.Encodings.Web;
using System.Text.Json;
using Fast.UnifyResult.Metadatas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Fast.UnifyResult.Contexts;

/// <summary>
/// <see cref="ValidatorContext"/> 验证上下文
/// </summary>
internal static class ValidatorContext
{
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
}