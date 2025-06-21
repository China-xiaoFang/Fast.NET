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

using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace

namespace Fast.UnifyResult;

/// <summary>
/// <see cref="BadPageResult"/> 错误页面
/// </summary>
[SuppressSniffer]
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
            $"{thisAssembly.GetName().Name}{thisType.Namespace?.Replace(nameof(Fast), string.Empty)}.Assets.error.html";

        // 解析嵌入式文件流
        using var readStream = thisAssembly.GetManifestResourceStream(errorHtml);

        if (readStream != null)
        {
            var buffer = new byte[readStream.Length];
            _ = readStream.Read(buffer, 0, buffer.Length);

            // 读取内容并替换
            var content = Encoding.UTF8.GetString(buffer);
            content = content.Replace($"@{{{nameof(Title)}}}", Title)
                .Replace($"@{{{nameof(Description)}}}", Description)
                .Replace($"@{{{nameof(StatusCode)}}}", StatusCode.ToString())
                .Replace($"@{{{nameof(Code)}}}", Code)
                .Replace($"@{{{nameof(CodeLang)}}}", CodeLang)
                .Replace($"@{{{nameof(Base64Icon)}}}", Base64Icon);

            return content;
        }

        throw new NullReferenceException("The embedded resource file error.html could not be found");
    }
}