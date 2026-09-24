// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Fast.DynamicApplication;

/// <summary>
/// text/plain 请求 Body 参数支持
/// </summary>
internal sealed class TextPlainMediaTypeFormatter : TextInputFormatter
{
    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    public TextPlainMediaTypeFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain"));

        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    /// <inheritdoc />
    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context,
        Encoding effectiveEncoding)
    {
        using var reader = new StreamReader(context.HttpContext.Request.Body, effectiveEncoding);
        string stringContent = await reader.ReadToEndAsync();

        return await InputFormatterResult.SuccessAsync(stringContent);
    }
}
