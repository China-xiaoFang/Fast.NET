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

using System.Text;
using Fast.IaaS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fast.SpecificationProcessor.FriendlyException.Results;

/// <summary>
/// <see cref="BadPageResult"/> 错误页面
/// </summary>
public class BadPageResult : StatusCodeResult
{
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = "ModelState Invalid";

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = "User data verification failed. Please input it correctly.";

    /// <summary>
    /// 图标
    /// </summary>
    /// <remarks>必须是 base64 类型</remarks>
    public string Base64Icon { get; set; } =
        "data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTYiIGhlaWdodD0iMTYiIGZpbGw9Im5vbmUiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+PHBhdGggZD0iTTE0LjIxIDEzLjVsMS43NjcgMS43NzMtLjcwNC43MDRMMTMuNSAxNC4yMWwtMS43NzMgMS43NzMtLjcwNC0uNzEgMS43NzQtMS43NzQtMS43NzQtMS43NzMuNzA0LS43MDQgMS43NzMgMS43NzQgMS43NzMtMS43NzQuNzA0LjcxMUwxNC4yMSAxMy41ek0yIDE1aDh2MUgxVjBoOC43MUwxNCA0LjI5VjEwaC0xVjVIOVYxSDJ2MTR6bTgtMTFoMi4yOUwxMCAxLjcxVjR6IiBmaWxsPSIjMTAxMDEwIi8+PC9zdmc+";

    /// <summary>
    /// 错误代码
    /// </summary>
    public string Code { get; set; } = "";

    /// <summary>
    /// 错误代码语言
    /// </summary>
    public string CodeLang { get; set; } = "json";

    /// <summary>
    /// 返回通用 401 错误页
    /// </summary>
    public static BadPageResult Status401Unauthorized =>
        new(StatusCodes.Status401Unauthorized)
        {
            Title = "401 Unauthorized", Code = "401 Unauthorized", Description = "", CodeLang = "txt"
        };

    /// <summary>
    /// 返回通用 403 错误页
    /// </summary>
    public static BadPageResult Status403Forbidden =>
        new(StatusCodes.Status403Forbidden) {Title = "403 Forbidden", Code = "403 Forbidden", Description = "", CodeLang = "txt"};

    /// <summary>
    /// 返回通用 404 错误页
    /// </summary>
    public static BadPageResult Status404NotFound =>
        new(StatusCodes.Status404NotFound) {Title = "404 Not Found", Code = "404 Not Found", Description = "", CodeLang = "txt"};

    /// <summary>
    /// 返回通用 500 错误页
    /// </summary>
    public static BadPageResult Status500InternalServerError =>
        new(StatusCodes.Status500InternalServerError)
        {
            Title = "500 Internal Server Error", Code = "500 Internal Server Error", Description = "", CodeLang = "txt"
        };

    /// <summary>
    /// 
    /// </summary>
    public BadPageResult() : base(400)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Microsoft.AspNetCore.Mvc.StatusCodeResult" /> class
    /// with the given <paramref name="statusCode" />.
    /// </summary>
    /// <param name="statusCode">The HTTP status code of the response.</param>
    public BadPageResult(int statusCode) : base(statusCode)
    {
    }

    /// <summary>
    /// 重写返回结果
    /// </summary>
    /// <param name="context"></param>
    public override void ExecuteResult(ActionContext context)
    {
        // 如果 Response 已经完成输出或 WebSocket 请求，则禁止写入
        if (context.HttpContext.IsWebSocketRequest() || context.HttpContext.Response.HasStarted)
        {
            return;
        }

        base.ExecuteResult(context);

        context.HttpContext.Response.Body.Write(Encoding.UTF8.GetBytes(ToString()));
    }

    /// <summary>
    /// 将 <see cref="BadPageResult"/> 转换成字符串
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
    {
        // 获取当前类型信息
        var thisType = typeof(BadPageResult);
        var thisAssembly = thisType.Assembly;

        // 读取嵌入式页面路径
        var errorHtml =
            $"{thisAssembly.GetAssemblyName()}{thisType.Namespace?.Replace(nameof(Fast), string.Empty)}.Assets.error.html";

        // 解析嵌入式文件流
        using var readStream = thisAssembly.GetManifestResourceStream(errorHtml);

        if (readStream != null)
        {
            var buffer = new byte[readStream.Length];
            _ = readStream.Read(buffer, 0, buffer.Length);

            // 读取内容并替换
            var content = Encoding.UTF8.GetString(buffer);
            content = content.Replace($"@{{{nameof(Title)}}}", Title).Replace($"@{{{nameof(Description)}}}", Description)
                .Replace($"@{{{nameof(StatusCode)}}}", StatusCode.ToString()).Replace($"@{{{nameof(Code)}}}", Code)
                .Replace($"@{{{nameof(CodeLang)}}}", CodeLang).Replace($"@{{{nameof(Base64Icon)}}}", Base64Icon);

            return content;
        }

        throw new NullReferenceException("The embedded resource file error.html could not be found");
    }
}