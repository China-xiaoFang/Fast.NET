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

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.UnifyResult.Middlewares;

/// <summary>
/// 状态码中间件
/// </summary>
internal class UnifyResultStatusCodesMiddleware
{
    /// <summary>
    /// 请求委托
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="next"></param>
    public UnifyResultStatusCodesMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// 中间件执行方法
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        // 只有请求错误（短路状态码）和非 WebSocket 才支持规范化处理
        if ((context.WebSockets.IsWebSocketRequest || context.Request.Path == "/ws") || context.Response.StatusCode < 400 ||
            context.Response.StatusCode == 404)
            return;

        // 获取规范化处理器
        var unifyResult = context.RequestServices.GetService<IUnifyResultProvider>();

        // 如果为空，则不走下面的逻辑
        if (unifyResult == null)
            return;

        // 处理规范化结果
        // 解决刷新 Token 时间和 Token 时间相近问题
        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized &&
            context.Response.Headers.ContainsKey("access-token") && context.Response.Headers.ContainsKey("x-access-token"))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
        }

        // 如果 Response 已经完成输出，则禁止写入
        if (context.Response.HasStarted)
            return;
        await unifyResult.OnResponseStatusCodes(context, context.Response.StatusCode);
    }
}