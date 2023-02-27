using Fast.Core.AdminFactory.EnumFactory;
using Furion.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GetXnRestfulResult = Fast.Core.Internal.Restful.Extension.Extension;

namespace Fast.Core.Internal.Middleware;

/// <summary>
/// 演示环境中间件
/// </summary>
public class DemoEnvironmentMiddleware
{
    private readonly RequestDelegate _next;

    public DemoEnvironmentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 判断系统环境
        if (GlobalContext.SystemSettingsOptions.Environment == EnvironmentEnum.Demonstration)
        {
            var httpRequestActionEnum = context.Request.Method switch
            {
                "GET" => context.GetMetadata<HttpGetAttribute>()?.Action,
                "POST" => context.GetMetadata<HttpPostAttribute>()?.Action,
                "PUT" => context.GetMetadata<HttpPutAttribute>()?.Action,
                "DELETE" => context.GetMetadata<HttpDeleteAttribute>()?.Action,
                _ => null
            };

            if (httpRequestActionEnum != null)
            {
                if (GlobalContext.SystemSettingsOptions.DemoEnvReqDisable.Any(wh => wh == httpRequestActionEnum))
                {
                    // 抛出StatusCode为403的异常
                    await context.Response.WriteAsJsonAsync(
                        GetXnRestfulResult.GetXnRestfulResult(StatusCodes.Status403Forbidden, false, null,
                            L.Text["403 演示环境，禁止操作！"].Value), App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                }
            }
        }
        else
        {
            // 抛给下一个过滤器
            await _next(context);
        }
    }
}