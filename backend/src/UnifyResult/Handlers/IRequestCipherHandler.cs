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
using Fast.UnifyResult.Inputs;
using Microsoft.AspNetCore.Http;

// ReSharper disable once CheckNamespace
namespace Fast.UnifyResult;

/// <summary>
/// <see cref="IRequestCipherHandler"/> 请求解密响应加密处理
/// </summary>
public interface IRequestCipherHandler
{
    /// <summary>
    /// 请求解密
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/> 请求上下文</param>
    /// <param name="requestMethod"><see cref="HttpRequestMethodEnum"/> 请求方式</param>
    /// <param name="plaintextData"><see cref="IDictionary{TKey,TValue}"/> 明文数据（解密后的数据）</param>
    /// <param name="ciphertextData"><see cref="RequestDecryptInput"/> 密文数据（请求源数据）</param>
    /// <returns></returns>
    Task DecipherAsync(HttpContext httpContext, HttpRequestMethodEnum requestMethod, IDictionary<string, object> plaintextData,
        RequestDecryptInput ciphertextData);

    /// <summary>
    /// 响应加密
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/> 请求上下文</param>
    /// <param name="requestMethod"><see cref="HttpRequestMethodEnum"/> 请求方式</param>
    /// <param name="plaintextData"><see cref="object"/> 明文数据（返回源数据）</param>
    /// <param name="ciphertextData"><see cref="object"/> 密文数据（加密后的数据）</param>
    /// <returns></returns>
    Task EncipherAsync(HttpContext httpContext, HttpRequestMethodEnum requestMethod, object plaintextData, object ciphertextData);

    /// <summary>
    /// 解密异常
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/> 请求上下文</param>
    /// <param name="exception"><see cref="Exception"/> 异常信息</param>
    /// <returns></returns>
    Task DecipherExceptionAsync(HttpContext httpContext, Exception exception);

    /// <summary>
    /// 加密异常
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/> 请求上下文</param>
    /// <param name="exception"><see cref="Exception"/> 异常信息</param>
    /// <returns></returns>
    Task EncipherExceptionAsync(HttpContext httpContext, Exception exception);
}