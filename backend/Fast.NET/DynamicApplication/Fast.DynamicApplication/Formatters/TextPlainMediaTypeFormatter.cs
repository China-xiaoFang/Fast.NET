using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Fast.DynamicApplication.Formatters;

/// <summary>
/// text/plain 请求 Body 参数支持
/// </summary>
public sealed class TextPlainMediaTypeFormatter : TextInputFormatter
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public TextPlainMediaTypeFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain"));

        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    /// <summary>
    /// 重写 <see cref="ReadRequestBodyAsync(InputFormatterContext, Encoding)"/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="effectiveEncoding"></param>
    /// <returns></returns>
    public async override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context,
        Encoding effectiveEncoding)
    {
        using var reader = new StreamReader(context.HttpContext.Request.Body, effectiveEncoding);
        var stringContent = await reader.ReadToEndAsync();

        return await InputFormatterResult.SuccessAsync(stringContent);
    }
}